using System.Windows;
using System.Text.RegularExpressions;
using TailForWin.Controller;
using TailForWin.Data;
using System;


namespace TailForWin
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup (object sender, StartupEventArgs e)
    {
      MainWindow wnd = new MainWindow ( );
      wnd.Show ( );

      if (e.Args.Length > 0)
      {
        foreach (string arg in e.Args)
        {
          Match match = Regex.Match (arg, @"/id=");

          if (match.Success)
          {
            Match id = Regex.Match (arg, @"\d+");

            if (id.Success)
            {
              FileManagerStructure fm = new FileManagerStructure ( );
              FileManagerData item = fm.GetNodeById (id.Value);

              if (item != null)
              {
                FileManagerDataEventArgs args = new FileManagerDataEventArgs (item);
                wnd.FileManagerTab (this, args);
                args.Dispose ( );
              }
            }
          }
        }
      }
    }
  }
}
