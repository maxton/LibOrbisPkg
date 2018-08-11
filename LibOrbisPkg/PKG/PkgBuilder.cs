using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
      var pfsStream = new OffsetStream(s, s.Position);
      new PFS.PfsBuilder().BuildPfs(new PFS.PfsProperties
      {
        BlockSize = 65536,
        output = pfsStream,
        proj = project,
        projDir = projectDir,
      });
      var align = s.Length % 65536;
      if (align != 0)
        s.SetLength(s.Length + 65536 - align);

      // TODO: Encrypt PFS (could also be done while writing image)
      // TODO: Generate hashes in Entries (body)
      // TODO: Calculate keys in entries (image key, etc)

      // Write body now because it will make calculating hashes easier.
      writer.WriteBody(pkg);

      CalcHeaderHashes(pkg, s);

      // Update header sizes now that we know how big things are...
      UpdateHeaderInfo(pkg, s.Length, pfsStream.Length);

      // Now write header
      s.Position = 0;
      writer.WritePkg(pkg);
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
      // Body Digest: SHA256 hash of entire body segment
      pkg.Header.body_digest = Crypto.Sha256(s, (long)pkg.Header.body_offset, (long)pkg.Header.body_size);
      // Digest table hash: SHA256 hash of digest table
      pkg.Header.digest_table_hash = Crypto.Sha256(pkg.Digests.FileData);

      using (var ms = new MemoryStream())
      {
        // SC Entries Hash 1: Hash of 5 SC entries
        foreach (var entry in new Entry[] { pkg.EntryKeys, pkg.ImageKey, pkg.GeneralDigests, pkg.Metas, pkg.Digests })
        {
          entry.Write(ms);
        }
        pkg.Header.sc_entries1_hash = Crypto.Sha256(ms);

        // SC Entries Hash 2: Hash of 4 SC entries
        ms.SetLength(0);
        foreach (var entry in new Entry[] { pkg.EntryKeys, pkg.ImageKey, pkg.GeneralDigests, pkg.Metas })
        {
          entry.Write(ms);
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
      pkg.EntryKeys = new KeysEntry();
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
          0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE
        },
        ContentDigest = new byte[32],
        GameDigest = new byte[32],
        HeaderDigest = new byte[32],
        UnknownDigest2 = new byte[32],
        MajorParamDigest = new byte[32],
        ParamDigest = new byte[32],
      };
      pkg.Metas = new MetasEntry();
      pkg.Digests = new GenericEntry(EntryId.DIGESTS);
      pkg.EntryNames = new NameTableEntry();
      pkg.LicenseDat = new GenericEntry(EntryId.LICENSE_DAT) { FileData = new byte[1024] };
      pkg.LicenseInfo = new GenericEntry(EntryId.LICENSE_INFO) { FileData = new byte[512] };
      var paramSfoPath = project.files.Where(f => f.TargetPath == "sce_sys/param.sfo").First().OrigPath;
      using (var paramSfo = File.OpenRead(Path.Combine(projectDir, paramSfoPath)))
        pkg.ParamSfo = new GenericEntry(EntryId.PARAM_SFO, "param.sfo")
        {
          FileData = SFO.ParamSfo.Update(paramSfo)
        };
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
      foreach(var entry in pkg.Entries)
      {
        var e = new MetaEntry
        {
          id = entry.Id,
          NameTableOffset = pkg.EntryNames.GetOffset(entry.Name),
          DataOffset = dataOffset,
          DataSize = entry.Length,
          // TODO
          Flags1 = 0,
          Flags2 = 0,
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
      }
      pkg.Metas.Metas.Sort((e1, e2) => e1.id.CompareTo(e2.id));
      pkg.Header.entry_count = (uint)pkg.Entries.Count;
      pkg.Header.entry_count_2 = (ushort)pkg.Entries.Count;
      pkg.Header.body_size = dataOffset;
      return pkg;
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
