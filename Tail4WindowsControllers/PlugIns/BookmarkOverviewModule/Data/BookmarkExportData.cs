namespace Org.Vs.TailForWin.Controllers.PlugIns.BookmarkOverviewModule.Data
{
  /// <summary>
  /// Bookmark export data
  /// </summary>
  public class BookmarkExportData
  {
    /// <summary>
    /// CSV export
    /// </summary>
    public bool CsvExport
    {
      get;
      set;
    }

    /// <summary>
    /// Excel export
    /// </summary>
    public bool ExcelExport
    {
      get;
      set;
    }

    /// <summary>
    /// OpenDocument export
    /// </summary>
    public bool OpenDocumentExport
    {
      get;
      set;
    }

    /// <summary>
    /// Export successful
    /// </summary>
    public bool Success
    {
      get;
      set;
    }
  }
}
