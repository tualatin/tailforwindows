using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.TabOptions.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for AlertsItem.xaml
  /// </summary>
  public partial class AlertsItem : ITabOptionItems
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(AlertsItem));

    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public AlertsItem()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
    }

    #region ClickEvents

    public void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if ( checkBoxSendMail.IsChecked == true )
      {
        if ( SettingsHelper.ParseEMailAddress(watermarkTextBoxEMailAddress.Text) )
        {
          SaveSettings?.Invoke(this, EventArgs.Empty);
        }
        else
        {
          MessageBox.Show(Application.Current.FindResource("EMailAddressNotValid") as string, CentralManager.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
          watermarkTextBoxEMailAddress.Focus();
        }
      }
      else
      {
        SaveSettings?.Invoke(this, EventArgs.Empty);
      }
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      CloseDialog?.Invoke(this, EventArgs.Empty);
    }

    private void btnOpenSoundFile_Click(object sender, RoutedEventArgs e)
    {
      if ( CentralManager.OpenFileLogDialog(out string fName, "MP3 (*.mp3)|*.mp3|Wave (*.wav)|*.wav|All files (*.*)|*.*", Application.Current.FindResource("SelectSoundFile") as string) )
        textBoxSoundFile.Text = fName;
    }

    private void btnTestEMail_Click(object sender, RoutedEventArgs e)
    {
      MailClient mailClient = new MailClient();
      mailClient.SendMailComplete += (o, a) => mailClient.Dispose();
      mailClient.InitClient();
      mailClient.SendMail("testMessage");
    }

    private void btnSmtp_Click(object sender, RoutedEventArgs e)
    {
      Window wnd = Window.GetWindow(this);
      SmtpSettings smtp = new SmtpSettings
      {
        Owner = wnd
      };

      smtp.ShowDialog();
    }

    #endregion

    #region Events

    /// <summary>
    /// Handle Escape key
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        btnCancel_Click(sender, e);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      alertOptions.DataContext = SettingsHelper.TailSettings.AlertSettings;
    }

    private void UserControl_Drop(object sender, DragEventArgs e)
    {
      e.Handled = true;

      try
      {
        var text = e.Data.GetData(DataFormats.FileDrop);

        if ( text == null )
          return;

        string fileName = string.Format("{0}", ((string[]) text)[0]);
        string extension = System.IO.Path.GetExtension(fileName);

        Regex regex = new Regex(CentralManager.REGEX_SOUNDFILE_EXTENSION);

        if ( !regex.IsMatch(extension) )
        {
          MessageBox.Show(Application.Current.FindResource("NoSoundFile") as string, CentralManager.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        textBoxSoundFile.Text = fileName;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void UserControl_DragEnter(object sender, DragEventArgs e)
    {
      e.Handled = true;

      if ( e.Source == sender )
        e.Effects = DragDropEffects.None;
    }

    private void textBoxSoundFile_PreviewDragOver(object sender, DragEventArgs e)
    {
      e.Handled = true;
    }

    #endregion
  }
}
