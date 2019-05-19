using LibOrbisPkg.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibOrbisPkg.PKG
{
  public class PkgValidator
  {
    private Pkg pkg_;
    private Action<string> errorHandler;
    public PkgValidator(Pkg pkg, Action<string> onError = null)
    {
      pkg_ = pkg;
      errorHandler = onError;
    }

    /// <summary>
    /// Checks the hashes and signatures for this PKG.
    /// </summary>
    /// <returns>Returns a list of failed hashes. If the list is empty then the PKG is valid.</returns>
    public List<string> Validate(Stream pkgStream)
    {
      var errors = new List<string>();
      Action<string> onError = e => { errors.Add(e); errorHandler?.Invoke(e); };

      // Check general digests
      var generalDigests = pkg_.CalcGeneralDigests();
      foreach(var d in generalDigests)
      {
        if(!d.Value.SequenceEqual(pkg_.GeneralDigests.Digests[d.Key]))
        {
          onError(d.Key.ToString());
        }
      }

      // Check PFS Signed digest
      var pfsSignedDigest = Crypto.Sha256(pkgStream, (long)pkg_.Header.pfs_image_offset, 0x10000);
      if (!pfsSignedDigest.SequenceEqual(pkg_.Header.pfs_signed_digest))
        onError(nameof(pkg_.Header.pfs_signed_digest));

      // Check PFS Image digest
      var pfsImageDigest = Crypto.Sha256(pkgStream, (long)pkg_.Header.pfs_image_offset, (long)pkg_.Header.pfs_image_size);
      if (!pfsImageDigest.SequenceEqual(pkg_.Header.pfs_image_digest))
        onError(nameof(pkg_.Header.pfs_image_digest));

      return errors;
    }
  }
}
