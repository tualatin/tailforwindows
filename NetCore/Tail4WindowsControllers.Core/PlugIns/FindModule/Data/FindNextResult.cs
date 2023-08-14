namespace Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Data
{
  /// <summary>
  /// FindNext result
  /// </summary>
  public class FindNextResult
  {
    /// <summary>
    /// Result
    /// </summary>
    public bool Result
    {
      get;
    }

    /// <summary>
    /// End index
    /// </summary>
    public double EndIndex
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="result">Result of search</param>
    /// <param name="endIndex">Last index</param>
    public FindNextResult(bool result, double endIndex)
    {
      Result = result;
      EndIndex = endIndex;
    }
  }
}
