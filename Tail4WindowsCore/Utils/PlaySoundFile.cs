using System;
using System.IO;
using System.Text;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Native;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Plays a sound media file
  /// </summary>
  public class PlaySoundFile : IPlaySoundFile
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(PlaySoundFile));

    private string _command;
    private readonly StringBuilder _returnData;
    private int _result;
    private string _mciMusicFile;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public PlaySoundFile()
    {
      _command = string.Empty;
      _returnData = new StringBuilder(256);
      _result = -1;
      _mciMusicFile = string.Empty;
    }

    /// <summary>
    /// Initialize sound engine
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    /// <returns>If media file exists, returns true otherwise false</returns>
    public bool InitSoundPlay(string soundFile)
    {
      if ( string.Compare(soundFile, "NoFile", StringComparison.Ordinal) == 0 )
        return false;
      if ( !File.Exists(soundFile) )
        return false;

      try
      {
        Close();

        _mciMusicFile = soundFile;
        _command = $"open \"{soundFile}\" type mpegvideo alias MediaFile";
        _result = NativeMethods.MciSendString(_command, null, 0, IntPtr.Zero);

        if ( _result == 0 )
          return true;

        _command = $"open \"{soundFile}\" alias MediaFile";
        _result = NativeMethods.MciSendString(_command, null, 0, IntPtr.Zero);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return _result == 0;
    }

    /// <summary>
    /// Play selected media file
    /// </summary>
    /// <param name="loop">Should loop</param>
    /// <returns>If media file is playing, returns false, otherwise true</returns>
    public bool Play(bool loop)
    {
      if ( IsPlaying() )
        return false;

      _command = "play MediaFile";

      if ( loop )
        _command += " REPEAT";

      try
      {
        _result = NativeMethods.MciSendString(_command, null, 0, IntPtr.Zero);

        if ( _result == 0 )
          return true;

        Close();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Close media file
    /// </summary>
    public void Close()
    {
      _command = "close MediaFile";
      NativeMethods.MciSendString(_command, null, 0, IntPtr.Zero);
    }

    /// <summary>
    /// If media file is playing at the moment
    /// </summary>
    /// <returns>If it is playing, return true, otherwise false</returns>
    public bool IsPlaying()
    {
      _command = "status MediaFile mode";

      try
      {
        _result = NativeMethods.MciSendString(_command, _returnData, 128, IntPtr.Zero);

        if ( _returnData.Length == 7 && string.CompareOrdinal(_returnData.ToString().Substring(0, 7), "playing") == 0 )
          return true;

        InitSoundPlay(_mciMusicFile);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return false;
    }

    /// <summary>
    /// Is media file currently open
    /// </summary>
    /// <returns>If not open, returns false otherwise true</returns>
    public bool IsOpen()
    {
      _command = "status MediaFile mode";

      try
      {
        _result = NativeMethods.MciSendString(_command, _returnData, 128, IntPtr.Zero);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return _returnData.Length == 4 && string.CompareOrdinal(_returnData.ToString().Substring(0, 4), "open") == 0;
    }

    /// <summary>
    /// Playing ist stopped
    /// </summary>
    /// <returns>If it is stopped <c>True</c> otherwise <c>False</c></returns>
    public bool IsStopped()
    {
      _command = "status MediaFile mode";

      try
      {
        _result = NativeMethods.MciSendString(_command, _returnData, 128, IntPtr.Zero);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return _returnData.Length == 7 && string.CompareOrdinal(_returnData.ToString().Substring(0, 7), "stopped") == 0;
    }
  }
}
