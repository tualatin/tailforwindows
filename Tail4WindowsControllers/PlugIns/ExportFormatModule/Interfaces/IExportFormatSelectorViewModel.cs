using System.Windows.Input;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.Controllers.PlugIns.ExportFormatModule.Interfaces
{
  /// <summary>
  /// Export format selector view model interface
  /// </summary>
  public interface IExportFormatSelectorViewModel
  {
    /// <summary>
    /// Selected export format
    /// </summary>
    EExportFormat SelectedExportFormat
    {
      get;
    }

    /// <summary>
    /// CSV export
    /// </summary>
    bool CsvExport
    {
      get;
    }

    /// <summary>
    /// Excel export
    /// </summary>
    bool ExcelExport
    {
      get;
    }

    /// <summary>
    /// OpenDocument export
    /// </summary>
    bool OpenDocumentExport
    {
      get;
    }

    /// <summary>
    /// Select format finished command
    /// </summary>
    ICommand SelectFormatFinishedCommand
    {
      get;
    }
  }
}
