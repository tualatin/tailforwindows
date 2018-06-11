using System;
using Org.Vs.TailForWin.BaseView.UserControls.Interfaces;


namespace Org.Vs.TailForWin.BaseView.UserControls
{
  /// <summary>
  /// Interaction logic for MainWindowQuickSearchBar.xaml
  /// </summary>
  public partial class MainWindowQuickSearchBar
  {
    private readonly IMainWindowQuickSearchViewModel _mainWindowQuickSearch;

    /// <summary>
    /// Window Guid
    /// </summary>
    public Guid WindowGuid
    {
      get => _mainWindowQuickSearch?.WindowGuid ?? Guid.Empty;
      set
      {
        if ( _mainWindowQuickSearch == null )
          return;

        _mainWindowQuickSearch.WindowGuid = value;
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public MainWindowQuickSearchBar()
    {
      InitializeComponent();

      _mainWindowQuickSearch = (IMainWindowQuickSearchViewModel) DataContext;
    }
  }
}
