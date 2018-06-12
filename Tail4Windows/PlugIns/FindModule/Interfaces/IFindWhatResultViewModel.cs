using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data;


namespace Org.Vs.TailForWin.PlugIns.FindModule.Interfaces
{
  /// <summary>
  /// FindResult view model interface
  /// </summary>
  public interface IFindWhatResultViewModel
  {
    /// <summary>
    /// Closing command
    /// </summary>
    ICommand ClosingCommand
    {
      get;
    }

    /// <summary>
    /// Loaded command
    /// </summary>
    ICommand LoadedCommand
    {
      get;
    }

    /// <summary>
    /// Filter textbox has focus
    /// </summary>
    bool FilterHasFocus
    {
      get;
    }

    /// <summary>
    /// Current filter text
    /// </summary>
    string FilterText
    {
      get;
    }

    /// <summary>
    /// FindResult view
    /// </summary>
    ListCollectionView FindWhatResultCollectionView
    {
      get;
    }

    /// <summary>
    /// Selected items
    /// </summary>
    ObservableCollection<LogEntry> SelectedItems
    {
      get;
    }

    /// <summary>
    /// List of <see cref="LogEntry"/> data source
    /// </summary>
    ObservableCollection<LogEntry> FindWhatResultSource
    {
      get;
      set;
    }

    /// <summary>
    /// Which window calls the find dialog
    /// </summary>
    Guid WindowGuid
    {
      get;
      set;
    }

    /// <summary>
    /// Left position
    /// </summary>
    double LeftPosition
    {
      get;
    }

    /// <summary>
    /// Top position
    /// </summary>
    double TopPosition
    {
      get;
    }

    /// <summary>
    /// Window height
    /// </summary>
    double WindowHeight
    {
      get;
    }

    /// <summary>
    /// Window width
    /// </summary>
    double WindowWidth
    {
      get;
    }
  }
}
