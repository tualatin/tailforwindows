using System;


namespace Org.Vs.TailForWin.Data.Events
{
  /// <summary>
  /// Holding the FileManager data inside
  /// </summary>
  public class FileManagerDataEventArgs : EventArgs, IDisposable
  {
    private FileManagerData fileManagerProperties;


    /// <summary>
    /// Releases all resources used by the FileManagerDataEventArgs.
    /// </summary>
    public void Dispose()
    {
      if(fileManagerProperties == null)
        return;

      fileManagerProperties.Dispose();
      fileManagerProperties = null;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="obj">FileManagerData object</param>
    public FileManagerDataEventArgs(FileManagerData obj)
    {
      fileManagerProperties = obj;
    }

    /// <summary>
    /// Get FileManager data
    /// </summary>
    /// <returns>FileManagerData object</returns>
    public FileManagerData GetData()
    {
      return fileManagerProperties;
    }
  }
}
