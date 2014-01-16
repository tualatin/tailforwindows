using System;


namespace TailForWin.Data
{
  public class AlertTriggerEventArgs : EventArgs
  {
    private TailForWin.Template.TextEditor.Data.LogEntry alertTriggerData;


    /// <summary>
    /// Set LogEntry data
    /// </summary>
    /// <param name="obj">LogEntry data</param>
    public AlertTriggerEventArgs (TailForWin.Template.TextEditor.Data.LogEntry obj)
    {
      alertTriggerData = obj;
    }

    /// <summary>
    /// Get LogEntry object
    /// </summary>
    /// <returns>LogEntry object</returns>
    public TailForWin.Template.TextEditor.Data.LogEntry GetData ( )
    {
      return (alertTriggerData);
    }
  }
}
