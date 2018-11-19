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
      var pkg = BuildPkg();
      var writer = new PkgWriter(s);

      // Write PFS first, to get stream length
      s.Position = (long) pkg.Header.pfs_image_offset;
      var EKPFS = Crypto.ComputeKeys(project.volume.Package.ContentId, project.volume.Package.Passcode, 1);
      var pfsStream = new OffsetStream(s, s.Position);
      Console.WriteLine("Preparing inner PFS...");
      var innerPfs = new PFS.PfsBuilder(PFS.PfsProperties.MakeInnerPFSProps(project, projectDir), Console.WriteLine);
      Console.WriteLine("Preparing outer PFS...");
      var outerPfs = new PFS.PfsBuilder(PFS.PfsProperties.MakeOuterPFSProps(innerPfs, EKPFS), Console.WriteLine);
      outerPfs.WriteImage(pfsStream);

      if(pkg.ParamSfo.ParamSfo.GetValueByName("PUBTOOLINFO") is SFO.Utf8Value v)
      {
        v.Value += 
          $",img0_l0_size={pfsStream.Length / (1000 * 1000)}" +
          $",img0_l1_size=0" +
          $",img0_sc_ksize=512" +
          $",img0_pc_ksize=576";
      }
      // TODO: Generate hashes in Entries (body)
      pkg.GeneralDigests.ParamDigest = Crypto.Sha256(pkg.ParamSfo.ParamSfo.Serialize());
      pkg.ImageKey.FileData = Crypto.RSA2048EncryptKey(RSAKeyset.FakeKeyset.Modulus.Reverse().ToArray(), EKPFS);

      // Write body now because it will make calculating hashes easier.
      writer.WriteBody(pkg, project.volume.Package.ContentId, project.volume.Package.Passcode);

      CalcHeaderHashes(pkg, s);

      // Update header sizes now that we know how big things are...
      UpdateHeaderInfo(pkg, s.Length, pfsStream.Length);

      // Now write header
      s.Position = 0;
      writer.WritePkg(pkg);

      // Pkg Signature
      byte[] header_sha256 = Crypto.Sha256(s, 0, 0x1000);
      s.Position = 0x1000;
      s.Write(Crypto.RSA2048EncryptKey(Keys.PkgSignKey, header_sha256), 0, 256);

      return pkg;
    }

    private void UpdateHeaderInfo(Pkg pkg, long stream_length, long pfs_length)
    {
      pkg.Header.body_size = pkg.Header.pfs_image_offset - pkg.Header.body_offset;
      pkg.Header.package_size = (ulong)stream_length;
      pkg.Header.mount_image_size = (ulong)stream_length;
      pkg.Header.pfs_image_size = (ulong)pfs_length;
    }

    private void CalcHeaderHashes(Pkg pkg, Stream s)
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
          new SubStream(s, entry.meta.DataOffset, entry.meta.DataSize).CopyTo(ms);
        }
        pkg.Header.sc_entries2_hash = Crypto.Sha256(ms);
      }

      // PFS Image 1st block and full SHA256 hashes
      pkg.Header.pfs_signed_digest = Crypto.Sha256(s, (long)pkg.Header.pfs_image_offset, 0x10000);
      pkg.Header.pfs_image_digest = Crypto.Sha256(s, (long)pkg.Header.pfs_image_offset, (long)pkg.Header.pfs_image_size);
    }

    /// <summary>
    /// Create the Pkg struct. Does not compute hashes or data sizes.
    /// </summary>
    public Pkg BuildPkg()
    {
      var pkg = new Pkg();
      var volType = GP4.VolumeTypeUtil.OfString(project.volume.Type);
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
        pfs_image_size = 0,
        mount_image_offset = 0,
        mount_image_size = 0,
        package_size = 0,
        pfs_signed_size = 0x10000,
        pfs_cache_size = 0x90000,
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
      pkg.GeneralDigests = new GeneralDigestsEntry()
      {
        UnknownDigest = new byte[] {
          0xD2, 0x56, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x6E
        },
        ContentDigest = new byte[32],
        GameDigest = new byte[32],
        HeaderDigest = new byte[32],
        SystemDigest = new byte[32],
        MajorParamDigest = new byte[32],
        ParamDigest = new byte[32],
      };
      pkg.Metas = new MetasEntry();
      pkg.Digests = new GenericEntry(EntryId.DIGESTS);
      pkg.EntryNames = new NameTableEntry();
      pkg.LicenseDat = new GenericEntry(EntryId.LICENSE_DAT) { FileData = GenLicense(pkg) };
      pkg.LicenseInfo = new GenericEntry(EntryId.LICENSE_INFO) { FileData = GenLicenseInfo(pkg) };
      var paramSfoPath = project.files.Where(f => f.TargetPath == "sce_sys/param.sfo").First().OrigPath;
      using (var paramSfo = File.OpenRead(Path.Combine(projectDir, paramSfoPath)))
      {
        var sfo = SFO.ParamSfo.FromStream(paramSfo);
        pkg.ParamSfo = new SfoEntry(sfo);
        sfo.Values.Add(new SFO.Utf8Value("PUBTOOLINFO", "c_date=20181107", 0x200));
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
        pkg.EntryNames,
        pkg.LicenseDat,
        pkg.LicenseInfo,
        pkg.ParamSfo,
        pkg.PsReservedDat
      };
      pkg.Digests.FileData = new byte[pkg.Entries.Count * Pkg.HASH_SIZE];

      // 1st pass: set names
      foreach (var entry in pkg.Entries)
      {
        pkg.EntryNames.GetOffset(entry.Name);
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
          Flags1 = flagMap.ContainsKey(entry.Id) ? flagMap[entry.Id] : 0,
          Flags2 = keyMap.ContainsKey(entry.Id) ? keyMap[entry.Id] : 0,
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
        project.volume.Package.EntitlementKey.FromHexCompact());
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
        project.volume.Package.EntitlementKey.FromHexCompact());
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
