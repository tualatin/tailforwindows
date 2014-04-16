using System.Windows;
using System.Windows.Input;


namespace TailForWin.Template.TaskBarNotifier
{
  /// <summary>
  /// Interaction logic for Settings.xaml
  /// </summary>
  public partial class Settings: Window
  {
    public Settings ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      DataContext = TailForWin.Controller.SettingsHelper.TailSettings.AlertSettings.PopupWndSettings;
    }

    private void btnOK_Click (object sender, RoutedEventArgs e)
    {
      SaveSettings ( );
      OnExit ( );
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      OnExit ( );
    }

    #region HelperFunctions

    private void OnExit ( )
    {
      Close ( );
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit ( );
    }

    private void SaveSettings ( )
    {
      TailForWin.Controller.SettingsHelper.TailSettings.AlertSettings.PopupWndSettings.HidingMilliseconds = spinnerHidingTime.StartIndex;
      TailForWin.Controller.SettingsHelper.TailSettings.AlertSettings.PopupWndSettings.OpeningMilliseconds = spinnerOpeningTime.StartIndex;
      TailForWin.Controller.SettingsHelper.TailSettings.AlertSettings.PopupWndSettings.StayOpenMilliseconds = spinnerStayOpenTime.StartIndex;

      TailForWin.Controller.SettingsHelper.SaveSettings ( );
    }

    #endregion
  }
}
