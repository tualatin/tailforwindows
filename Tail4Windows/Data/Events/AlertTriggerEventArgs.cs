using System;


namespace Org.Vs.TailForWin.Data.Events
{
  /// <summary>
  /// AlertTriggerEventArgs
  /// </summary>
  public class AlertTriggerEventArgs : EventArgs
  {
    private readonly Template.TextEditor.Data.LogEntry alertTriggerData;


    /// <summary>
    /// Set LogEntry data
    /// </summary>
    /// <param name="obj">LogEntry data</param>
    public AlertTriggerEventArgs(Template.TextEditor.Data.LogEntry obj)
    {
      alertTriggerData = obj;
    }

    /// <summary>
    /// Get LogEntry object
    /// </summary>
    /// <returns>LogEntry object</returns>
    public Template.TextEditor.Data.LogEntry GetData()
    {
      return alertTriggerData;
    }
  }
}
