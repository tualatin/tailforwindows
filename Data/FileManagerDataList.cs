using System.Collections.ObjectModel;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// List of FileManagerData
  /// </summary>
  public class FileManagerDataList : ObservableCollection<FileManagerData>
  {
    // Creating the Tasks collection in this way enables data binding from XAML.
  }
}
