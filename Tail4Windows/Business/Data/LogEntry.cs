using System;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Business.Data
{
  /// <summary>
  /// LogEntry
  /// </summary>
  public class LogEntry : NotifyMaster
  {
    private int _index;

    /// <summary>
    /// Current index
    /// </summary>
    public int Index
    {
      get => _index;
      set
      {
        if ( value == _index )
          return;

        _index = value;
        OnPropertyChanged();
      }
    }

    private DateTime _dateTime;

    /// <summary>
    /// DateTime
    /// </summary>
    public DateTime DateTime
    {
      get => _dateTime;
      set
      {
        if ( Equals(value, _dateTime) )
          return;

        _dateTime = value;
        OnPropertyChanged();
      }
    }

    private string _message;

    /// <summary>
    /// Message
    /// </summary>
    public string Message
    {
      get => _message;
      set
      {
        if ( Equals(value, _message) )
          return;

        _message = value;
        OnPropertyChanged();
      }
    }

    private System.Windows.Media.ImageSource _bookmarkPoint;

    /// <summary>
    /// Bookmark image when set a bookmark to a line
    /// </summary>
    public System.Windows.Media.ImageSource BookmarkPoint
    {
      get => _bookmarkPoint;
      set
      {
        _bookmarkPoint = value;
        OnPropertyChanged();
      }
    }

    private bool _isCacheData;

    /// <summary>
    /// This <see cref="LogEntry"/> is cache data
    /// </summary>
    public bool IsCacheData
    {
      get => _isCacheData;
      set
      {
        if ( value == _isCacheData )
          return;

        _isCacheData = value;
        OnPropertyChanged();
      }
    }
  }
}
