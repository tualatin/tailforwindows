using Org.Vs.TailForWin.Business.Utils;


namespace Org.Vs.TailForWin.Business.Data
{
  /// <summary>
  /// Business environment
  /// </summary>
  public static class BusinessEnvironment
  {
    /// <summary>
    /// Tail4Window database file
    /// </summary>
    public static readonly string TailForWindowsDatabaseFile = EnvironmentContainer.UserSettingsPath + @"\T4W.db";
  }
}
