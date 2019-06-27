using System;
using System.IO;

namespace LibOrbisPkgTests
{

  /// <summary>
  /// Class that creates a temp file on construction and deletes it on dispose.
  /// </summary>
  class TempFile : IDisposable
  {
    /// <summary>
    /// The path to the temporary file.
    /// </summary>
    public string Path { get; }
    /// <summary>
    /// Creates a new temporary file that is deleted upon Dispose.
    /// </summary>
    public TempFile()
    {
      Path = System.IO.Path.GetTempFileName();
    }
    public void Dispose()
    {
      if(File.Exists(Path))
        File.Delete(Path);
    }
  }
}
