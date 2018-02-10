using Org.Vs.TailForWin.Business.Data;


namespace Org.Vs.TailForWin.Business.Controllers
{
  /// <summary>
  /// Business MainWnd to MainWndStatusBar singelton controller
  /// </summary>
  public class BusinessMainWndToMainWndStatusBarController
  {
    private static BusinessMainWndToMainWndStatusBarController instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static BusinessMainWndToMainWndStatusBarController Instance => instance ?? (instance = new BusinessMainWndToMainWndStatusBarController());

    /// <summary>
    /// Current business MainWnd to MainWndStatusBar data
    /// </summary>
    public BusinessMainWndToMainWndStatusBarData CurrentData
    {
      get;
    }

    private BusinessMainWndToMainWndStatusBarController() => CurrentData = new BusinessMainWndToMainWndStatusBarData();
  }
}
