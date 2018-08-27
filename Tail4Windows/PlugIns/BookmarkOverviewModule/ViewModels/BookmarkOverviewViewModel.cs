using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Utils;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.BookmarkOverviewModule.ViewModels
{
  /// <summary>
  /// Bookmark overview module view model
  /// </summary>
  public class BookmarkOverviewViewModel : NotifyMaster, IBookmarkOverviewViewModel
  {
    private readonly List<Predicate<LogEntry>> _criteria = new List<Predicate<LogEntry>>();

    #region Properties

    private double _topPosition;

    /// <summary>
    /// Top position
    /// </summary>
    public double TopPosition
    {
      get => _topPosition;
      set
      {
        _topPosition = value;
        OnPropertyChanged();
      }
    }

    private double _leftPosition;

    /// <summary>
    /// Left position
    /// </summary>
    public double LeftPosition
    {
      get => _leftPosition;
      set
      {
        _leftPosition = value;
        OnPropertyChanged();
      }
    }

    private double _windowHeight;

    /// <summary>
    /// Window height
    /// </summary>
    public double WindowHeight
    {
      get => _windowHeight;
      set
      {
        _windowHeight = value;
        OnPropertyChanged();
      }
    }

    private double _windowWidth;

    /// <summary>
    /// Window width
    /// </summary>
    public double WindowWidth
    {
      get => _windowWidth;
      set
      {
        _windowWidth = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Bookmark view
    /// </summary>
    public ListCollectionView BookmarkCollectionView
    {
      get;
      set;
    }

    private ObservableCollection<LogEntry> _bookmarkSource;

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    public ObservableCollection<LogEntry> BookmarkSource
    {
      get => _bookmarkSource;
      set
      {
        _bookmarkSource = value;

        OnPropertyChanged();
        SetupFindResultCollectionView();
      }
    }

    private ObservableCollection<LogEntry> _selectedItems;

    /// <summary>
    /// SelectedItems
    /// </summary>
    public ObservableCollection<LogEntry> SelectedItems
    {
      get => _selectedItems;
      set
      {
        if ( value == _selectedItems )
          return;

        _selectedItems = value;
        OnPropertyChanged();
      }
    }

    private LogEntry _selectedItem;

    /// <summary>
    /// Selected item
    /// </summary>
    public LogEntry SelectedItem
    {
      get => _selectedItem;
      set
      {
        if ( Equals(value, _selectedItem) )
          return;

        _selectedItem = value;
        OnPropertyChanged();
      }
    }

    private string _filterText;

    /// <summary>
    /// Current filter text
    /// </summary>
    public string FilterText
    {
      get => _filterText;
      set
      {
        if ( Equals(value, _filterText) )
          return;

        _filterText = value;
        OnPropertyChanged();

        _criteria.Clear();

        if ( string.IsNullOrWhiteSpace(_filterText) )
        {
          BookmarkCollectionView.Filter = DynamicFilter;
          return;
        }

        _criteria.Add(p => p.Message.ToLower().Contains(_filterText));
        BookmarkCollectionView.Filter = DynamicFilter;
      }
    }

    private bool _filterHasFocus;

    /// <summary>
    /// Filter has focus
    /// </summary>
    public bool FilterHasFocus
    {
      get => _filterHasFocus;
      set
      {
        _filterHasFocus = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Which window calls the find dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BookmarkOverviewViewModel()
    {
      BookmarkSource = new ObservableCollection<LogEntry>();
    }

    #region Commands

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new System.NotImplementedException();

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new System.NotImplementedException();

    #endregion

    #region Command functions

    #endregion

    private void SetupFindResultCollectionView()
    {
      FilterHasFocus = false;

      if ( BookmarkCollectionView == null )
      {
        FilterHasFocus = true;
        return;
      }

      BookmarkCollectionView = (ListCollectionView) new CollectionViewSource { Source = BookmarkSource }.View;
      BookmarkCollectionView.CustomSort = new LogEntryComparer();
      BookmarkCollectionView.Filter = DynamicFilter;

      SelectedItem = BookmarkCollectionView.Count == 0 ? null : BookmarkSource.First();
      FilterHasFocus = true;
      OnPropertyChanged(nameof(BookmarkCollectionView));
    }

    private bool DynamicFilter(object item)
    {
      var t = item as LogEntry;

      if ( _criteria.Count == 0 )
        return true;

      bool result = _criteria.TrueForAll(p => p(t));

      return result;
    }
  }
}
