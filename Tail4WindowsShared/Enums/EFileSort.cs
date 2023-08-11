using System.ComponentModel;

namespace Org.Vs.Tail4Win.Shared.Enums
{
  /// <summary>
  /// Enum File sort
  /// </summary>
  public enum EFileSort
  {
    /// <summary>
    /// File creation time
    /// </summary>
    [Description("EFileSortFileCreationTime")]
    FileCreationTime,

    /// <summary>
    /// No file sort
    /// </summary>
    [Description("EFileSortDefault")]
    Nothing
  }
}
