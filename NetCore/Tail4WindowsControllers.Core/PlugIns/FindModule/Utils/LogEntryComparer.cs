﻿using System.Collections;
using Org.Vs.TailForWin.Business.Services.Data;

namespace Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Utils
{
  /// <summary>
  /// <see cref="LogEntry"/> comparer
  /// </summary>
  public class LogEntryComparer : IComparer
  {
    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x"><see cref="LogEntry"/> x</param>
    /// <param name="y"><see cref="LogEntry"/> y</param>
    /// <returns>Compareable result</returns>
    public int Compare(object x, object y)
    {
      if ( !(x is LogEntry) || !(y is LogEntry) )
        return 1;

      var xFm = (LogEntry) x;
      var yFm = (LogEntry) y;

      int nx = xFm.Index;
      int ny = yFm.Index;

      return nx.CompareTo(ny);
    }
  }
}
