using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Collections;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;

namespace Org.Vs.TailForWin.Core.Data
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
