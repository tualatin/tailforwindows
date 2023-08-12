using Org.Vs.Tail4Win.Business.Services.Data;
using Org.Vs.Tail4Win.Core.Data;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.LogWindowModule.Data
{
  /// <summary>
  /// MessageFloodData;
  /// </summary>
  public class MessageFloodData
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public MessageFloodData() => Results = new List<string>();

    /// <summary>
    /// <see cref="LogEntry"/>
    /// </summary>
    public LogEntry LogEntry
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="List{T}"/> of results
    /// </summary>
    public List<string> Results
    {
      get;
      set;
    }

    /// <summary>
    /// Filter of type <see cref="FilterData"/>
    /// </summary>
    public FilterData Filter
    {
      get;
      set;
    }
  }
}
