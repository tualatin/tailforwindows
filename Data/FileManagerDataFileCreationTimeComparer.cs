using System;
using System.Collections.Generic;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// FileCreationTime comparer
  /// </summary>
  public class FileManagerDataFileCreationTimeComparer : IComparer<FileManagerData>
  {
    #region IComparer<DateTime?> Members

    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x">FileManagerData x</param>
    /// <param name="y">FileManagerData y</param>
    /// <returns>Compareable result</returns>
    public int Compare(FileManagerData x, FileManagerData y)
    {
      DateTime nx = x.FileCreationTime ?? DateTime.MaxValue;
      DateTime ny = y.FileCreationTime ?? DateTime.MaxValue;

      return (-(nx.CompareTo(ny)));
    }

    #endregion
  }
}
