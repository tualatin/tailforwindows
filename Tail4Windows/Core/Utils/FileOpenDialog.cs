using Microsoft.Win32;

namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// FileOpenDialog util
  /// </summary>
  public static class FileOpenDialog
  {
    /// <summary>
    /// Opens standard file open dialog
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="title">Title</param>
    /// <param name="fileName">Filename</param>
    /// <returns><c>True</c> if success, otherwise <c>False</c></returns>
    public static bool OpenDialog(string filter, string title, out string fileName)
    {
      OpenFileDialog wnd = new OpenFileDialog
      {
        Filter = filter,
        RestoreDirectory = true,
        Title = title
      };

      bool? result = wnd.ShowDialog();
      fileName = string.Empty;

      if ( result != true )
        return false;

      fileName = wnd.FileName;

      return true;
    }
  }
}
