using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibOrbisPkg.Rif;
using LibOrbisPkg.Util;

namespace LibOrbisPkg.PKG
{
  public class PkgBuilder
  {
    private GP4.Gp4Project project;
    private string projectDir;

    public PkgBuilder(GP4.Gp4Project proj, string proj_dir)
    {
      project = proj;
      projectDir = proj_dir;
    }

    /// <summary>
    /// Writes your PKG to the given stream.
    /// Assumes exclusive use of the stream (writes are absolute, relative to 0)
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Completed Pkg structure</returns>
    public Pkg Write(Stream s)
    {
      // Write PFS first, to get stream length
      var EKPFS = Crypto.ComputeKeys(project.volume.Package.ContentId, project.volume.Package.Passcode, 1);
      Console.WriteLine("Preparing inner PFS...");
      var innerPfs = new PFS.PfsBuilder(PFS.PfsProperties.MakeInnerPFSProps(project, projectDir), x => Console.WriteLine($"[innerpfs] {x}"));
      Console.WriteLine("Preparing outer PFS...");
      var outerPfs = new PFS.PfsBuilder(PFS.PfsProperties.MakeOuterPFSProps(project, innerPfs, EKPFS), x => Console.WriteLine($"[outerpfs] {x}"));

      var pkg = BuildPkg(outerPfs.CalculatePfsSize());
      var writer = new PkgWriter(s);
      s.Position = (long)pkg.Header.pfs_image_offset;
      var pfsStream = new OffsetStream(s, s.Position);
      outerPfs.WriteImage(pfsStream);
      
      // Update header sizes now that we know how big things are...
      pkg.Header.package_size = (ulong)s.Length;
      pkg.Header.mount_image_size = (ulong)s.Length;
      pkg.Header.pfs_image_size = (ulong)pfsStream.Length;
      pkg.Header.body_size = pkg.Header.pfs_image_offset - pkg.Header.body_offset;

      // Set PFS Image 1st block and full SHA256 hashes (mount image)
      pkg.Header.pfs_signed_digest = Crypto.Sha256(s, (long)pkg.Header.pfs_image_offset, 0x10000);
      pkg.Header.pfs_image_digest = Crypto.Sha256(s, (long)pkg.Header.pfs_image_offset, (long)pkg.Header.pfs_image_size);

      if (pkg.ParamSfo.ParamSfo.GetValueByName("PUBTOOLINFO") is SFO.Utf8Value v)
      {
        v.Value += 
          $",img0_l0_size={pfsStream.Length / (1000 * 1000)}" +
          $",img0_l1_size=0" +
          $",img0_sc_ksize=512" +
          $",img0_pc_ksize=832";
      }
      CalcGeneralDigests(pkg);
      pkg.ImageKey.FileData = Crypto.RSA2048EncryptKey(RSAKeyset.FakeKeyset.Modulus, EKPFS);
      if(pkg.Header.content_type == ContentType.GD)
      {
        CalcPlaygoDigests(pkg, s);
        pkg.ChunkDat.MchunkAttrs[0].size = (ulong)s.Length;
        pkg.ChunkDat.InnerMChunkAttrs[0].size = (ulong)innerPfs.CalculatePfsSize();
      }

      // Write body now because it will make calculating hashes easier.
      writer.WriteBody(pkg, project.volume.Package.ContentId, project.volume.Package.Passcode);

      CalcBodyDigests(pkg, s);

      // Now write header
      s.Position = 0;
      writer.WriteHeader(pkg.Header);

      // Final Pkg digest and signature
      pkg.HeaderDigest = Crypto.Sha256(s, 0, 0xFE0);
      s.Position = 0xFE0;
      s.Write(pkg.HeaderDigest, 0, pkg.HeaderDigest.Length);
      byte[] header_sha256 = Crypto.Sha256(s, 0, 0x1000);
      s.Position = 0x1000;
      s.Write(pkg.HeaderSignature = Crypto.RSA2048EncryptKey(Keys.PkgSignKey, header_sha256), 0, 256);

      return pkg;
    }

    /// <summary>
    /// Calculates the digests for the GeneralDigests entry
    /// </summary>
    /// <param name="pkg"></param>
    private static void CalcGeneralDigests(Pkg pkg)
    {
      var sfo = pkg.ParamSfo.ParamSfo;
      var majorParamString =
        "ATTRIBUTE" + sfo["ATTRIBUTE"] +
        "CATEGORY" + sfo["CATEGORY"] +
        "FORMAT" + sfo["FORMAT"] +
        "PUBTOOLVER" + sfo["PUBTOOLVER"];
      pkg.GeneralDigests.Set(GeneralDigest.MajorParamDigest,
        Crypto.Sha256(Encoding.ASCII.GetBytes(majorParamString)));
      using (var ms = new MemoryStream())
      {
        ms.Write(Encoding.ASCII.GetBytes(pkg.Header.content_id), 0, 36);
        ms.Write(new byte[12], 0, 12);
        ms.WriteInt32BE((int)pkg.Header.drm_type);
        ms.WriteInt32BE((int)pkg.Header.content_type);

        if (pkg.Header.content_type == ContentType.AC
          || pkg.Header.content_type == ContentType.GD
          || pkg.Header.content_flags.HasFlag(ContentFlags.GD_AC))
        {
          ms.Write(pkg.Header.pfs_image_digest, 0, 32);
        }
        ms.Write(pkg.GeneralDigests.Digests[GeneralDigest.MajorParamDigest], 0, 32);
        pkg.GeneralDigests.Set(GeneralDigest.ContentDigest, Crypto.Sha256(ms));
      }
      pkg.GeneralDigests.Set(GeneralDigest.GameDigest, pkg.Header.pfs_image_digest);
      using (var ms = new MemoryStream())
      {
        new PkgWriter(ms).WriteHeader(pkg.Header);
        using (var hash = System.Security.Cryptography.SHA256.Create())
        {
          ms.Position = 0;
          hash.TransformBlock(ms.ReadBytes(64), 0, 64, null, 0);
          ms.Position = 0x400;
          hash.TransformFinalBlock(ms.ReadBytes(128), 0, 128);
          pkg.GeneralDigests.Set(GeneralDigest.HeaderDigest, hash.Hash);
        }
      }
      pkg.GeneralDigests.Set(GeneralDigest.ParamDigest, Crypto.Sha256(pkg.ParamSfo.ParamSfo.Serialize()));
    }

    /// <summary>
    /// Calculates and writes the digests for the body (entries / SC filesystem)
    /// </summary>
    /// <param name="pkg"></param>
    /// <param name="s"></param>
    private static void CalcBodyDigests(Pkg pkg, Stream s)
    {
      // Entry digests
      var digests = pkg.Digests;
      var digestsOffset = pkg.Metas.Metas.Where(m => m.id == EntryId.DIGESTS).First().DataOffset;
      for (var i = 1; i < pkg.Metas.Metas.Count; i++)
      {
        var meta = pkg.Metas.Metas[i];
        var hash = Crypto.Sha256(s, meta.DataOffset, meta.DataSize);
        Buffer.BlockCopy(hash, 0, digests.FileData, 32 * i, 32);
        s.Position = digestsOffset + 32 * i;
        s.Write(hash, 0, 32);
      }

      // Body Digest: SHA256 hash of entire body segment
      pkg.Header.body_digest = Crypto.Sha256(s, (long)pkg.Header.body_offset, (long)pkg.Header.body_size);
      // Digest table hash: SHA256 hash of digest table
      pkg.Header.digest_table_hash = Crypto.Sha256(pkg.Digests.FileData);

      using (var ms = new MemoryStream())
      {
        // SC Entries Hash 1: Hash of 5 SC entries
        foreach (var entry in new Entry[] { pkg.EntryKeys, pkg.ImageKey, pkg.GeneralDigests, pkg.Metas, pkg.Digests })
        {
          new SubStream(s, entry.meta.DataOffset, entry.meta.DataSize).CopyTo(ms);
        }
        pkg.Header.sc_entries1_hash = Crypto.Sha256(ms);

        // SC Entries Hash 2: Hash of 4 SC entries
        ms.SetLength(0);
        foreach (var entry in new Entry[] { pkg.EntryKeys, pkg.ImageKey, pkg.GeneralDigests, pkg.Metas })
        {
          long size = entry.meta.DataSize;
          if(entry.Id == EntryId.METAS)
          {
            size = pkg.Header.sc_entry_count * 0x20;
          }
          new SubStream(s, entry.meta.DataOffset, size).CopyTo(ms);
        }
        pkg.Header.sc_entries2_hash = Crypto.Sha256(ms);
      }
    }

    private static void CalcPlaygoDigests(Pkg pkg, Stream s)
    {
      const int CHUNK_SIZE = 0x10000;
      for(long i = (long)pkg.Header.pfs_image_offset / CHUNK_SIZE; i < s.Length / CHUNK_SIZE; i++)
      {
        Buffer.BlockCopy(Crypto.Sha256(s, i * CHUNK_SIZE, CHUNK_SIZE), 0, pkg.ChunkSha.FileData, (int)i * 4, 4);
      }
    }

    /// <summary>
    /// Create the Pkg struct. Does not compute hashes or data sizes.
    /// </summary>
    public Pkg BuildPkg(long pfsSize)
    {
      var pkg = new Pkg();
      var volType = project.volume.Type;
      pkg.Header = new Header
      {
        CNTMagic = "\u007fCNT",
        flags = PKGFlags.Unknown,
        unk_0x08 = 0,
        unk_0x0C = 0xF,
        entry_count = 6,
        sc_entry_count = 6,
        entry_count_2 = 6,
        entry_table_offset = 0x2A80,
        main_ent_data_size = 0xD00,
        body_offset = 0x2000,
        body_size = 0x7E000,
        content_id = project.volume.Package.ContentId,
        drm_type = DrmType.PS4,
        content_type = VolTypeToContentType(volType),
        content_flags = ContentFlags.Unk_x8000000 | VolTypeToContentFlags(volType),
        // TODO
        promote_size = 0,
        version_date = 0x20161020,
        version_hash = 0x1738551,
        unk_0x88 = 0,
        unk_0x8C = 0,
        unk_0x90 = 0,
        unk_0x94 = 0,
        iro_tag = IROTag.None,
        ekc_version = 1,
        sc_entries1_hash = new byte[32],
        sc_entries2_hash = new byte[32],
        digest_table_hash = new byte[32],
        body_digest = new byte[32],
        unk_0x400 = 1,
        pfs_image_count = 1,
        pfs_flags = 0x80000000000003CC,
        pfs_image_offset = 0x80000,
        pfs_image_size = (ulong)pfsSize,
        mount_image_offset = 0,
        mount_image_size = 0,
        package_size = 0,
        pfs_signed_size = 0x10000,
        pfs_cache_size = 0xD0000,
        pfs_image_digest = new byte[32],
        pfs_signed_digest = new byte[32],
        pfs_split_size_nth_0 = 0,
        pfs_split_size_nth_1 = 0
      };
      pkg.HeaderDigest = new byte[32];
      pkg.HeaderSignature = new byte[0x100];
      pkg.EntryKeys = new KeysEntry(
        project.volume.Package.ContentId, 
        project.volume.Package.Passcode);
      pkg.ImageKey = new GenericEntry(EntryId.IMAGE_KEY)
      {
        FileData = new byte[0x100]
      };
      pkg.GeneralDigests = new GeneralDigestsEntry();
      pkg.Metas = new MetasEntry();
      pkg.Digests = new GenericEntry(EntryId.DIGESTS);
      pkg.EntryNames = new NameTableEntry();
      pkg.LicenseDat = new GenericEntry(EntryId.LICENSE_DAT) { FileData = GenLicense(pkg) };
      pkg.LicenseInfo = new GenericEntry(EntryId.LICENSE_INFO) { FileData = GenLicenseInfo(pkg) };
      var paramSfoPath = project.files.Items.Where(f => f.TargetPath == "sce_sys/param.sfo").First().OrigPath;
      using (var paramSfo = File.OpenRead(Path.Combine(projectDir, paramSfoPath)))
      {
        var sfo = SFO.ParamSfo.FromStream(paramSfo);
        pkg.ParamSfo = new SfoEntry(sfo);
        string date = "", time = "";
        if((project.volume.Package.CreationDate ?? "") == "")
        {
          date = "c_date=" + DateTime.UtcNow.ToString("yyyyMMdd");
        }
        else
        {
          var split = project.volume.Package.CreationDate.Split(' ');
          var dateTime = DateTime.Parse(project.volume.Package.CreationDate).ToUniversalTime();
          if(split.Length == 2) // Date and time specified
          {
            date = "c_date=" + dateTime.ToString("yyyyMMdd");
            time = ",c_time=" + dateTime.ToString("HHmmss");
          }
          else //  just date is specified
          {
            date = "c_date=" + dateTime.ToString("yyyyMMdd");
          }
        }
        sfo.Values.Add(new SFO.Utf8Value("PUBTOOLINFO", date+time, 0x200));
        sfo.Values.Add(new SFO.IntegerValue("PUBTOOLVER", 0x02890000));
      }
      pkg.PsReservedDat = new GenericEntry(EntryId.PSRESERVED_DAT) { FileData = new byte[0x2000] };
      pkg.Entries = new List<Entry>
      {
        pkg.EntryKeys,
        pkg.ImageKey,
        pkg.GeneralDigests,
        pkg.Metas,
        pkg.Digests,
        pkg.EntryNames
      };
      if (pkg.Header.content_type == ContentType.GD)
      {
        pkg.ChunkDat = PlayGo.ChunkDat.FromProject(project);
        pkg.ChunkSha = new GenericEntry(EntryId.PLAYGO_CHUNK_SHA, "playgo-chunk.sha");
        pkg.ChunkXml = new GenericEntry(EntryId.PLAYGO_MANIFEST_XML, "playgo-manifest.xml")
        {
          FileData = PlayGo.Manifest.Default
        };
        // Add playgo entries for GD PKGs
        pkg.Entries.AddRange(new Entry[]
        {
          pkg.ChunkDat,
          pkg.ChunkSha,
          pkg.ChunkXml
        });
      }
      pkg.Entries.AddRange(new Entry[]
      { 
        pkg.LicenseDat,
        pkg.LicenseInfo,
        pkg.ParamSfo,
        pkg.PsReservedDat
      });
      foreach(var file in project.files.Items.Where(f => f.DirName == "sce_sys/" && EntryNames.NameToId.ContainsKey(f.FileName)))
      {
        var name = file.FileName;
        if (name == "param.sfo") continue;
        var entry = new FileEntry(EntryNames.NameToId[name], Path.Combine(projectDir, file.OrigPath));
        pkg.Entries.Add(entry);
      }
      pkg.Digests.FileData = new byte[pkg.Entries.Count * Pkg.HASH_SIZE];

      // 1st pass: set names
      foreach (var entry in pkg.Entries.OrderBy(e => e.Name))
      {
        pkg.EntryNames.GetOffset(entry.Name);
      }
      // estimate size for playgo
      if (pkg.Header.content_type == ContentType.GD)
      {
        long bodySize = 0;
        foreach(var e in pkg.Entries)
        {
          bodySize += (e.Length + 15) & ~15; // round up to nearest 16
        }
        bodySize += 32 * pkg.Entries.Count; // metas
        bodySize += 4 * (pfsSize / 0x10000); // playgo hashes of pfs
        if (bodySize + (long)pkg.Header.body_offset >= (long)pkg.Header.pfs_image_offset)
        {
          pkg.Header.pfs_image_offset = (ulong)((bodySize + (long)pkg.Header.body_offset + 0xFFFF) & ~0xFFFFL);
        }
        pkg.ChunkSha.FileData = new byte[4 * ((pkg.Header.pfs_image_offset + pkg.Header.pfs_image_size) / 0x10000)];
      }
      // 2nd pass: set sizes, offsets in meta table
      var dataOffset = 0x2000u;
      var flagMap = new Dictionary<EntryId,uint>() {
        { EntryId.DIGESTS, 0x40000000 },
        { EntryId.ENTRY_KEYS, 0x60000000 },
        { EntryId.IMAGE_KEY, 0xE0000000 },
        { EntryId.GENERAL_DIGESTS, 0x60000000 },
        { EntryId.METAS, 0x60000000 },
        { EntryId.ENTRY_NAMES, 0x40000000 },
        { EntryId.LICENSE_DAT, 0x80000000 },
        { EntryId.LICENSE_INFO, 0x80000000 },
      };
      var keyMap = new Dictionary<EntryId, uint>
      {
        { EntryId.IMAGE_KEY, 3u << 12 },
        { EntryId.LICENSE_DAT, 3u << 12 },
        { EntryId.LICENSE_INFO, 2u << 12 },
      };
      foreach(var entry in pkg.Entries)
      {
        var e = new MetaEntry
        {
          id = entry.Id,
          NameTableOffset = pkg.EntryNames.GetOffset(entry.Name),
          DataOffset = dataOffset,
          DataSize = entry.Length,
          // TODO
          Flags1 = flagMap.GetOrDefault(entry.Id),
          Flags2 = keyMap.GetOrDefault(entry.Id),
        };
        pkg.Metas.Metas.Add(e);
        if(entry == pkg.Metas)
        {
          e.DataSize = (uint)pkg.Entries.Count * 32;
        }

        dataOffset += e.DataSize;

        var align = dataOffset % 16;
        if (align != 0)
          dataOffset += 16 - align;
        entry.meta = e;
      }
      pkg.Metas.Metas.Sort((e1, e2) => e1.id.CompareTo(e2.id));
      pkg.Header.entry_count = (uint)pkg.Entries.Count;
      pkg.Header.entry_count_2 = (ushort)pkg.Entries.Count;
      pkg.Header.body_size = dataOffset;
      return pkg;
    }

    private byte[] GenLicense(Pkg pkg)
    {
      var license = new LicenseDat(
        pkg.Header.content_id,
        pkg.Header.content_type,
        project.volume.Package.EntitlementKey?.FromHexCompact());
      using (var ms = new MemoryStream())
      {
        new LicenseDatWriter(ms).Write(license);
        ms.SetLength(0x400);
        return ms.ToArray();
      }
    }

    private byte[] GenLicenseInfo(Pkg pkg)
    {
      var info = new LicenseInfo(
        pkg.Header.content_id,
        pkg.Header.content_type,
        project.volume.Package.EntitlementKey?.FromHexCompact());
      using (var ms = new MemoryStream())
      {
        new LicenseInfoWriter(ms).Write(info);
        ms.SetLength(0x200);
        return ms.ToArray();
      }
    }

    private ContentType VolTypeToContentType(GP4.VolumeType t)
    {
      switch (t)
      {
        case GP4.VolumeType.pkg_ps4_app:
          return ContentType.GD;
        case GP4.VolumeType.pkg_ps4_patch:
          return ContentType.DP;
        case GP4.VolumeType.pkg_ps4_remaster:
          return ContentType.DP;
        case GP4.VolumeType.pkg_ps4_ac_data:
        case GP4.VolumeType.pkg_ps4_sf_theme:
        case GP4.VolumeType.pkg_ps4_theme:
          return ContentType.AC;
        case GP4.VolumeType.pkg_ps4_ac_nodata:
          return ContentType.AL;
        default:
          return 0;
      }
    }

    private ContentFlags VolTypeToContentFlags(GP4.VolumeType t)
    {
      switch (t)
      {
        case GP4.VolumeType.pkg_ps4_app:
        case GP4.VolumeType.pkg_ps4_ac_data:
        case GP4.VolumeType.pkg_ps4_sf_theme:
        case GP4.VolumeType.pkg_ps4_theme:
          return ContentFlags.GD_AC;
        case GP4.VolumeType.pkg_ps4_patch:
        case GP4.VolumeType.pkg_ps4_remaster:
          // TODO
          return ContentFlags.SUBSEQUENT_PATCH;
        case GP4.VolumeType.pkg_ps4_ac_nodata:
          // TODO
          return ContentFlags.NON_GAME;
        default:
          return 0;
      }
    }
  }
}
