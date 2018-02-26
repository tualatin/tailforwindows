using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Business.Data
{
  /// <summary>
  /// Business MainWindow to MainWindowStatusBar data
  /// </summary>
  public class BusinessMainWndToMainWndStatusBarData : NotifyMaster
  {
    private string _currentStatusBarBackgroundColorHex;

    /// <summary>
    /// Current StatusBar background color;
    /// </summary>
    public string CurrentStatusBarBackgroundColorHex
    {
      get => _currentStatusBarBackgroundColorHex;
      set
      {
        if ( Equals(value, _currentStatusBarBackgroundColorHex) )
          return;

        _currentStatusBarBackgroundColorHex = value;
        OnPropertyChanged(nameof(CurrentStatusBarBackgroundColorHex));
      }
    }

    private string _currentBusyState;

    /// <summary>
    /// Current busy state
    /// </summary>
    public string CurrentBusyState
    {
      get => _currentBusyState;
      set
      {
        _currentBusyState = value;
        OnPropertyChanged(nameof(CurrentBusyState));
      }
    }
  }
}
