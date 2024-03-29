﻿using System.Collections;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;

namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Utils
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
    /// <returns>Comparable result</returns>
    public int Compare(object x, object y)
    {
      if ( !(x is TailData xFm) || !(y is TailData yFm) )
        return 1;

      switch ( SettingsHelperController.CurrentSettings.DefaultFileSort )
      {
      case EFileSort.FileCreationTime:

        if ( xFm.IsWindowsEvent || yFm.IsWindowsEvent )
          return FileCompare(xFm, yFm);

        DateTime nx = xFm.FileCreationTime ?? DateTime.MaxValue;
        DateTime ny = yFm.FileCreationTime ?? DateTime.MaxValue;

        return nx.CompareTo(ny);

      default:

        return FileCompare(xFm, yFm);
      }
    }

    private static int FileCompare(TailData xFm, TailData yFm)
    {
      string xs = xFm.File;
      string ys = yFm.File;

      try
      {
        // ReSharper disable once StringCompareToIsCultureSpecific
        return xs.CompareTo(ys);
      }
      catch
      {
        return 1;
      }
    }
  }
}
