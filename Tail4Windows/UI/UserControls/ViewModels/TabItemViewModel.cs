using System;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI.UserControls.ViewModels
{
  /// <summary>
  /// TabItemViewModel
  /// </summary>
  public class TabItemViewModel
  {
    /// <summary>
    /// Header
    /// </summary>
    public string Header
    {
      get;
      set;
    }

    /// <summary>
    /// Name of TabItem
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand
    {
      get;
      set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="onClose">OnClose <see cref="Action"/></param>
    public TabItemViewModel()
    {

    }
  }
}
