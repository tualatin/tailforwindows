using System;
using System.Text.RegularExpressions;
using System.Windows;
using log4net;


namespace Org.Vs.TailForWin
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(App));

    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
      UI.T4Window wnd = new UI.T4Window();
      AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
      wnd.Show();

      if ( e.Args.Length <= 0 )
        return;

      foreach ( var arg in e.Args )
      {
        Match m = Regex.Match(arg, "/id=");

        if ( m.Success )
        {
          string guid;

          try
          {
            guid = arg.Substring("/id=".Length);
          }
          catch
          {
            guid = string.Empty;
          }

          if ( string.IsNullOrEmpty(guid) )
            continue;

          Match id = Regex.Match(guid, "^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$");

          if ( !id.Success )
            continue;
        }
        else
        {
          Regex regex = new Regex(@"(?:(?:(?:\b[a-z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)\\|\\?[^\\/:*?""<>|\r\n]+\\?)(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
          Match result = regex.Match(arg);

          //if ( result.Success )
          //  wnd.OpenFileFromParameter(arg);
        }
      }
    }

    private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      LOG.Error("{0} caused a(n) {1} {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, e.ExceptionObject.GetType().Name, e.ExceptionObject);
    }
  }
}
