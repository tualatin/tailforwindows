namespace Org.Vs.TailForWin.Template.TextEditor.Data
{
  public static class TemplateData
  {
    /// <summary>
    /// Logview UI state
    /// </summary>
    public static TemplateStates State
    {
      get;
      set;
    }

    /// <summary>
    /// Logview UI possibilities
    /// </summary>
    public enum TemplateStates
    {
      /// <summary>
      /// Show datetime and logmessage
      /// </summary>
      ShowDateTime,

      /// <summary>
      /// Show datetime, linenumbers and logmessage
      /// </summary>
      ShowDateTimeLineNumber,

      /// <summary>
      /// Show linenumbers and logmessage
      /// </summary>
      ShowLineNumber,

      /// <summary>
      /// Show logmessage only
      /// </summary>
      ShowDefault
    }
  }
}
