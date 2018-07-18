using Org.Vs.TailForWin.Controllers.PlugIns.BookmarkCommentModule.Interfaces;

namespace Org.Vs.TailForWin.PlugIns.BookmarkCommentModule
{
  /// <summary>
  /// Interaction logic for AddBookmarkComment.xaml
  /// </summary>
  public partial class AddBookmarkComment
  {
    private readonly IAddBookmarkCommentViewModel _addBookmarkCommentViewModel;

    /// <summary>
    /// Gets/sets bookmark comment
    /// </summary>
    public string Comment
    {
      get => _addBookmarkCommentViewModel == null ? string.Empty : _addBookmarkCommentViewModel.Comment;
      set
      {
        if (_addBookmarkCommentViewModel == null)
          return;

        _addBookmarkCommentViewModel.Comment = value;
      }
    }

    /// <summary>
    /// constructor
    /// </summary>
    public AddBookmarkComment()
    {
      InitializeComponent();
      _addBookmarkCommentViewModel = (IAddBookmarkCommentViewModel) DataContext;
    }
  }
}
