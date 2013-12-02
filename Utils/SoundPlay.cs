using System;
using System.Collections.Generic;
using System.Linq;
using TailForWin.Data;


namespace TailForWin.Utils
{
  // TODO sound engine
  public static class SoundPlay
  {
    private static System.Media.SoundPlayer player;


    /// <summary>
    /// Initialize sound engine
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    public static void InitSoundPlay (string soundFile) 
    {
      if (soundFile.CompareTo (LogFile.ALERT_SOUND_FILENAME) == 0)
        return;

      player = new System.Media.SoundPlayer ( )
      {
        SoundLocation = soundFile
      };
    }

    /// <summary>
    /// Play sound
    /// </summary>
    public static void Play ()
    {
      if (player != null)
        player.Play ( );
    }

    /// <summary>
    /// Update sound file when it is changed
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    public static void UpdateSoundFile (string soundFile)
    {
      if (player == null)
        InitSoundPlay (soundFile);
      else
        player.SoundLocation = soundFile;
    }
  }
}
