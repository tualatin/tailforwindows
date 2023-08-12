namespace Org.Vs.Tail4Win.Core.Enums
{
  /// <summary>
  /// Export format enum
  /// </summary>
  [Flags]
  public enum EExportFormat
  {
    /// <summary>
    /// CSV format
    /// </summary>
    Csv = 2,

    /// <summary>
    /// Excel format
    /// </summary>
    Excel = 4,

    /// <summary>
    /// Open document format
    /// </summary>
    OpenDocument = 8,

    /// <summary>
    /// None
    /// </summary>
    None = 16
  }
}
