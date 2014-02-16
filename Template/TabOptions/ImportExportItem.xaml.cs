using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using TailForWin.Utils;
using TailForWin.Data;
using TailForWin.Controller;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for ImportExport.xaml
  /// </summary>
  public partial class ImportExportItem : ITabOptionItems
  {
    public ImportExportItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      textBoxConfigPath.Text = string.Format ("{0}{1}.Config", AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
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


    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }

    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (SaveSettings != null)
        CloseDialog (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    #endregion

    private void btnImport_Click (object sender, RoutedEventArgs e)
    {
      string importSettings;

      if (!LogFile.OpenFileLogDialog (out importSettings, "Export Settings (*export)|*.export",
                                    Application.Current.FindResource ("OpenDialogImportSettings") as string))
        return;

      if (importSettings == null)
        return;

      if (MessageBox.Show (Application.Current.FindResource ("QImportSettings") as string, LogFile.APPLICATION_CAPTION,
                         MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        return;

      try
      {
        string appName = AppDomain.CurrentDomain.FriendlyName;
        FileStream importFile = new FileStream (importSettings, FileMode.Open);
        Stream output = File.Create (string.Format ("{0}{1}.Config", AppDomain.CurrentDomain.BaseDirectory, appName));
        byte[] buffer = new byte[1024];
        int len;

        while ((len = importFile.Read (buffer, 0, buffer.Length)) > 0)
        {
          output.Write (buffer, 0, len);
        }

        output.Flush ( );
        importFile.Flush ( );

        output.Close ( );
        importFile.Close ( );

        SettingsHelper.ReloadSettings ( );
        SettingsHelper.ReadSettings ( );
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, GetType (  ).Name, string.Format ("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod (  )));
      }
    }

    private void btnExport_Click (object sender, RoutedEventArgs e)
    {
      string appName = AppDomain.CurrentDomain.FriendlyName;
      string appSettings = string.Format ("{0}{1}.Config", AppDomain.CurrentDomain.BaseDirectory, appName);
      string date = DateTime.Now.ToString ("yyyy_MM_dd_hh_mm");
     
      SaveFileDialog saveDialog = new SaveFileDialog
      {
        FileName = string.Format ("{0}_{1}.Config", date, appName),
        DefaultExt = ".export",
        Filter = "Export Settings (*.export)|*.export"
      };

      bool? result = saveDialog.ShowDialog ( );

      if (result != true)
        return;

      try
      {
        FileStream saveFile = new FileStream (appSettings, FileMode.Open);
        Stream output = File.Create (string.Format ("{0}.{1}", saveDialog.FileName, saveDialog.DefaultExt));
        byte[] buffer = new byte[1024];
        int len;

        while ((len = saveFile.Read (buffer, 0, buffer.Length)) > 0)
        {
          output.Write (buffer, 0, len);
        }

        saveFile.Flush ( );
        output.Flush ( );

        saveFile.Close ( );
        output.Close ( );
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, GetType (  ).Name, string.Format ("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod (  )));
      }
    }
  }
}
