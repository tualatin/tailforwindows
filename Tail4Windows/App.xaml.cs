using System;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.BaseView;
using Org.Vs.TailForWin.Core.Utils;


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
      // If it is not the minimum .NET version installed
      if ( EnvironmentContainer.NetFrameworkKey <= 393295 )
      {
        LOG.Error("Wrong .NET version! Please install .NET 4.6 or newer.");
        Shutdown();
        return;
      }

      T4Window wnd = new T4Window();
      AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
      wnd.Show();
    }

    private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) =>
      LOG.Error("{0} caused a(n) {1} {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, e.ExceptionObject.GetType().Name, e.ExceptionObject);
  }
}
