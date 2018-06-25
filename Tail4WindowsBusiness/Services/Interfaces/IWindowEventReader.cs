using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Events.Delegates;


namespace Org.Vs.TailForWin.Business.Services.Interfaces
{
  /// <summary>
  /// Windows event reader interface
  /// </summary>
  public interface IWindowEventReader
  {
    #region Events

    /// <summary>
    /// A new <see cref="LogEntry"/> is created
    /// </summary>
    event LogEntryCreated OnLogEntryCreated;

    #endregion

    /// <summary>
    /// Current log line index
    /// </summary>
    int Index
    {
      get;
    }

    /// <summary>
    /// Init <see cref="WindowsEventReader"/>
    /// </summary>
    /// <param name="machine">Name of machine</param>
    void InitWindowsEventReader(string machine);

    /// <summary>
    /// Get <see cref="ObservableCollection{T}"/> of <see cref="WindowsEventCategory"/> with Windows events categories
    /// </summary>
    /// <returns>Task</returns>
    Task<ObservableCollection<WindowsEventCategory>> GetCategoriesAsync();

    /// <summary>
    /// Starts read Windows  events
    /// </summary>
    /// <param name="category">Category to read</param>
    /// <returns><see cref="List{T}"/> of <see cref="LogEntry"/> of last xxx entries</returns>
    List<LogEntry> StartReadWindowsEvents(string category);

    /// <summary>
    /// Stops reading Windows events
    /// </summary>
    void StopReadWindowsEvents();

    /// <summary>
    /// Reset current index
    /// </summary>
    void ResetIndex();

    /// <summary>
    /// Set current index to special value
    /// </summary>
    /// <param name="index">Index</param>
    void SetIndex(int index);
  }
}
