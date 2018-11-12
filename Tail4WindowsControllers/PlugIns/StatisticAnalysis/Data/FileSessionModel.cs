using System;


namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data
{
  /// <summary>
  /// File session model
  /// </summary>
  public class FileSessionModel
  {
    /// <summary>
    /// Session count
    /// </summary>
    public int SessionCount
    {
      get;
      set;
    }

    /// <summary>
    /// Date time of item
    /// </summary>
    public DateTime Date
    {
      get;
      set;
    }

    /// <summary>
    /// Time span
    /// </summary>
    public TimeSpan TimeSpan
    {
      get;
      set;
    }

    /// <summary>
    /// File size
    /// </summary>
    public double FileSize
    {
      get;
      set;
    }

    /// <summary>
    /// Bookmark count
    /// </summary>
    public ulong BookmarkCount
    {
      get;
      set;
    }

    /// <summary>
    /// Is SmartWatch enabled
    /// </summary>
    public bool IsSmartWatch
    {
      get;
      set;
    }

    /// <summary>
    /// Is a Windows event log
    /// </summary>
    public bool IsWindowsEvent
    {
      get;
      set;
    }

    /// <summary>
    /// Log count
    /// </summary>
    public ulong LogCount
    {
      get;
      set;
    }
  }
}
