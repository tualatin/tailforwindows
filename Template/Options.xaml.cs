using System.Windows;
using System;
using TailForWin.Controller;
using System.Windows.Input;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class Options: Window
  {
    /// <summary>
    /// Update properties event handler
    /// </summary>
    public event EventHandler UpdateEvent;


    public Options ()
    {
      InitializeComponent ( );

      tabColorItem.CloseDialog += OnExit;
      tabColorItem.SaveSettings += OnSaveSettings;

      tabAboutItem.CloseDialog += OnExit;

      tabSysInfoItem.CloseDialog += OnExit;

      tabOptionsItem.CloseDialog += OnExit;
      tabOptionsItem.SaveSettings += OnSaveSettings;

      tabAlertItem.CloseDialog += OnExit;
      tabAlertItem.SaveSettings += OnSaveSettings;

      PreviewKeyDown += HandleEsc;
    }

    #region Events
    
    private void OnUpdateEvent ()
    {
      if (UpdateEvent != null)
        UpdateEvent (this, EventArgs.Empty);
    }

    private void OnExit (object sender, EventArgs e)
    {
      Close ( );
    }

    private void OnSaveSettings (object sender, EventArgs e)
    {
      SettingsHelper.SaveSettings ( );
      OnUpdateEvent ( );
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit (sender, e);
    }

    #endregion
  }
}
