using System;
using System.IO;
using System.Text;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Native;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// This class is responsible to play sounds in Org.Vs.TailForWindows
  /// </summary>
  public static class SoundPlay
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SoundPlay));


    private static string command = string.Empty;
    // private static StringBuilder msg = new StringBuilder ((int) 256);
    private static readonly StringBuilder ReturnData = new StringBuilder(256);
    private static int result = -1;
    private static string mciMusicFile = string.Empty;


    /// <summary>
    /// Initialize sound engine
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    /// <returns>If media file exists, returns true otherwise false</returns>
    public static bool InitSoundPlay(string soundFile)
    {
      if(string.Compare(soundFile, LogFile.ALERT_SOUND_FILENAME, StringComparison.Ordinal) == 0)
        return false;
      if(!File.Exists(soundFile))
        return false;

      try
      {
        Close();

        mciMusicFile = soundFile;
        command = $"open \"{soundFile}\" type mpegvideo alias MediaFile";
        result = NativeMethods.MciSendString(command, null, 0, IntPtr.Zero);

        if(result == 0)
          return true;

        command = $"open \"{soundFile}\" alias MediaFile";
        result = NativeMethods.MciSendString(command, null, 0, IntPtr.Zero);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return result == 0;
    }

    /// <summary>
    /// Play selected media file
    /// </summary>
    /// <param name="loop">Should loop</param>
    /// <returns>If media file is playing, returns false, otherwise true</returns>
    public static bool Play(bool loop)
    {
      if(IsPlaying())
        return false;

      command = "play MediaFile";

      if(loop)
        command += " REPEAT";

      try
      {
        result = NativeMethods.MciSendString(command, null, 0, IntPtr.Zero);

        if(result == 0)
          return true;

        Close();
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Close media file
    /// </summary>
    public static void Close()
    {
      command = "close MediaFile";
      NativeMethods.MciSendString(command, null, 0, IntPtr.Zero);
    }

    /// <summary>
    /// If media file is playing at the moment
    /// </summary>
    /// <returns>If it is playing, return true, otherwise false</returns>
    public static bool IsPlaying()
    {
      command = "status MediaFile mode";

      try
      {
        result = NativeMethods.MciSendString(command, ReturnData, 128, IntPtr.Zero);

        if(ReturnData.Length == 7 && string.CompareOrdinal(ReturnData.ToString().Substring(0, 7), "playing") == 0)
          return true;

        InitSoundPlay(mciMusicFile);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Is media file currently open
    /// </summary>
    /// <returns>If not open, returns false otherwise true</returns>
    private static bool IsOpen()
    {
      command = "status MediaFile mode";

      try
      {
        result = NativeMethods.MciSendString(command, ReturnData, 128, IntPtr.Zero);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return ReturnData.Length == 4 && string.CompareOrdinal(ReturnData.ToString().Substring(0, 4), "open") == 0;
    }

    public static bool IsStopped()
    {
      command = "status MediaFile mode";

      try
      {
        result = NativeMethods.MciSendString(command, ReturnData, 128, IntPtr.Zero);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return ReturnData.Length == 7 && string.CompareOrdinal(ReturnData.ToString().Substring(0, 7), "stopped") == 0;
    }
  }
}
