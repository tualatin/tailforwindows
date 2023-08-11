namespace Org.Vs.Tail4Win.Shared.Interfaces
{
  /// <summary>
  /// Plays sound file interface
  /// </summary>
  public interface IPlaySoundFile
  {
    /// <summary>
    /// Initialize sound engine
    /// </summary>
    /// <param name="soundFile">Soundfile</param>
    /// <returns>If media file exists, returns true otherwise false</returns>
    bool InitSoundPlay(string soundFile);

    /// <summary>
    /// Play selected media file
    /// </summary>
    /// <param name="loop">Should loop</param>
    /// <returns>If media file is playing, returns false, otherwise true</returns>
    bool Play(bool loop);

    /// <summary>
    /// Close media file
    /// </summary>
    void Close();

    /// <summary>
    /// If media file is playing at the moment
    /// </summary>
    /// <returns>If it is playing, return true, otherwise false</returns>
    bool IsPlaying();

    /// <summary>
    /// Is media file currently open
    /// </summary>
    /// <returns>If not open, returns false otherwise true</returns>
    bool IsOpen();

    /// <summary>
    /// Playing ist stopped
    /// </summary>
    /// <returns>If it is stopped <c>True</c> otherwise <c>False</c></returns>
    bool IsStopped();
  }
}
