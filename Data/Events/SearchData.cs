﻿using System;


namespace Org.Vs.TailForWin.Data.Events
{
  /// <summary>
  /// SearchData class
  /// </summary>
  public class SearchData : EventArgs
  {
    /// <summary>
    /// Word what is to find
    /// </summary>
    public string WordToFind
    {
      get;
      set;
    }

    /// <summary>
    /// Count matches only?
    /// </summary>
    public bool Count
    {
      get;
      set;
    }

    /// <summary>
    /// Search for bookmarks only
    /// </summary>
    public bool SearchBookmarks
    {
      get;
      set;
    }
  }
}