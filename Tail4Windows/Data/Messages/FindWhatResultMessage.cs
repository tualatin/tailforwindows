using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Business.Data;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// FindWhat result message
  /// </summary>
  public class FindWhatResultMessage
  {
    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/>
    /// </summary>
    public ObservableCollection<LogEntry> FindWhatResults
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="findWhatResults"><see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/></param>
    public FindWhatResultMessage(ObservableCollection<LogEntry> findWhatResults) => FindWhatResults = findWhatResults;
  }
}
