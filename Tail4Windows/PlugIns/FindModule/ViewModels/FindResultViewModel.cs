using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.PlugIns.FindModule.Controller;


namespace Org.Vs.TailForWin.PlugIns.FindModule.ViewModels
{
  /// <summary>
  /// FindResult view model
  /// </summary>
  public class FindResultViewModel : NotifyMaster
  {
    private readonly List<Predicate<LogEntry>> _criteria = new List<Predicate<LogEntry>>();
    private ISettingsDbController _dbController;

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

    /// <summary>
    /// FindResult view
    /// </summary>
    public ListCollectionView FindResultCollectionView
    {
      get;
      set;
    }

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    private ObservableCollection<LogEntry> FindResultSource
    {
      get;
      set;
    } = new ObservableCollection<LogEntry>();

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
          FindResultCollectionView.Filter = DynamicFilter;
          return;
        }

        _criteria.Add(p => p.Message.ToLower().Contains(_filterText));
        FindResultCollectionView.Filter = DynamicFilter;
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

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindResultViewModel()
    {
      _dbController = new SettingsDbController();
      _dbController.ReadDbSettings();
      SetupFindResultCollecitonView();
    }

    private void SetupFindResultCollecitonView()
    {
      FindResultCollectionView = (ListCollectionView) new CollectionViewSource { Source = FindResultSource }.View;
      FindResultCollectionView.CustomSort = new LogEntryComparer();
      FindResultCollectionView.Filter = DynamicFilter;

      FilterHasFocus = true;
    }

    private bool DynamicFilter(object item)
    {
      LogEntry t = item as LogEntry;

      if ( _criteria.Count == 0 )
        return true;

      var result = _criteria.TrueForAll(p => p(t));

      return result;
    }
  }
}
