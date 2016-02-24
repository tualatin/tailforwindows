﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TailForWin.Controller;
using TailForWin.Data;
using TailForWin.Data.Enums;
using TailForWin.Utils;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for OptionsItem.xaml
  /// </summary>
  public partial class OptionsItem : ITabOptionItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;

    private readonly bool isInit;
    private string sendToLnkName;


    public OptionsItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      sendToLnkName = string.Format ("{0}\\{1}.lnk", Environment.GetFolderPath (Environment.SpecialFolder.SendTo), LogFile.APPLICATION_CAPTION);

      Rename_BtnSendTo ( );
      SetComboBoxes ( );
      SetControls ( );

      isInit = true;
    }
    
    #region ClickEvents
    
    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.LinesRead = spinnerNLines.StartIndex;

      if (SaveSettings != null)
        SaveSettings (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    private void btnReset_Click (object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show (Application.Current.FindResource ("QResetSettings") as string,
                         Application.Current.FindResource ("Question") as string, MessageBoxButton.YesNo,
                         MessageBoxImage.Question) != MessageBoxResult.Yes)
        return;

      SettingsHelper.SetToDefault ( );
      SetControls ( );
    }

    private void btnProxy_Click (object sender, RoutedEventArgs e)
    {
      Window wnd = Window.GetWindow (this);
      ProxyServer ps = new ProxyServer { Owner = wnd };

      ps.ShowDialog ( );
    }

    private void btnSendToMenu_Click (object sender, RoutedEventArgs e)
    {
      try
      {
        if (File.Exists (sendToLnkName))
        {
          File.Delete (sendToLnkName);
          Rename_BtnSendTo ( );
        }
        else
        {
          IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell ( );
          IWshRuntimeLibrary.IWshShortcut shortCut = shell.CreateShortcut (sendToLnkName);
          shortCut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly ( ).Location;
          shortCut.Save ( );
          Rename_BtnSendTo ( );
        }
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, GetType ( ).Name, string.Format ("{0}, exception: {1}", System.Reflection.MethodBase.GetCurrentMethod ( ).Name, ex));
      }
    }

    #endregion

    #region Events

    private void UserControl_Loaded (object sender, RoutedEventArgs e)
    {
      gridOptions.DataContext = SettingsHelper.TailSettings;
    }

    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }

    private void comboBoxThreadPriority_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultThreadPriority = (System.Threading.ThreadPriority) Enum.Parse (typeof (System.Threading.ThreadPriority), comboBoxThreadPriority.SelectedItem as string);
    }

    private void comboBoxThreadRefreshRate_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultRefreshRate = (ETailRefreshRate) Enum.Parse (typeof (ETailRefreshRate), comboBoxThreadRefreshRate.SelectedItem as string);
    }

    private void comboBoxTimeFormat_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultTimeFormat = SettingsData.GetDescriptionEnum<ETimeFormat> (comboBoxTimeFormat.SelectedItem as string);
    }

    private void comboBoxDateFormat_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultDateFormat = SettingsData.GetDescriptionEnum<EDateFormat> (comboBoxDateFormat.SelectedItem as string);
    }

    #endregion

    #region HelperFunctions

    private void Rename_BtnSendTo ( )
    {
      if (File.Exists (sendToLnkName))
        btnSendToMenu.Content = "Remove 'SendTo'";
      else
        btnSendToMenu.Content = "Add 'SendTo'";
    }

    private void SetControls ()
    {
      comboBoxThreadPriority.SelectedItem = SettingsHelper.TailSettings.DefaultThreadPriority.ToString ( );
      comboBoxThreadRefreshRate.SelectedItem = SettingsHelper.TailSettings.DefaultRefreshRate.ToString ( );
      comboBoxTimeFormat.SelectedItem = SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultTimeFormat);
      comboBoxDateFormat.SelectedItem = SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultDateFormat);

      spinnerNLines.StartIndex = SettingsHelper.TailSettings.LinesRead;
    }

    private void SetComboBoxes ()
    {
      Array.ForEach (Enum.GetNames (typeof(System.Threading.ThreadPriority)), priorityName => comboBoxThreadPriority.Items.Add (priorityName));
      comboBoxThreadPriority.SelectedIndex = 0;

      Array.ForEach (Enum.GetNames (typeof(ETailRefreshRate)), refreshName => comboBoxThreadRefreshRate.Items.Add (refreshName));
      comboBoxThreadRefreshRate.SelectedIndex = 0;

      foreach (ETimeFormat timeFormat in Enum.GetValues (typeof (ETimeFormat)))
      {
        string item = SettingsData.GetEnumDescription (timeFormat);
        comboBoxTimeFormat.Items.Add (item);
      }

      comboBoxTimeFormat.SelectedIndex = 0;

      foreach (EDateFormat dateFormat in Enum.GetValues (typeof (EDateFormat)))
      {
        string item = SettingsData.GetEnumDescription (dateFormat);
        comboBoxDateFormat.Items.Add (item);
      }

      comboBoxDateFormat.SelectedIndex = 0;
    }

    #endregion
  }
}
