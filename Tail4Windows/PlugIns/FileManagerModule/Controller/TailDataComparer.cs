using System;
using System.Collections;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller
{
  /// <summary>
  /// TailData comparer
  /// </summary>
  public class TailDataComparer : IComparer
  {
    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x"><see cref="TailData"/> x</param>
    /// <param name="y"><see cref="TailData"/> y</param>
    /// <returns>Compareable result</returns>
    public int Compare(object x, object y)
    {
      if ( !(x is TailData) || !(y is TailData) )
        return 1;

      var xFm = (TailData) x;
      var yFm = (TailData) y;

      var nx = xFm.FileCreationTime ?? DateTime.MaxValue;
      var ny = yFm.FileCreationTime ?? DateTime.MaxValue;

      return -nx.CompareTo(ny);
    }
  }
}
