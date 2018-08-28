using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Business.Services.Data;
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

    #region Properties

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    public ObservableCollection<LogEntry> BookmarkSource
    {
      get => _bookmarkViewModel?.BookmarkSource;
      set
      {
        if ( _bookmarkViewModel == null )
          return;

        _bookmarkViewModel.BookmarkSource = value;
      }
    }

    #endregion

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
