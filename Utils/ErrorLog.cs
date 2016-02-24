using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TailForWin.Data.Enums;


namespace TailForWin.Utils
{

  /// <summary>
  /// Error log class
  /// </summary>
  public static class ErrorLog
  {
    private static readonly List<string> AllMessages = new List<string> ( );
    private static DateTime now;
    private static StreamWriter sw;
    private static string logFileName = string.Empty;

    /// <summary>
    /// Path of logfile
    /// </summary>
    public static string LogFileName
    {
      get
      {
        return (logFileName);
      }
    }

    /// <summary>
    /// Create new log file
    /// </summary>
    public static void StartLog ()
    {
      logFileName = string.Format ("{0}\\{1}_error.log", Path.GetDirectoryName (System.Reflection.Assembly.GetEntryAssembly ( ).Location), Environment.MachineName);

      try
      {
        if (File.Exists (logFileName))
          File.Delete (logFileName);
        if (sw == null)
          sw = new StreamWriter (logFileName, true, Encoding.UTF8);
      }
      catch (Exception ex)
      {
        Console.WriteLine (@"{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod ( ).Name);
      }
    }

    /// <summary>
    /// Write message to log file
    /// </summary>
    /// <param name="flag">Error flag</param>
    /// <param name="source">Source</param>
    /// <param name="msg">Message</param>
    /// <param name="args">Arguments</param>
    public static void WriteLog (ErrorFlags flag, string source, string msg, params object[] args)
    {
      msg = string.Format (msg, args);
      WriteLog (flag, source, msg);
    }

    /// <summary>
    /// Write message to log file
    /// </summary>
    /// <param name="flag">Error flag</param>
    /// <param name="source">Source</param>
    /// <param name="msg">Message</param>
    public static void WriteLog (ErrorFlags flag, string source, string msg)
    {
      if (sw == null)
        StartLog ( );

      if (sw == null)
        return;

      try
      {
        now = GetSystemTime ( );

        AllMessages.Add (string.Format ("{0},{1} > |{2} |{3,-30} |{4}", now.ToString ("dd MMM yyyy HH:mm:ss", CultureInfo.CurrentCulture), now.Millisecond.ToString ("D3"), (char) flag, source, msg));
        Flush ( );
      }
      catch (Exception ex)
      {
        Console.WriteLine (@"{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod ( ).Name);
      }
    }

    /// <summary>
    /// Stop write any logs
    /// </summary>
    public static void StopLog ()
    {
      if (sw == null)
        return;

      Flush ( );

      sw.Close ( );
      sw = null;
    }

    /// <summary>
    /// Get system time now
    /// </summary>
    /// <returns>DateTime</returns>
    public static DateTime GetSystemTime ()
    {
      return (DateTime.Now);
    }

    private static void Flush ()
    {
      if (ReferenceEquals (AllMessages, null))
        return;

      AllMessages.ForEach (sw.WriteLine);

      sw.Flush ( );

      AllMessages.Clear ( );
    }
  }
}
