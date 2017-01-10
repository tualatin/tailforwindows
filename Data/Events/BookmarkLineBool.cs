using System;


namespace Org.Vs.TailForWin.Data.Events
{
  /// <summary>
  /// BookmarLineBool object
  /// </summary>
  public class BookmarkLineBool : EventArgs
  {
    /// <summary>
    /// Bookmark line
    /// </summary>
    public bool BookmarkLine
    {
      get;
      set;
    }
  }
}
