using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace TailForWin.Utils
{
  /// <summary>
  /// Error flags
  /// </summary>
  public enum ErrorFlags
  {
    /// <summary>
    /// Error
    /// </summary>
    Error = (byte) 'E',

    /// <summary>
    /// Info
    /// </summary>
    Info = (byte) 'I',

    /// <summary>
    /// Debug
    /// </summary>
    Debug = (byte) 'D'
  }

  /// <summary>
  /// Error log class
  /// </summary>
  public static class ErrorLog
  {
    private static List<string> allMessages = new List<string> ( );
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
      logFileName = Path.GetDirectoryName (System.Reflection.Assembly.GetEntryAssembly ( ).Location) + "\\error.log";

      try
      {
        if (File.Exists (logFileName))
          File.Delete (logFileName);
        if (sw == null)
          sw = new StreamWriter (logFileName, true, Encoding.UTF8);
      }
      catch (Exception ex)
      {
        Console.WriteLine (string.Format ("ErrorLog Exception: {0}", ex));
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

        allMessages.Add (string.Format ("{0},{1} > |{2} |{3,-30} |{4}", now.ToString ("T", CultureInfo.CurrentCulture), now.Millisecond.ToString ("D3"), (char) flag, source, msg));
        Flush ( );
      }
      catch (Exception ex)
      {
        Console.WriteLine (string.Format ("WriteLog Exception: {0}", ex));
      }
    }

    /// <summary>
    /// Stop write any logs
    /// </summary>
    public static void StopLog ()
    {
      if (sw != null)
      {
        Flush ( );

        sw.Close ( );
        sw = null;
      }
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
      if (object.ReferenceEquals (allMessages, null))
        return;

      for (int i = 0; i < allMessages.Count; i++)
        sw.WriteLine (allMessages[i]);

      sw.Flush ( );

      allMessages.Clear ( );
    }
  }
}
