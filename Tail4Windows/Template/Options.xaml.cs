﻿using System;
using System.Windows.Input;
using Org.Vs.TailForWin.Controller;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class Options
  {
    /// <summary>
    /// Update properties event handler
    /// </summary>
    public event EventHandler UpdateEvent;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public Options()
    {
      InitializeComponent();

      tabColorItem.CloseDialog += OnExit;
      tabColorItem.SaveSettings += OnSaveSettings;

      tabAboutItem.CloseDialog += OnExit;

      tabImportExportItem.CloseDialog += OnExit;

      tabOptionsItem.CloseDialog += OnExit;
      tabOptionsItem.SaveSettings += OnSaveSettings;

      tabAlertItem.CloseDialog += OnExit;
      tabAlertItem.SaveSettings += OnSaveSettings;

      tabExtrasItem.CloseDialog += OnExit;
      tabExtrasItem.SaveSettings += OnSaveSettings;

      PreviewKeyDown += HandleEsc;
    }

    #region Events

    private void OnUpdateEvent()
    {
      UpdateEvent?.Invoke(this, EventArgs.Empty);
    }

    private void OnExit(object sender, EventArgs e)
    {
      Close();
    }

    private void OnSaveSettings(object sender, EventArgs e)
    {
      SettingsHelper.SaveSettings();
      OnUpdateEvent();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        OnExit(sender, e);
    }

    #endregion
  }
}