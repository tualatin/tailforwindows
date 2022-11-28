namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Global filter highlight message
  /// </summary>
  public class OpenGlobalHightlightFromTailDataMessage
  {
    /// <summary>
    /// Filter pattern
    /// </summary>
    public string FilterPattern
    {
      get;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="filterPattern">Filter pattern</param>
    public OpenGlobalHightlightFromTailDataMessage(string filterPattern) => FilterPattern = filterPattern;
  }
}
