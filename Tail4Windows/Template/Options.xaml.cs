using System;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.TabOptions.Interfaces;


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

      InjectMyInterface(tabColorItem);
      InjectMyInterface(tabAboutItem);
      InjectMyInterface(tabImportExportItem);
      InjectMyInterface(tabOptionsItem);
      InjectMyInterface(tabAlertItem);
      InjectMyInterface(tabExtrasItem);

      PreviewKeyDown += HandleEsc;
    }

    private void InjectMyInterface(ITabOptionItems tabOptionItem)
    {
      tabOptionItem.CloseDialog += OnExit;
      tabOptionItem.SaveSettings += OnSaveSettings;
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
      LogFile.Settings.SaveSettings();
      OnUpdateEvent();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        OnExit(sender, e);
    }

    #endregion
  }
}
