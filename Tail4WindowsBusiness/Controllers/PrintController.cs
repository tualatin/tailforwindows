using System.Collections.Generic;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.Controllers
{
  /// <summary>
  /// Print controller
  /// </summary>
  public class PrintController
  {
    private FlowDocument _flowDocument;

    /// <summary>
    /// Print current log window items
    /// </summary>
    /// <param name="logItems">List of <see cref="LogEntry"/></param>
    /// <param name="tailData">Current <see cref="TailData"/> settings</param>
    /// <returns>Task</returns>
    public void PrintDocument(IEnumerable<LogEntry> logItems, TailData tailData)
    {
      var printDialog = new PrintDialog
      {
        UserPageRangeEnabled = true,
        PageRangeSelection = PageRangeSelection.AllPages,
        PrintTicket = GetPrintTicketFromPrinter()
      };

      if ( !printDialog.ShowDialog().GetValueOrDefault() )
        return;

      _flowDocument = new FlowDocument
      {
        FontSize = 11f,
        FontFamily = new System.Windows.Media.FontFamily("Curier New"),
        PageHeight = printDialog.PrintableAreaHeight,
        PageWidth = printDialog.PrintableAreaWidth,
        PagePadding = new Thickness(80),
        ColumnGap = 0
      };

      MouseService.SetBusyState();

      foreach ( var item in logItems )
      {
        _flowDocument.Blocks.Add(!tailData.Timestamp
          ? new Paragraph(new Run($"{item.Index}\t{item.Message}"))
          : new Paragraph(new Run($"{item.Index}\t{item.DateTime.ToString(SettingsHelperController.CurrentSettings.CurrentStringFormat)} {item.Message}")));
      }

      _flowDocument.ColumnWidth = _flowDocument.PageWidth - _flowDocument.ColumnGap - _flowDocument.PagePadding.Left - _flowDocument.PagePadding.Right;

      var page = ((IDocumentPaginatorSource) _flowDocument).DocumentPaginator;
      printDialog.PrintDocument(page, $"{CoreEnvironment.ApplicationTitle} printing file {tailData.File}");
    }

    /// <summary>
    /// Returns a PrintTicket based on the current default printer.</summary>
    /// <returns>A PrintTicket for the current local default printer.</returns>
    private PrintTicket GetPrintTicketFromPrinter()
    {
      using ( var localPrintServer = new LocalPrintServer() )
      {
        PrintQueue printQueue;

        // Retrieving collection of local printer on user machine
        var localPrinterCollection = localPrintServer.GetPrintQueues();
        System.Collections.IEnumerator localPrinterEnumerator = localPrinterCollection.GetEnumerator();

        if ( localPrinterEnumerator.MoveNext() )
        {
          // Get PrintQueue from first available printer
          printQueue = (PrintQueue) localPrinterEnumerator.Current;
        }
        else
        {
          // No printer exist, return null PrintTicket
          return null;
        }

        // Get default PrintTicket from printer
        var printTicket = printQueue?.DefaultPrintTicket;
        var printCapabilities = printQueue?.GetPrintCapabilities();

        // Modify PrintTicket
        if ( printCapabilities != null && printCapabilities.CollationCapability.Contains(Collation.Collated) )
          printTicket.Collation = Collation.Collated;

        if ( printCapabilities != null && printCapabilities.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge) )
          printTicket.Duplexing = Duplexing.TwoSidedLongEdge;

        if ( printCapabilities != null && printCapabilities.StaplingCapability.Contains(Stapling.StapleDualLeft) )
          printTicket.Stapling = Stapling.StapleDualLeft;

        return printTicket;
      }
    }
  }
}
