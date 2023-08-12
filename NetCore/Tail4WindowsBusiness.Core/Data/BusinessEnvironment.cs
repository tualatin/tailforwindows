using Org.Vs.Tail4Win.Core.Utils;

namespace Org.Vs.Tail4Win.Business.Data
{
  /// <summary>
  /// Business environment
  /// </summary>
  public static class BusinessEnvironment
  {
    /// <summary>
    /// Tail4Window database file
    /// </summary>
    public static readonly string TailForWindowsDatabaseFile = $"Filename={CoreEnvironment.UserSettingsPath}\\T4W.db; Upgrade={true}";
  }
}
