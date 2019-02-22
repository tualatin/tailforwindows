using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// History data
  /// </summary>
  public class HistoryData : NotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public HistoryData() => FindCollection = new AsyncObservableCollection<string>();

    private bool _wrap;

    /// <summary>
    /// Wrap at the end of search
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Wrap
    {
      get => _wrap;
      set
      {
        if ( _wrap == value )
          return;

        _wrap = value;
        OnPropertyChanged();
      }
    }

    private AsyncObservableCollection<string> _findCollection;

    /// <summary>
    /// Collection of history
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "History")]
    public AsyncObservableCollection<string> FindCollection
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
