using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TailForWin.Data;
using System.IO;


namespace TailForWin.Utils
{
  // TODO sound engine
  public static class SoundPlay
  {
    [DllImport ("winmm.dll")]
    private static extern int mciSendString (string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hWndCallback);

    private static string command = string.Empty;
    private static StringBuilder msg = new StringBuilder ((int) 256);
    private static StringBuilder returnData = new StringBuilder ((int) 256);
    private static int result = -1;
    private static string mciMusicFile = string.Empty;


    /// <summary>
    /// Initialize sound engine
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    public static bool InitSoundPlay (string soundFile) 
    {
      if (soundFile.CompareTo (LogFile.ALERT_SOUND_FILENAME) == 0)
        return (false);
      if (!File.Exists (soundFile))
        return (false);

      Close ( );

      mciMusicFile = soundFile;
      command = string.Format ("open \"{0}\" type mpegvideo alias MediaFile", soundFile);
      result = mciSendString (command, null, 0, IntPtr.Zero);

      if (result != 0)
      {
        command = string.Format ("open \"{0}\" alias MediaFile", soundFile);
        result = mciSendString (command, null, 0, IntPtr.Zero);

        if (result == 0)
          return (true);
        else
          return (false);
      }
      else
        return (true);
    }

    /// <summary>
    /// Play sound
    /// </summary>
    public static bool Play (bool loop)
    {
      if (IsPlaying ( ))
        return (false);

      command = "play MediaFile";

      if (loop)
       command += " REPEAT";

      result = mciSendString (command, null, 0, IntPtr.Zero);

      if (result == 0)
        return (true);
      else
      {
        Close ( );

        return (false);
      }
    }

    public static void Close ( )
    {
      command = "close MediaFile";
      mciSendString (command, null, 0, IntPtr.Zero);
    }

    public static bool IsPlaying ()
    {
      command = "status MediaFile mode";
      result = mciSendString (command, returnData, 128, IntPtr.Zero);

      if (returnData.Length == 7 && String.Compare (returnData.ToString ( ).Substring (0, 7), "playing", false) == 0)
        return (true);
      else
      {
        InitSoundPlay (mciMusicFile);

        return (false);
      }
    }

    private static bool IsOpen ()
    {
      command = "status MediaFile mode";
      result = mciSendString (command, returnData, 128, IntPtr.Zero);

      if (returnData.Length == 4 && String.Compare (returnData.ToString ( ).Substring (0, 4), "open", false) == 0)
        return (true);
      else
        return (false);
    }

    public static bool IsStopped ()
    {
      command = "status MediaFile mode";
      result = mciSendString (command, returnData, 128, IntPtr.Zero);

      if (returnData.Length == 7 && String.Compare (returnData.ToString ( ).Substring (0, 7), "stopped", false) == 0)
        return (true);
      else
        return (false);
    }
  }
}
