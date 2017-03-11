using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;


namespace Org.Vs.TailForWin.Logging
{
  /// <summary>
  /// A converter which uses the first letter of the logging level, capitalized to represent the log-level.
  /// </summary>
  public class SingleLetterLevelConverter : PatternLayoutConverter
  {
    /// <summary>
    /// Convert first letter of logging level
    /// </summary>
    /// <param name="writer">TextWriter</param>
    /// <param name="loggingEvent">LoggingEvent</param>
    protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
    {
      // use first letter of the logging level, capitalized to represent the log-level
      writer.Write(loggingEvent.Level.DisplayName.Substring(0, 1).ToUpper());
    }
  }
}