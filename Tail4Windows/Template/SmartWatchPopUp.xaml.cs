using System;
using System.Windows;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SmartWatchPopUp.xaml
  /// </summary>
  public partial class SmartWatchPopUp : Window
  {
    public SmartWatchPopUp()
    {
      InitializeComponent();
    }

    private void SmartWatchWnd_Loaded(object sender, RoutedEventArgs e)
    {
      Activate();
      Focus();
    }

    private void SmartWatchWnd_Deactivated(object sender, EventArgs e)
    {
      Topmost = true;
      Activate();
      Focus();
    }
  }
}
