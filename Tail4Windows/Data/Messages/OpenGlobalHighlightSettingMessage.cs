namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Opens GlobalHighlight setting message
  /// </summary>
  public class OpenGlobalHighlightSettingMessage
  {
    /// <summary>
    /// FilterPattern
    /// </summary>
    public string FilterPattern
    {
      get;
    }

    /// <summary>
    /// Who is sending the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who is sending the message</param>
    /// <param name="filterPattern"></param>
    public OpenGlobalHighlightSettingMessage(object sender, string filterPattern)
    {
      Sender = sender;

      if (!string.IsNullOrWhiteSpace(filterPattern))
        FilterPattern = filterPattern;
    }
  }
}
