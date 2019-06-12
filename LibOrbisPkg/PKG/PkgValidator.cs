using LibOrbisPkg.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibOrbisPkg.PKG
{
  public class PkgValidator
  {
    public enum ValidationType
    {
      Hash,
      Signature
    }
    public class Validation
    {
      /// <summary>
      /// The type of validation being checked.
      /// </summary>
      public readonly ValidationType Type;
      /// <summary>
      /// Short name of the digest
      /// </summary>
      public readonly string Name;
      /// <summary>
      /// Human-readable description of this validation step.
      /// </summary>
      public readonly string Description;
      /// <summary>
      /// The offset of the final hash or digest in the PKG file;
      /// </summary>
      public readonly long Location;
      public Func<bool> Validate;
      public Validation(ValidationType t, string name, string desc, long location)
      {
        Type = t;
        Name = name;
        Description = desc;
        Location = location;
      }
      public override bool Equals(object obj)
      => obj is Validation v ? v.Type == Type && v.Name == Name && v.Location == Location : false;
      public override int GetHashCode()
      => Crypto.CombineHashCodes(
          Type.GetHashCode(),
          Name.GetHashCode(),
          Description.GetHashCode(),
          Location.GetHashCode());
    }

    private Pkg pkg;
    public PkgValidator(Pkg pkg)
    {
      this.pkg = pkg;
    }

    private Dictionary<GeneralDigest, Tuple<string, string>> GeneralDigests =
      new Dictionary<GeneralDigest, Tuple<string, string>> {
        { GeneralDigest.ContentDigest,
          Tuple.Create("Content Digest",
            "A hash of the Content ID, DRM type, Content Type, PFS Image digest, and Major Param digest") },
        { GeneralDigest.GameDigest,
          Tuple.Create("Game Digest",
            "The PFS image digest") },
        { GeneralDigest.HeaderDigest,
          Tuple.Create("Header Digest",
            "A hash of the first 64 bytes and the 128 bytes at 0x400 of the PKG") },
        { GeneralDigest.SystemDigest,
          Tuple.Create("System Digest",
            "???") },
        { GeneralDigest.MajorParamDigest,
          Tuple.Create("Major Param Digest",
            "A hash of the ATTRIBUTE, CATEGORY, FORMAT, and PUBTOOLVER param.sfo entries") },
        { GeneralDigest.ParamDigest,
          Tuple.Create("Param Digest",
            "A hash of the entire param.sfo file (entry)") },
        { GeneralDigest.PlaygoDigest,
          Tuple.Create("Playgo Digest",
            "???") },
        { GeneralDigest.TrophyDigest,
          Tuple.Create("Trophy Digest",
            "???") },
        { GeneralDigest.ManualDigest,
          Tuple.Create("Manual Digest",
            "???") },
        { GeneralDigest.KeymapDigest,
          Tuple.Create("Keymap Digest",
            "???") },
        { GeneralDigest.OriginDigest,
          Tuple.Create("Origin Digest",
            "???") },
        { GeneralDigest.TargetDigest,
          Tuple.Create("Target Digest",
            "???") },
        { GeneralDigest.OriginGameDigest,
          Tuple.Create("Origin Game Digest",
            "???") },
        { GeneralDigest.TargetGameDigest,
          Tuple.Create("Target Game Digest",
            "???") },
      };

    public List<Validation> Validations(Stream pkgStream)
    {
      var validations = new List<Validation>();

      // Create validators for PKG general digests
      foreach(var d in pkg.GeneralDigests.Digests)
      {
        if (d.Key == GeneralDigest.DigestMetaData) continue;
        var meta = GeneralDigests[d.Key];
        var generalDigestsLoc = pkg.GeneralDigests.meta.DataOffset;
        var validation = new Validation(
          ValidationType.Hash, 
          meta.Item1, 
          meta.Item2, 
          generalDigestsLoc + ((int)d.Key * 32));
        switch (d.Key)
        {
          case GeneralDigest.ContentDigest:
            validation.Validate = () =>
              pkg.GeneralDigests.Digests[GeneralDigest.ContentDigest].SequenceEqual(pkg.ComputeContentDigest());
            break;
          case GeneralDigest.GameDigest:
            validation.Validate = () =>
              pkg.GeneralDigests.Digests[GeneralDigest.GameDigest].SequenceEqual(pkg.Header.pfs_image_digest);
            break;
          case GeneralDigest.HeaderDigest:
            validation.Validate = () =>
              pkg.GeneralDigests.Digests[GeneralDigest.HeaderDigest].SequenceEqual(pkg.ComputeHeaderDigest());
            break;
          case GeneralDigest.MajorParamDigest:
            validation.Validate = () =>
              pkg.GeneralDigests.Digests[GeneralDigest.MajorParamDigest].SequenceEqual(pkg.ComputeMajorParamDigest());
            break;
          case GeneralDigest.ParamDigest:
            validation.Validate = () =>
              pkg.GeneralDigests.Digests[GeneralDigest.ParamDigest].SequenceEqual(Crypto.Sha256(pkg.ParamSfo.ParamSfo.Serialize()));
            break;
          default:
            continue;
        }
        validations.Add(validation);
      }

      validations.Add(new Validation(
        ValidationType.Hash,
        "PFS Signed Digest",
        "A hash of the first 0x10000 bytes of the PFS image",
        0x460)
        {
          Validate = () =>
            Crypto.Sha256(pkgStream, (long)pkg.Header.pfs_image_offset, 0x10000).SequenceEqual(pkg.Header.pfs_signed_digest)
        });

      validations.Add(new Validation(
        ValidationType.Hash,
        "PFS Image Digest",
        "A hash of the entire PFS image",
        0x440)
        {
          Validate = () =>
            Crypto.Sha256(pkgStream, (long)pkg.Header.pfs_image_offset, (long)pkg.Header.pfs_image_size)
            .SequenceEqual(pkg.Header.pfs_image_digest)
        });

      return validations;
    }

    /// <summary>
    /// Checks the hashes and signatures for this PKG.
    /// </summary>
    /// <returns>Returns a list of validation steps and their success (true) or failure (false).</returns>
    public IEnumerable<Tuple<Validation, bool>> Validate(Stream pkgStream)
    {
      foreach(var validation in Validations(pkgStream))
      {
        yield return Tuple.Create(validation, validation.Validate());
      }
    }
  }
}
