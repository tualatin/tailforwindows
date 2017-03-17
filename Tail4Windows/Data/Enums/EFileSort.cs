using System.ComponentModel;


namespace Org.Vs.TailForWin.Data.Enums
{
  /// <summary>
  /// Enum File sort
  /// </summary>
  public enum EFileSort
  {
    /// <summary>
    /// File creation time
    /// </summary>
    [Description("File creation time")]
    FileCreationTime,

    /// <summary>
    /// No file sort
    /// </summary>
    [Description("Default")]
    Nothing
  }
}
