using System;


namespace Org.Vs.TailForWin.Controllers.PlugIns.ExportFormatModule.Enums
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
