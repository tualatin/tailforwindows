using System.Windows;
using TailForWin.Controller;
using System.Windows.Input;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaktionslogik für ProxyServer.xaml
  /// </summary>
  public partial class ProxyServer : Window
  {
    public ProxyServer ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;
    }

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      DataContext = SettingsHelper.TailSettings.ProxySettings;
    }

    private void btnOK_Click (object sender, RoutedEventArgs e)
    {
      OnExit ( );
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      OnExit ( );
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit ( );
    }

    private void OnExit ()
    {
      Close ( );
    }
  }
}
