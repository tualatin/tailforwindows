using System;
using System.Runtime.InteropServices;
using System.Text;
using TailForWin.Data;
using System.IO;


namespace TailForWin.Utils
{
  public static class SoundPlay
  {
    [DllImport ("winmm.dll")]
    private static extern int mciSendString (string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hWndCallback);

    private static string command = string.Empty;
    // private static StringBuilder msg = new StringBuilder ((int) 256);
    private static readonly StringBuilder ReturnData = new StringBuilder (256);
    private static int result = -1;
    private static string mciMusicFile = string.Empty;


    /// <summary>
    /// Initialize sound engine
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    public static bool InitSoundPlay (string soundFile) 
    {
      if (String.Compare(soundFile, LogFile.ALERT_SOUND_FILENAME, StringComparison.Ordinal) == 0)
        return (false);
      if (!File.Exists (soundFile))
        return (false);

      Close ( );

      mciMusicFile = soundFile;
      command = string.Format ("open \"{0}\" type mpegvideo alias MediaFile", soundFile);
      result = mciSendString (command, null, 0, IntPtr.Zero);

      if (result == 0)
        return (true);

      command = string.Format ("open \"{0}\" alias MediaFile", soundFile);
      result = mciSendString (command, null, 0, IntPtr.Zero);

      return (result == 0);
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

      Close ( );

      return (false);
    }

    public static void Close ( )
    {
      command = "close MediaFile";
      mciSendString (command, null, 0, IntPtr.Zero);
    }

    public static bool IsPlaying ()
    {
      command = "status MediaFile mode";
      result = mciSendString (command, ReturnData, 128, IntPtr.Zero);

      if (ReturnData.Length == 7 && String.CompareOrdinal(ReturnData.ToString ( ).Substring (0, 7), "playing") == 0)
        return (true);

      InitSoundPlay (mciMusicFile);

      return (false);
    }

    private static bool IsOpen ()
    {
      command = "status MediaFile mode";
      result = mciSendString (command, ReturnData, 128, IntPtr.Zero);

      return (ReturnData.Length == 4 && String.CompareOrdinal(ReturnData.ToString ( ).Substring (0, 4), "open") == 0);
    }

    public static bool IsStopped ()
    {
      command = "status MediaFile mode";
      result = mciSendString (command, ReturnData, 128, IntPtr.Zero);

      return (ReturnData.Length == 7 && String.CompareOrdinal(ReturnData.ToString ( ).Substring (0, 7), "stopped") == 0);
    }
  }
}
