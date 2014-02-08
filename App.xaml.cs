﻿using System.Linq;
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
  public partial class App
  {
    private void Application_Startup (object sender, StartupEventArgs e)
    {
#if DEBUG
      ErrorLog.StartLog ( );
#endif

      MainWindow wnd = new MainWindow ( );
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      wnd.Show ( );

      if (e.Args.Length <= 0) 
        return;

      foreach (FileManagerDataEventArgs args in from arg in e.Args
                                                let match = Regex.Match (arg, @"/id=")
                                                where match.Success
                                                select Regex.Match (arg, @"\d+") into id
                                                where id.Success
                                                let fm = new FileManagerStructure ( )
                                                select fm.GetNodeById (id.Value) into item
                                                where item != null
                                                select new FileManagerDataEventArgs (item))
      {
        wnd.FileManagerTab (this, args);
        args.Dispose ( );
      }
    }

    private static void CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)
    {
      ErrorLog.WriteLog (ErrorFlags.Error, "TfW", string.Format ("UnhandledException: {0}", e.ExceptionObject));
    }
  }
}
