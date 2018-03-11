using System;
using System.Windows;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Enums;


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

    private BusinessMainWndToMainWndStatusBarController()
    {
    }

    private static readonly Lazy<BusinessMainWndToMainWndStatusBarData> CurrentData = new Lazy<BusinessMainWndToMainWndStatusBarData>(() => new BusinessMainWndToMainWndStatusBarData());

    /// <summary>
    /// Current business MainWnd to MainWndStatusBar data
    /// </summary>
    public static BusinessMainWndToMainWndStatusBarData CurrentBusinessData => CurrentData.Value;

    /// <summary>
    /// Set current business data
    /// </summary>
    /// <param name="statusbarState"><see cref="EStatusbarState"/></param>
    public void SetCurrentBusinessData(EStatusbarState statusbarState)
    {
      switch ( statusbarState )
      {
      case EStatusbarState.FileLoaded:

        CurrentBusinessData.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex;
        break;

      case EStatusbarState.Busy:

        CurrentBusinessData.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex;
        break;

      case EStatusbarState.Default:

        CurrentBusinessData.CurrentStatusBarBackgroundColorHex = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex;
        CurrentBusinessData.CurrentBusyState = Application.Current.TryFindResource("TrayIconReady").ToString();
        break;

      default:

        throw new NotImplementedException();
      }
    }
  }
}
