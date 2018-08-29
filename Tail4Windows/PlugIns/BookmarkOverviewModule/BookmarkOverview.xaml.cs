using Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.BookmarkOverviewModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.BookmarkOverviewModule
{
  /// <summary>
  /// Interaction logic for BookmarkOverview.xaml
  /// </summary>
  public partial class BookmarkOverview
  {
    private readonly IBookmarkOverviewViewModel _bookmarkViewModel;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BookmarkOverview()
    {
      InitializeComponent();

      _bookmarkViewModel = (BookmarkOverviewViewModel) DataContext;
    }
  }
}
