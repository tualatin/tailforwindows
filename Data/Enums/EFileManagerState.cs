
namespace TailForWin.Data.Enums
{
  /// <summary>
  /// Enum for FileManager state
  /// </summary>
  public enum EFileManagerState
  {
    /// <summary>
    /// Insert new file to FileManager
    /// </summary>
    AddFile,

    /// <summary>
    /// Open FileManager without new file
    /// </summary>
    OpenFileManager,

    /// <summary>
    /// Do something in FilterDialogue
    /// </summary>
    EditFilter,

    /// <summary>
    /// Edit selected item
    /// </summary>
    EditItem
  }
}
