using System;
using System.IO;

namespace LibOrbisPkgTests
{

  /// <summary>
  /// Class that creates a temp dir on construction and deletes it on dispose.
  /// </summary>
  class TempDir : IDisposable
  {
    /// <summary>
    /// The path to the temporary directory.
    /// </summary>
    public string Path { get; }
    /// <summary>
    /// Creates a new temporary directory that is deleted upon Dispose.
    /// </summary>
    public TempDir()
    {
      Path = System.IO.Path.GetTempFileName();
      File.Delete(Path);
      Directory.CreateDirectory(Path);
    }
    public void Dispose()
    {
      if(Directory.Exists(Path))
        Directory.Delete(Path, true);
    }
  }
}
