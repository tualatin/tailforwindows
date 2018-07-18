namespace Org.Vs.TailForWin.Controllers.PlugIns.BookmarkCommentModule.Interfaces
{
  /// <summary>
  /// AddBookmarkComment view model interface
  /// </summary>
  public interface IAddBookmarkCommentViewModel
  {
    /// <summary>
    /// Gets/sets bookmark comment
    /// </summary>
    string Comment
    {
      get;
      set;
    }

    /// <summary>
    /// TextBox has focus
    /// </summary>
    bool TextBoxHasFocus
    {
      get;
    }
  }
}
