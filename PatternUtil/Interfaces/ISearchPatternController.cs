using System;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.PatternUtil.Interfaces
{
  /// <summary>
  /// Search pattern controller interface
  /// </summary>
  public interface ISearchPatternController
  {
    /// <summary>
    /// Releases all resources used by SearchPatternController.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Get current log file by certain saved pattern
    /// </summary>
    /// <param name="tailProperty">TailForLog property</param>
    /// <returns>Log file with full path otherwise null</returns>
    /// <exception cref="ArgumentException">If tailProperty is null</exception>
    string GetCurrentFileByPattern(TailLogData tailProperty);
  }
}