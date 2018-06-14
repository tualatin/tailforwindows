using System;


namespace Org.Vs.TailForWin.Data.Messages.FindWhat
{
  /// <summary>
  /// Open FindWhat window message
  /// </summary>
  public class OpenFindWhatWindowMessage
  {
    /// <summary>
    /// Who sends the message
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public object Sender
    {
      // ReSharper disable once UnusedAutoPropertyAccessor.Global
      get;
    }

    /// <summary>
    /// Title message
    /// </summary>
    public string Title
    {
      get;
    }

    /// <summary>
    /// FindWhat word
    /// </summary>
    public string FindWhat
    {
      get;
    }

    /// <summary>
    /// Which window calls the FindWhat dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="title">Title message</param>
    /// <param name="windowGuid">Which window calls the FindWhat dialog</param>
    /// <param name="findWhat">Find word</param>
    public OpenFindWhatWindowMessage(object sender, string title, Guid windowGuid, string findWhat = null)
    {
      Sender = sender;
      Title = title;
      WindowGuid = windowGuid;

      if ( !string.IsNullOrWhiteSpace(findWhat) )
        FindWhat = findWhat;
    }
  }
}
