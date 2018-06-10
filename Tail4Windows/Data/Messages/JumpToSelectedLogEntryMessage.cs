using System;
using Org.Vs.TailForWin.Business.Data;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Jump to selected <see cref="LogEntry"/> from FindWhatResult window message
  /// </summary>
  public class JumpToSelectedLogEntryMessage
  {
    /// <summary>
    /// Selected <see cref="LogEntry"/>
    /// </summary>
    public LogEntry SelectedLogEntry
    {
      get;
    }

    /// <summary>
    /// Which window calls the find dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the find dialog</param>
    /// <param name="logEntry">Selected <see cref="LogEntry"/></param>
    public JumpToSelectedLogEntryMessage(Guid windowGuid, LogEntry logEntry)
    {
      WindowGuid = windowGuid;
      SelectedLogEntry = logEntry;
    }
  }
}
