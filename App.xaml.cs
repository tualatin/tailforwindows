using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Utils;
using System;
using System.Text.RegularExpressions;
using System.Windows;


namespace Org.Vs.TailForWin
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private void Application_Startup(object sender, StartupEventArgs e)
    {
#if DEBUG
      ErrorLog.StartLog();
#endif

      MainWindow wnd = new MainWindow();
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      wnd.Show();

      if (e.Args.Length <= 0)
        return;

      foreach (var arg in e.Args)
      {
        Match m = Regex.Match(arg, @"/id=");

        if (m.Success)
        {
          Match id = Regex.Match(arg, @"\d+");

          if (!id.Success)
            continue;

          FileManagerStructure fm = new FileManagerStructure();
          FileManagerData item = fm.GetNodeById(id.Value);

          if (item == null)
            continue;

          FileManagerDataEventArgs args = new FileManagerDataEventArgs(item);
          wnd.FileManagerTab(this, args);
          args.Dispose();
        }
        else
        {
          Regex regex = new Regex(@"(?:(?:(?:\b[a-z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)\\|\\?[^\\/:*?""<>|\r\n]+\\?)(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

          Match result = regex.Match(arg);

          if (result.Success)
            wnd.OpenFileFromParameter(arg);
        }
      }

      //foreach (FileManagerDataEventArgs args in from arg in e.Args
      //                                          let match = Regex.Match (arg, @"/id=")
      //                                          where match.Success
      //                                          select Regex.Match (arg, @"\d+") into id
      //                                          where id.Success
      //                                          let fm = new FileManagerStructure ( )
      //                                          select fm.GetNodeById (id.Value) into item
      //                                          where item != null
      //                                          select new FileManagerDataEventArgs (item))
      //{
      //  wnd.FileManagerTab (this, args);
      //  args.Dispose ( );
      //}
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      ErrorLog.WriteLog(ErrorFlags.Error, "TfW", string.Format("UnhandledException: {0}", e.ExceptionObject));
    }
  }
}
