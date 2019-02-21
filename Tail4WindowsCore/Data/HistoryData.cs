using System.Collections.Generic;
using Newtonsoft.Json;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// History data
  /// </summary>
  public class HistoryData
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public HistoryData() => FindCollection = new List<string>();

    /// <summary>
    /// Wrap at the end of search
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Collection of history
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "History")]
    public List<string> FindCollection
    {
      get;
      set;
    }
  }
}
