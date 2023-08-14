namespace Org.Vs.TailForWin.Business.Utils.Interfaces
{
  /// <summary>
  /// Profile converter interface
  /// </summary>
  public interface IProfileConverter
  {
    /// <summary>
    /// Converts a roaming profile into local profile
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    Task ConvertIntoLocalProfileAsync();

    /// <summary>
    /// Converts a local profile into roaming profile
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    Task ConvertIntoRoamingProfileAsync();
  }
}
