namespace Org.Vs.TailForWin.UI.UserControls.Interfaces
{
  /// <summary>
  /// RegexHelper view model interface
  /// </summary>
  public interface IRegexHelper
  {
    /// <summary>
    /// Certain element hast focus
    /// </summary>
    bool ElementHasFocus
    {
      get;
      set;
    }

    /// <summary>
    /// Element text
    /// </summary>
    string ElementText
    {
      get;
      set;
    }

    /// <summary>
    /// SelectionIndex
    /// </summary>
    int CaretIndex
    {
      get;
      set;
    }
  }
}
