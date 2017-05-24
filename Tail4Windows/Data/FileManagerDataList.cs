using System;
using System.Collections;
using System.Collections.ObjectModel;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// List of FileManagerData
  /// </summary>
  public class FileManagerDataList : ObservableCollection<FileManagerData>, IComparer
  {
    // Creating the Tasks collection in this way enables data binding from XAML.

    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x">FileManagerData x</param>
    /// <param name="y">FileManagerData y</param>
    /// <returns>Compareable result</returns>
    public int Compare(object x, object y)
    {
      if(x is FileManagerData && y is FileManagerData)
      {
        var xFm = x as FileManagerData;
        var yFm = y as FileManagerData;

        DateTime nx = xFm.FileCreationTime ?? DateTime.MaxValue;
        DateTime ny = yFm.FileCreationTime ?? DateTime.MaxValue;

        return -nx.CompareTo(ny);
      }
      return 1;
    }
  }
}
