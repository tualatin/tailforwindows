using System;
using System.Collections;
using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.Data
{
  /// <summary>
  /// FileManager data collection
  /// </summary>
  public class FileManagerData : ObservableCollection<TailData>, IComparer
  {
    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x">FileManagerData x</param>
    /// <param name="y">FileManagerData y</param>
    /// <returns>Compareable result</returns>
    public int Compare(object x, object y)
    {
      if (!(x is TailData) || !(y is TailData))
        return 1;

      var xFm = (TailData)x;
      var yFm = (TailData)y;

      var nx = xFm.FileCreationTime ?? DateTime.MaxValue;
      var ny = yFm.FileCreationTime ?? DateTime.MaxValue;

      return -nx.CompareTo(ny);
    }
  }
}
