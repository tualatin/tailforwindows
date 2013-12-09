using System.Windows;
using System.Text.RegularExpressions;
using TailForWin.Controller;
using TailForWin.Data;
using System;
using TailForWin.Utils;


namespace TailForWin
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup (object sender, StartupEventArgs e)
    {
#if DEBUG
      ErrorLog.StartLog ( );
#endif

      MainWindow wnd = new MainWindow ( );
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
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

    private void CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)
    {
      ErrorLog.WriteLog (ErrorFlags.Error, "TfW", string.Format ("UnhandledException: {0}", e.ExceptionObject));
    }
  }
}
