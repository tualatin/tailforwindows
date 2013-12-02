﻿using System.Windows.Controls;
using System;
using System.Windows.Input;
using TailForWin.Controller;
using TailForWin.Data;
using System.Windows;
using System.Text.RegularExpressions;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for AlertsItem.xaml
  /// </summary>
  public partial class AlertsItem: UserControl, ITabItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;


    public AlertsItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;  
    }

    #region ClickEvents
    
    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (SaveSettings != null)
        SaveSettings (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    private void btnOpenSoundFile_Click (object sender, RoutedEventArgs e)
    {
      string fName = string.Empty;

      if (LogFile.OpenFileLogDialog (out fName, "MP3 (*.mp3)|*.mp3|Wave (*.wav)|*.wav|All files (*.*)|*.*", Application.Current.FindResource ("SelectSoundFile") as string))
        textBoxSoundFile.Text = fName;
    }

    #endregion

    #region Events

    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }

    private void UserControl_Loaded (object sender, RoutedEventArgs e)
    {
      alertOptions.DataContext = SettingsHelper.TailSettings.AlertSettings;
    }

    private void UserControl_Drop (object sender, DragEventArgs e)
    {
      try
      {
        var text = e.Data.GetData (DataFormats.FileDrop);

        if (text != null)
        {
          string fileName = string.Format ("{0}", ((string[]) text)[0]);
          string extension = System.IO.Path.GetExtension (fileName);

          Regex regex = new Regex (LogFile.REGEX_SOUNDFILE_EXTENSION);

          if (!regex.IsMatch (extension))
          {
            MessageBox.Show (Application.Current.FindResource ("NoSoundFile").ToString ( ), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
          }

          textBoxSoundFile.Text = fileName;
        }
      }
      catch (Exception ex)
      {
#if DEBUG
        Console.WriteLine (string.Format ("AlertsItem Drop exception {0}", ex));
#endif
      }
      finally
      {
        e.Handled = true;
      }
    }

    private void UserControl_DragEnter (object sender, DragEventArgs e)
    {
      if (e.Source == sender)
        e.Effects = DragDropEffects.None;

      e.Handled = true;
    }

    private void textBoxSoundFile_PreviewDragOver (object sender, DragEventArgs e)
    {
      e.Handled = true;
    }

    #endregion
  }
}
