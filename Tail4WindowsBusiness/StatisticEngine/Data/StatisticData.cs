﻿using System;


namespace Org.Vs.TailForWin.Business.StatisticEngine.Data
{
  /// <summary>
  /// StatisticData object
  /// </summary>
  public class StatisticData
  {
    /// <summary>
    /// LogReader Id
    /// </summary>
    public Guid LogReaderId
    {
      get;
    }

    /// <summary>
    /// Current index
    /// </summary>
    public int Index
    {
      get;
    }

    /// <summary>
    /// File with path
    /// </summary>
    public string FileName
    {
      get;
    }

    /// <summary>
    /// Elapsed time as <see cref="TimeSpan"/>
    /// </summary>
    public TimeSpan? ElapsedTime
    {
      get;
    }

    /// <summary>
    /// Is Windows event
    /// </summary>
    public bool IsWindowsEvent
    {
      get;
    }

    /// <summary>
    /// Count of Bookmarks
    /// </summary>
    public int BookmarkCount
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="fileName">File with path</param>
    /// <param name="bookmarkCount">Count of Bookmarks</param>
    /// <param name="elapsedTime">Elapsed time as <see cref="TimeSpan"/></param>
    /// <param name="isWindowsEvent">Is Windows event</param>
    public StatisticData(Guid logReaderId, int index, string fileName, int bookmarkCount, TimeSpan? elapsedTime, bool isWindowsEvent = false)
    {
      LogReaderId = logReaderId;
      Index = index;
      FileName = fileName;
      BookmarkCount = bookmarkCount;
      IsWindowsEvent = isWindowsEvent;
      ElapsedTime = elapsedTime;
    }
  }
}
