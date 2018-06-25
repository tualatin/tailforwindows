using System;
using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Business.Services.Data;


namespace Org.Vs.TailForWin.Data.Messages.FindWhat
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
    /// Which window calls the find dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="findWhatResults"><see cref="ObservableCollection{T}"/> of <see cref="LogEntry"/></param>
    /// <param name="windowGuid">Which window calls the find dialog</param>
    public FindWhatResultMessage(ObservableCollection<LogEntry> findWhatResults, Guid windowGuid)
    {
      FindWhatResults = findWhatResults;
      WindowGuid = windowGuid;
    }
  }
}
