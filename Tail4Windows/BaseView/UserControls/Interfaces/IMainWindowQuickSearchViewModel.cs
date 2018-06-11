using System;
using System.Windows.Input;


namespace Org.Vs.TailForWin.BaseView.UserControls.Interfaces
{
  /// <summary>
  /// MainWindow quick search view model interface
  /// </summary>
  public interface IMainWindowQuickSearchViewModel
  {
    /// <summary>
    /// Has quick search textbox the focus
    /// </summary>
    bool IsFocused
    {
      get;
    }

    /// <summary>
    /// Search text
    /// </summary>
    string SearchText
    {
      get;
    }

    /// <summary>
    /// Window id
    /// </summary>
    Guid WindowGuid
    {
      get;
      set;
    }

    /// <summary>
    /// Quick search command
    /// </summary>
    ICommand QuickSearchCommand
    {
      get;
    }
  }
}
