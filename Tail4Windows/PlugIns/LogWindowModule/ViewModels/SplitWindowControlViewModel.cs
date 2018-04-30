using System;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.ViewModels
{
  /// <summary>
  /// SplitWindowControl view model
  /// </summary>
  public class SplitWindowControlViewModel : NotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SplitWindowControlViewModel));

    private const double Offset = 5;

    #region Properties

    private double _splitterHeight;

    /// <summary>
    /// Current splitter height
    /// </summary>
    public double SplitterHeight
    {
      get => _splitterHeight;
      set
      {
        if ( Equals(value, _splitterHeight) )
          return;

        if ( value + (Offset - 1) > Height )
          return;

        _splitterHeight = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Current height
    /// </summary>
    public double Height
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SplitWindowControlViewModel() => SplitterHeight = 0;

    #region Commands

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    private ICommand _sizeChangedCommand;

    /// <summary>
    /// Size changed command
    /// </summary>
    public ICommand SizeChangedCommand => _sizeChangedCommand ?? (_sizeChangedCommand = new RelayCommand(p => ExecuteSizeChangedCommand((SizeChangedEventArgs) p)));

    #endregion

    #region Command functions

    private void ExecuteLoadedCommand()
    {

    }

    private void ExecuteSizeChangedCommand(SizeChangedEventArgs e)
    {
      // Calculate the distance position of GridSplitter
      double result = Math.Abs(SplitterHeight + Offset);

      if ( (int) result == (int) Offset )
        return;

      double percentage = (result * 100) / e.PreviousSize.Height;

      if ( percentage >= 100 )
      {
        SplitterHeight = Height - Offset;
        return;
      }

      double distance = e.PreviousSize.Height - result;
      double newPosition = Height - distance;

      if ( newPosition - Offset < 0 )
      {
        SplitterHeight = 0;
        return;
      }

      SplitterHeight = newPosition - Offset;
    }

    #endregion
  }
}
