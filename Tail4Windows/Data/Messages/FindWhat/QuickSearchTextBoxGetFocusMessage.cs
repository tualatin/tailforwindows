namespace Org.Vs.TailForWin.Data.Messages.FindWhat
{
  /// <summary>
  /// QuickSearch textbox get focus message
  /// </summary>
  public class QuickSearchTextBoxGetFocusMessage
  {
    /// <summary>
    /// Who sends the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Is focused
    /// </summary>
    public bool IsFocused
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="isFocused">IsFocused</param>
    public QuickSearchTextBoxGetFocusMessage(object sender, bool isFocused)
    {
      Sender = sender;
      IsFocused = isFocused;
    }
  }
}
