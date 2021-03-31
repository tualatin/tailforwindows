using System;
using System.Threading.Tasks;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for SmtpOptionPage.xaml
  /// </summary>
  public partial class SmtpOptionPage : IOptionPage
  {
    private NotifyTaskCompletion _notifyTaskCompletion;
    private string _pw;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmtpOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("AlertSmtpOptionPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("cfc162ef-5755-4958-a559-ab893ca8e1ed");

    /// <summary>
    /// Current page settings changed
    /// </summary>
    public bool PageSettingsChanged => false;

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

    private async Task SetPasswordAsync() => _pw = await StringEncryption.DecryptAsync(SettingsHelperController.CurrentSettings.SmtpSettings.Password).ConfigureAwait(false);

    private void UserControlUnloaded(object sender, RoutedEventArgs e) => NotifyTaskCompletion.Create(SavePasswordAsync);

    private async Task SavePasswordAsync()
    {
      if ( string.IsNullOrWhiteSpace(PasswordBox.Password) )
      {
        SettingsHelperController.CurrentSettings.SmtpSettings.Password = string.Empty;
        return;
      }

      SettingsHelperController.CurrentSettings.SmtpSettings.Password = await StringEncryption.EncryptAsync(PasswordBox.Password).ConfigureAwait(false);
    }
  }
}
