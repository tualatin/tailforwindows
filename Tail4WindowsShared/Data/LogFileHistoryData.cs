using Newtonsoft.Json;
using Org.Vs.Tail4Win.Shared.Collections;
using Org.Vs.Tail4Win.Shared.Controllers;
using Org.Vs.Tail4Win.Shared.Data.Base;

namespace Org.Vs.Tail4Win.Shared.Data
{
  /// <summary>
  /// Log file history data
  /// </summary>
  public class LogFileHistoryData : NotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogFileHistoryData() => FindCollection = new QueueSet<string>(SettingsHelperController.CurrentSettings.HistoryMaxSize);

    private QueueSet<string> _findCollection;

    /// <summary>
    /// Collection of history
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "LogFileHistory")]
    public QueueSet<string> FindCollection
    {
      get => _findCollection;
      set
      {
        if ( _findCollection == value )
          return;

        _findCollection = value;
        OnPropertyChanged();
      }
    }
  }
}
