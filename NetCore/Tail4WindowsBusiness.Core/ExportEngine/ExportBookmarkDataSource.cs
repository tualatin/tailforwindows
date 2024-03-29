﻿using log4net;
using OfficeOpenXml;
using Org.Vs.TailForWin.Business.ExportEngine.Extensions;
using Org.Vs.TailForWin.Business.ExportEngine.Interfaces;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Logging;

namespace Org.Vs.TailForWin.Business.ExportEngine
{
  /// <summary>
  /// Export bookmark data source
  /// </summary>
  public class ExportBookmarkDataSource : IDataExport<LogEntry>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ExportBookmarkDataSource));

    private ExcelPackage _excel;
    private CancellationTokenSource _cts;

    /// <summary>
    /// Export data as CSV
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ExportAsCsvAsync(IList<LogEntry> data, string fileName)
    {
      var result = false;

      try
      {
        _excel = CreateDocument(data);
        var csv = _excel.ConvertToCsv();
        var fileInfo = new FileInfo(fileName);

        using ( var fs = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None) )
        {
          SetCancellationTokenSource();
          await fs.WriteAsync(csv, _cts.Token);
        }

        result = true;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return result;
    }

    /// <summary>
    /// Export data as Excel sheet
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ExportAsExcelAsync(IList<LogEntry> data, string fileName)
    {
      var result = false;

      try
      {
        SetCancellationTokenSource();
        await Task.Run(() =>
        {
          _excel = CreateDocument(data);
          var fileInfo = new FileInfo(fileName);
          _excel.SaveAs(fileInfo);

          result = true;
        }, _cts.Token);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return result;
    }

    /// <summary>
    /// Export data as OpenDocument
    /// </summary>
    /// <param name="data"><see cref="IList{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="fileName">Filename</param>
    /// <returns><see cref="Task"/> if success <c>True</c> otherwise <c>False</c></returns>
    public Task<bool> ExportAsOpenDocumentAsync(IList<LogEntry> data, string fileName) => throw new NotImplementedException();

    private ExcelPackage CreateDocument(IEnumerable<LogEntry> data)
    {
      var excel = new ExcelPackage();

      excel.Workbook.Worksheets.Add("Bookmarks");

      var bookmarkWorksheet = excel.Workbook.Worksheets["Bookmarks"];
      var headerRows = new List<string[]>
      {
        new[]
        {
          "Index",
          "Bookmark comment",
          "Log text"
        }
      };
      string headerRange = "A1:" + char.ConvertFromUtf32(headerRows[0].Length + 64) + "1";

      bookmarkWorksheet.Cells[headerRange].LoadFromArrays(headerRows);
      bookmarkWorksheet.Cells[headerRange].Style.Font.Bold = true;
      bookmarkWorksheet.Cells[headerRange].Style.Font.Size = 11;
      bookmarkWorksheet.Cells[2, 1].LoadFromArrays(CreateFlatList(data)).AutoFitColumns(10, 150);
      bookmarkWorksheet.View.FreezePanes(2, 1);

      excel.Compression = CompressionLevel.BestSpeed;
      return excel;
    }

    private IEnumerable<object[]> CreateFlatList(IEnumerable<LogEntry> data) =>
      data.Select(bookmark => new object[] { bookmark.Index, bookmark.BookmarkToolTip, !EnvironmentContainer.Instance.BookmarkManager.TimeStamp? bookmark.Message :
        $"{bookmark.DateTime.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat)} {bookmark.Message}"}).ToList();

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
