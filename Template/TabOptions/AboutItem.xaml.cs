using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using TailForWin.Utils;
using TailForWin.Data;
using System;
using System.Windows.Input;
using System.Diagnostics;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for AboutItem.xaml
  /// </summary>
  public partial class AboutItem: UserControl, ITabOptionItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    public event EventHandler SaveSettings;


    public AboutItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      Assembly assembly = Assembly.GetExecutingAssembly ( );
      labelBuildDate.Content = (BuildDate.GetBuildDateTime (assembly)).ToString ("dd.MM.yyyy HH:mm:ss");
      labelAppName.Content = LogFile.APPLICATION_CAPTION;
      labelVersion.Content = assembly.GetName ( ).Version;
    }

    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (SaveSettings != null)
        SaveSettings (this, EventArgs.Empty);

      throw new NotImplementedException ( );
    }

    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnSave_Click (sender, e);
    }

    private void Hyperlink_RequestNavigate (object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
      e.Handled = true;
    }
  }
}
