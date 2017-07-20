using System.Collections.Generic;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Org.Vs.TailForWin.Template.TextEditor.Data;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Controller
{
  /// <summary>
  /// PrintHelper
  /// </summary>
  public class PrintHelper
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="items">Items to print</param>
    /// <param name="fileName">Filename</param>
    /// <param name="timeStamp">With timestamp, default <code>false</code></param>
    /// <param name="format">Format identifier</param>
    public PrintHelper(IEnumerable<LogEntry> items, string fileName, bool timeStamp = false, string format = null)
    {
      PrintDialog printDialog = new PrintDialog
      {
        UserPageRangeEnabled = true,
        PageRangeSelection = PageRangeSelection.AllPages,
        PrintTicket = GetPrintTicketFromPrinter()
      };

      if ( !printDialog.ShowDialog().GetValueOrDefault() )
        return;

      FlowDocument flowDocument = new FlowDocument
      {
        FontSize = 11f,
        FontFamily = new System.Windows.Media.FontFamily("Tahoma"),
        PageHeight = printDialog.PrintableAreaHeight,
        PageWidth = printDialog.PrintableAreaWidth,
        PagePadding = new Thickness(80),
        ColumnGap = 0
      };

      foreach ( LogEntry item in items )
      {
        flowDocument.Blocks.Add(!timeStamp
          ? new Paragraph(new Run($"{item.Index}\t{item.Message}"))
          : new Paragraph(new Run($"{item.Index}\t{item.DateTime.ToString(format)} {item.Message}")));
      }

      flowDocument.ColumnWidth = flowDocument.PageWidth - flowDocument.ColumnGap - flowDocument.PagePadding.Left - flowDocument.PagePadding.Right;

      DocumentPaginator page = ((IDocumentPaginatorSource) flowDocument).DocumentPaginator;
      printDialog.PrintDocument(page, $"{CentralManager.APPLICATION_CAPTION} printing file {fileName}");
    }

    /// <summary>
    /// Returns a PrintTicket based on the current default printer.</summary>
    /// <returns>A PrintTicket for the current local default printer.</returns>
    private static PrintTicket GetPrintTicketFromPrinter()
    {
      PrintQueue printQueue;
      LocalPrintServer localPrintServer = new LocalPrintServer();

      // Retrieving collection of local printer on user machine
      PrintQueueCollection localPrinterCollection = localPrintServer.GetPrintQueues();
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
      PrintTicket printTicket = printQueue.DefaultPrintTicket;
      PrintCapabilities printCapabilites = printQueue.GetPrintCapabilities();

      // Modify PrintTicket
      if ( printCapabilites.CollationCapability.Contains(Collation.Collated) )
        printTicket.Collation = Collation.Collated;

      if ( printCapabilites.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge) )
        printTicket.Duplexing = Duplexing.TwoSidedLongEdge;

      if ( printCapabilites.StaplingCapability.Contains(Stapling.StapleDualLeft) )
        printTicket.Stapling = Stapling.StapleDualLeft;

      return printTicket;
    }
  }
}
