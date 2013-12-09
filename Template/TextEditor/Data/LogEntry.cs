using System;


namespace TailForWin.Template.TextEditor.Data
{
  public class LogEntry: PropertyChangedBase
  {
    /// <summary>
    /// DateTime
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// Index for LineNumbers
    /// </summary>
    public int Index { get; set; }

    private string message;

    /// <summary>
    /// Logmessage
    /// </summary>
    public string Message 
    {
      get
      {
        return (message);
      }
      set
      {
        message = value;
        OnPropertyChanged ("Message");
      }
    }

    /// <summary>
    /// Create a copy of object
    /// </summary>
    /// <returns>A clone from object</returns>
    public LogEntry Clone ()
    {
      return (this.MemberwiseClone ( ) as LogEntry);
    }
  }
}
