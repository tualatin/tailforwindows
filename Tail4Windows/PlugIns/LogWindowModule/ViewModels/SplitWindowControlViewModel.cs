using System;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.ViewModels
{
  /// <summary>
  /// SplitWindowControl view model
  /// </summary>
  public class SplitWindowControlViewModel : NotifyMaster
  {
    private const double Offset = 5;

    #region Properties

    private double _splitterPosition;

    /// <summary>
    /// Current splitter height
    /// </summary>
    public double SplitterPosition
    {
      get => _splitterPosition;
      set
      {
        if ( Equals(value, _splitterPosition) )
          return;

        if ( value + (Offset - 1) > Height )
          return;

        _splitterPosition = value;
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
      double result = Math.Abs(SplitterPosition + Offset);

      if ( (int) result == (int) Offset )
        return;

      double percentage = (result * 100) / e.PreviousSize.Height;

      if ( percentage >= 100 )
      {
        SplitterPosition = Height - Offset;
        return;
      }

      double distance = e.PreviousSize.Height - result;
      double newPosition = Height - distance;

      if ( newPosition - Offset < 0 )
      {
        SplitterPosition = 0;
        return;
      }

      SplitterPosition = newPosition - Offset;
    }

    #endregion
  }
}
