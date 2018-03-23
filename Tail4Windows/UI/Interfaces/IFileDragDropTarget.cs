namespace Org.Vs.TailForWin.UI.Interfaces
{
  /// <summary>
  /// File Drag and Drop interface
  /// </summary>
  public interface IFileDragDropTarget
  {
    /// <summary>
    /// On file drop
    /// </summary>
    /// <param name="filePaths">Array of file pathes</param>
    void OnFileDrop(string[] filePaths);
  }
}
