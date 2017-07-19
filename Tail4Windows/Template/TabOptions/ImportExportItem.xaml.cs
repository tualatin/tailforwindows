using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;
using Microsoft.Win32;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.TabOptions.Interfaces;


namespace Org.Vs.TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for ImportExport.xaml
  /// </summary>
  public partial class ImportExportItem : ITabOptionItems
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ImportExportItem));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ImportExportItem()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
      textBoxConfigPath.Text = $"{AppDomain.CurrentDomain.BaseDirectory}{AppDomain.CurrentDomain.FriendlyName}.Config";
    }

    private void UCImportExportOptions_Loaded(object sender, RoutedEventArgs e)
    {
      // set special ToolTip for TabItemHeader
      ToolTip myToolTip = new ToolTip()
      {
        Style = (Style) FindResource("TabItemToolTipStyle"),
        Content = textBoxConfigPath.Text
      };
      ToolTipService.SetToolTip(textBoxConfigPath, myToolTip);

    }

    #region ITabOptionItems Members

    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;


    /// <summary>
    /// Handles Escape button blick
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        btnCancel_Click(sender, e);
    }

    /// <summary>
    /// Save clicke event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if ( SaveSettings != null )
        CloseDialog(this, EventArgs.Empty);
    }

    /// <summary>
    /// Cancel click event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      CloseDialog?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    private void btnImport_Click(object sender, RoutedEventArgs e)
    {
      if ( !LogFile.OpenFileLogDialog(out string importSettings, "Export Settings (*export)|*.export",
                                    Application.Current.FindResource("OpenDialogImportSettings") as string) )
        return;

      if ( importSettings == null )
        return;

      if ( MessageBox.Show(Application.Current.FindResource("QImportSettings") as string, LogFile.APPLICATION_CAPTION,
                         MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes )
        return;

      try
      {
        string appName = AppDomain.CurrentDomain.FriendlyName;
        FileStream importFile = new FileStream(importSettings, FileMode.Open);
        Stream output = File.Create($"{AppDomain.CurrentDomain.BaseDirectory}{appName}.Config");
        byte[] buffer = new byte[1024];
        int len;

        while ( (len = importFile.Read(buffer, 0, buffer.Length)) > 0 )
        {
          output.Write(buffer, 0, len);
        }

        output.Flush();
        importFile.Flush();

        output.Close();
        importFile.Close();

        LogFile.Settings.ReloadSettings();
        LogFile.Settings.ReadSettings();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void btnExport_Click(object sender, RoutedEventArgs e)
    {
      string appName = AppDomain.CurrentDomain.FriendlyName;
      string appSettings = $"{AppDomain.CurrentDomain.BaseDirectory}{appName}.Config";
      string date = DateTime.Now.ToString("yyyy_MM_dd_hh_mm");

      SaveFileDialog saveDialog = new SaveFileDialog
      {
        FileName = $"{date}_{appName}.Config",
        DefaultExt = ".export",
        Filter = "Export Settings (*.export)|*.export"
      };

      bool? result = saveDialog.ShowDialog();

      if ( result != true )
        return;

      try
      {
        FileStream saveFile = new FileStream(appSettings, FileMode.Open);
        Stream output = File.Create($"{saveDialog.FileName}.{saveDialog.DefaultExt}");
        byte[] buffer = new byte[1024];
        int len;

        while ( (len = saveFile.Read(buffer, 0, buffer.Length)) > 0 )
        {
          output.Write(buffer, 0, len);
        }

        saveFile.Flush();
        output.Flush();

        saveFile.Close();
        output.Close();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
  }
}
