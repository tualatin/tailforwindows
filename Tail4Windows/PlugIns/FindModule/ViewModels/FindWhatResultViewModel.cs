using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.FindModule.Controller;
using Org.Vs.TailForWin.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.UserControls;


namespace Org.Vs.TailForWin.PlugIns.FindModule.ViewModels
{
  /// <summary>
  /// FindResult view model
  /// </summary>
  public class FindWhatResultViewModel : NotifyMaster, IFindWhatResultViewModel
  {
    private readonly List<Predicate<LogEntry>> _criteria = new List<Predicate<LogEntry>>();
    private readonly ISettingsDbController _dbController;

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
    /// FindResult view
    /// </summary>
    public ListCollectionView FindWhatResultCollectionView
    {
      get;
      set;
    }

    private ObservableCollection<LogEntry> _findWhatResultSource;

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    public ObservableCollection<LogEntry> FindWhatResultSource
    {
      get => _findWhatResultSource;
      set
      {
        _findWhatResultSource = value;

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
          FindWhatResultCollectionView.Filter = DynamicFilter;
          return;
        }

        _criteria.Add(p => p.Message.ToLower().Contains(_filterText));
        FindWhatResultCollectionView.Filter = DynamicFilter;
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
    public FindWhatResultViewModel()
    {
      _dbController = SettingsDbController.Instance;
      FindWhatResultSource = new ObservableCollection<LogEntry>();
    }

    #region Commands

    private ICommand _loadeCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadeCommand ?? (_loadeCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    private ICommand _closingCommand;

    /// <summary>
    /// Closing command
    /// </summary>
    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand(p => ExecuteClosingCommand()));

    private ICommand _findWhatResultMouseDoubleClickCommand;

    /// <summary>
    /// Mouse double click command
    /// </summary>
    public ICommand FindWhatResultMouseDoubleClickCommand => _findWhatResultMouseDoubleClickCommand ?? (_findWhatResultMouseDoubleClickCommand =
                                                               new RelayCommand(ExecuteFindWhatResultMouseDoubleClickCommand));

    #endregion

    #region Command functions

    private void ExecuteFindWhatResultMouseDoubleClickCommand(object param)
    {
      if ( !(param is MouseButtonEventArgs e) )
        return;

      if ( !(e.Source is VsDataGrid dg) )
        return;

      if ( !(dg.CurrentItem is LogEntry selectedItem) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new JumpToSelectedLogEntryMessage(WindowGuid, selectedItem));
    }

    private void ExecuteClosingCommand()
    {
      SettingsHelperController.CurrentSettings.FindResultPositionX = LeftPosition;
      SettingsHelperController.CurrentSettings.FindResultPositionY = TopPosition;

      SettingsHelperController.CurrentSettings.FindResultHeight = WindowHeight;
      SettingsHelperController.CurrentSettings.FindResultWidth = WindowWidth;

      _dbController.UpdateFindResultDbSettings();
    }

    private void ExecuteLoadedCommand()
    {
      TopPosition = SettingsHelperController.CurrentSettings.FindResultPositionY;
      LeftPosition = SettingsHelperController.CurrentSettings.FindResultPositionX;

      WindowHeight = SettingsHelperController.CurrentSettings.FindResultHeight;
      WindowWidth = SettingsHelperController.CurrentSettings.FindResultWidth;

      FilterHasFocus = true;
    }

    #endregion

    private void SetupFindResultCollectionView()
    {
      FilterHasFocus = false;

      if ( FindWhatResultSource == null )
      {
        FilterHasFocus = true;
        return;
      }

      FindWhatResultCollectionView = (ListCollectionView) new CollectionViewSource { Source = FindWhatResultSource }.View;
      FindWhatResultCollectionView.CustomSort = new LogEntryComparer();
      FindWhatResultCollectionView.Filter = DynamicFilter;

      FilterHasFocus = true;
      OnPropertyChanged(nameof(FindWhatResultCollectionView));
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
