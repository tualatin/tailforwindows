using System;


namespace Org.Vs.TailForWin.Template.TextEditor.Data
{
  /// <summary>
  /// LogEntry object
  /// </summary>
  public class LogEntry : PropertyChangedBase
  {
    /// <summary>
    /// DateTime
    /// </summary>
    public DateTime DateTime
    {
      get;
      set;
    }

    /// <summary>
    /// Index for LineNumbers
    /// </summary>
    public int Index
    {
      get;
      set;
    }

    private System.Windows.Media.ImageSource bookmarkPoint;

    /// <summary>
    /// Bookmark image when set a bookmark to a line
    /// </summary>
    public System.Windows.Media.ImageSource BookmarkPoint
    {
      get
      {
        return bookmarkPoint;
      }
      set
      {
        bookmarkPoint = value;
        OnPropertyChanged("BookmarkPoint");
      }
    }

    private string message;

    /// <summary>
    /// Log message
    /// </summary>
    public string Message
    {
      get
      {
        return message;
      }
      set
      {
        message = value;
        OnPropertyChanged("Message");
      }
    }

    /// <summary>
    /// Create a copy of object
    /// </summary>
    /// <returns>A clone from object</returns>
    public LogEntry Clone()
    {
      return this.MemberwiseClone() as LogEntry;
    }
  }
}
