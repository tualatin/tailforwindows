using Org.Vs.TailForWin.Business.Utils;


namespace Org.Vs.TailForWin.Business.Data
{
  /// <summary>
  /// Business environment
  /// </summary>
  public class BusinessEnvironment
  {
    /// <summary>
    /// Tail4Window database file
    /// </summary>
    public static string TailForWindowsDatabaseFile = EnvironmentContainer.UserSettingsPath + @"\T4W.db";
  }
}
