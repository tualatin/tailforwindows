using System.Threading.Tasks;
using System.Windows;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for ProxyOptionPage.xaml
  /// </summary>
  public partial class ProxyOptionPage : IOptionPage
  {
    private NotifyTaskCompletion _notifyTaskCompletion;
    private string _pw;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ProxyOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("EnvironmentProxyOptionPageTitle").ToString();

    private void UserControlLoaded(object sender, RoutedEventArgs e)
    {
      _notifyTaskCompletion = NotifyTaskCompletion.Create(SetPasswordAsync);
      _notifyTaskCompletion.PropertyChanged += NotifyTaskCompletionPropertyChanged;
    }

    private void NotifyTaskCompletionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      PasswordBox.Password = _pw;
      _pw = null;

      _notifyTaskCompletion.PropertyChanged -= NotifyTaskCompletionPropertyChanged;
    }

    private async Task SetPasswordAsync()
    {
      _pw = await StringEncryption.DecryptAsync(SettingsHelperController.CurrentSettings.ProxySettings.Password).ConfigureAwait(false);
    }
  }
}
