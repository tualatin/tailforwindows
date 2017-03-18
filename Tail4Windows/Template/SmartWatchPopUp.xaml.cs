using System;
using System.Windows;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.Events;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SmartWatchPopUp.xaml
  /// </summary>
  public partial class SmartWatchPopUp : Window
  {
    /// <summary>
    /// Fires, when user accept the dialog
    /// </summary>
    public event SmartWatchOpenFileEventHandler SmartWatchOpenFile;

    /// <summary>
    /// New file
    /// </summary>
    public string NewFileOpen
    {
      get;
      set;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchPopUp()
    {
      InitializeComponent();
    }

    private void SmartWatchWnd_Loaded(object sender, RoutedEventArgs e)
    {
      SmartWatchWnd.DataContext = SettingsHelper.TailSettings.SmartWatchData;
      LblNewFile.Text = $"File '{NewFileOpen}' detected. What should {LogFile.APPLICATION_CAPTION} do?";

      Activate();
      Focus();
      BtnOpenSameTab.Focus();
    }

    private void SmartWatchWnd_Deactivated(object sender, EventArgs e)
    {
    }

    private void BtnIgnore_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void BtnOpenSameTab_Click(object sender, RoutedEventArgs e)
    {
      SmartWatchOpenFile?.Invoke(this, false);
      Close();
    }

    private void BtnOpenNewTab_Click(object sender, RoutedEventArgs e)
    {
      SmartWatchOpenFile?.Invoke(this, true);
      Close();
    }
  }
}
