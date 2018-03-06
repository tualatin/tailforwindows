namespace Org.Vs.TailForWin.UI.UserControls.Interfaces
{
  /// <summary>
  /// Update control interface
  /// </summary>
  public interface IUpdateControlViewModel
  {
    /// <summary>
    /// Current application version
    /// </summary>
    string ApplicationVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Current web version
    /// </summary>
    string WebVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Update hint string
    /// </summary>
    string UpdateHint
    {
      get;
      set;
    }
  }
}
