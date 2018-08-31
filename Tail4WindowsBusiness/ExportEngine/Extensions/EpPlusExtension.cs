using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.Business.ExportEngine.Extensions
{
  /// <summary>
  /// EPPlus extension
  /// </summary>
  public static class EpPlusExtension
  {
    /// <summary>
    /// Converts an <see cref="ExcelPackage"/> to CSV format
    /// </summary>
    /// <param name="package"><see cref="ExcelPackage"/></param>
    /// <returns>A byte array</returns>
    public static byte[] ConvertToCsv(this ExcelPackage package)
    {
      ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
      int maxColumnNumber = worksheet.Dimension.End.Column;
      var currentRow = new List<string>(maxColumnNumber);
      int totalRowCount = worksheet.Dimension.End.Row;
      var currentRowNumber = 1;
      var memory = new MemoryStream();

      using ( var writer = new StreamWriter(memory, Encoding.ASCII) )
      {
        while ( currentRowNumber <= totalRowCount )
        {
          BuildRow(worksheet, currentRow, currentRowNumber, maxColumnNumber);
          WriteRecordToFile(currentRow, writer, currentRowNumber, totalRowCount);

          currentRow.Clear();
          currentRowNumber++;
        }
      }
      return memory.ToArray();
    }

    private static void WriteRecordToFile(List<string> record, StreamWriter sw, int rowNumber, int totalRowCount)
    {
      string commaDelimitedRecord = record.ToDelimitedString(",");

      if ( rowNumber == totalRowCount )
        sw.Write(commaDelimitedRecord);
      else
        sw.WriteLine(commaDelimitedRecord);
    }

    private static void BuildRow(ExcelWorksheet worksheet, ICollection<string> currentRow, int currentRowNumber, int maxColumnNumber)
    {
      for ( int i = 1; i <= maxColumnNumber; i++ )
      {
        var cell = worksheet.Cells[currentRowNumber, i];
        AddCellValue(cell == null ? string.Empty : GetCellText(cell), currentRow);
      }
    }

    private static void AddCellValue(string s, ICollection<string> record) => record.Add($"{'"'}{s}{'"'}");

    private static string GetCellText(ExcelRangeBase cell) => cell.Value?.ToString() ?? string.Empty;
  }
}
