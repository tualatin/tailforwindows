using System;


namespace TailForWin.Data
{
  /// <summary>
  /// Holding the FileManager data inside
  /// </summary>
  public class FileManagerDataEventArgs: EventArgs, IDisposable
  {
    private FileManagerData fileManagerProperties;


    public void Dispose ()
    {
      if (fileManagerProperties != null)
      {
        fileManagerProperties.Dispose ( );
        fileManagerProperties = null;
      }
    }

    public FileManagerDataEventArgs (FileManagerData obj)
    {
      fileManagerProperties = obj;
    }

    /// <summary>
    /// Get FileManager data
    /// </summary>
    /// <returns>FileManagerData object</returns>
    public FileManagerData GetData ()
    {
      return (fileManagerProperties);
    }
  }
}
