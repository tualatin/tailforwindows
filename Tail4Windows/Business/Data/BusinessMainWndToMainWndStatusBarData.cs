using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Business.Data
{
  /// <summary>
  /// Business MainWindow to MainWindowStatusBar data
  /// </summary>
  public class BusinessMainWndToMainWndStatusBarData : NotifyMaster
  {
    private Brush _currentStatusBarBackgroundColor;

    /// <summary>
    /// Current StatusBar background color;
    /// </summary>
    public Brush CurrentStatusBarBackgroundColor
    {
      get => _currentStatusBarBackgroundColor;
      set
      {
        _currentStatusBarBackgroundColor = value;
        OnPropertyChanged(nameof(CurrentStatusBarBackgroundColor));
      }
    }
  }
}
