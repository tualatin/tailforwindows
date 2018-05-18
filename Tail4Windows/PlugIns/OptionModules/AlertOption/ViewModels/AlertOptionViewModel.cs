using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption.ViewModels
{
  /// <summary>
  /// AlertOption view model
  /// </summary>
  public class AlertOptionViewModel : NotifyMaster, IFileDragDropTarget, IAlertOptionViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(AlertOptionViewModel));

    #region Commands

    private IAsyncCommand _sendTestMailCommand;

    /// <summary>
    /// Send test mail command
    /// </summary>
    public IAsyncCommand SendTestMailCommand => _sendTestMailCommand ?? (_sendTestMailCommand = AsyncCommand.Create(ExecuteSendTestMailAsync));

    private ICommand _selectSoundFileCommand;

    /// <summary>
    /// Select sound file command
    /// </summary>
    public ICommand SelectSoundFileCommand => _selectSoundFileCommand ?? (_selectSoundFileCommand = new RelayCommand(p => ExecuteSelectSoundFile()));

    private ICommand _previewDragEnterCommand;

    /// <summary>
    /// Preview drag enter command
    /// </summary>
    public ICommand PreviewDragEnterCommand => _previewDragEnterCommand ?? (_previewDragEnterCommand = new RelayCommand(ExecutePreviewDragEnterCommand));

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new NotImplementedException();

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

    #endregion

    #region Command functions

    private void ExecutePreviewDragEnterCommand(object parameter)
    {
      if ( !(parameter is DragEventArgs e) )
        return;

      e.Handled = true;
      e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Move : DragDropEffects.None;
    }

    private async Task ExecuteSendTestMailAsync()
    {
      if ( string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.SmtpSettings.FromAddress)
          || string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.SmtpSettings.SmtpServerName) )
      {
        if ( InteractionService.ShowQuestionMessageBox(Application.Current.TryFindResource("AlertOptionSmtpSettingsNotValid").ToString()) == MessageBoxResult.No )
          return;

        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenSmtpSettingMessage(this));
        return;
      }

      IMailController mailController = new MailController();
      await mailController.SendLogMailAsync(Application.Current.TryFindResource("AlertOptionTestMail").ToString()).ConfigureAwait(false);
    }

    private void ExecuteSelectSoundFile()
    {
      if ( InteractionService.OpenFileDialog(out string fileName, "MP3(*.mp3)|*.mp3|Wave(*.wav)|*.wav|All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle) )
        SettingsHelperController.CurrentSettings.AlertSettings.SoundFileNameFullPath = fileName;
    }

    #endregion

    /// <summary>
    /// On file drop
    /// </summary>
    /// <param name="filePaths">Array of file pathes</param>
    public void OnFileDrop(string[] filePaths)
    {
      if ( filePaths.Length == 0 )
        return;

      try
      {
        string fileName = filePaths.First();
        string extension = System.IO.Path.GetExtension(fileName);

        if ( string.IsNullOrWhiteSpace(extension) )
          return;

        var regex = new Regex(@"^\.(mp3|wav)");

        if ( !regex.IsMatch(extension) )
        {
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("NoSoundFile").ToString());
          return;
        }

        SettingsHelperController.CurrentSettings.AlertSettings.SoundFileNameFullPath = fileName;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }
  }
}
