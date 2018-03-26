using System;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.UI.UserControls.Commands
{
  /// <summary>
  /// Adds a new <see cref="TabItem"/> to <see cref="DragSupportTabControl"/>
  /// </summary>
  public class AddNewTabItemCommand : ICommand
  {
    /// <summary>
    /// Can execute changed event handler
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Always true</returns>
    public bool CanExecute(object parameter) => true;

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    public void Execute(object parameter)
    {
      if ( !(parameter is Button) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new AddNewTabItemMessage(this));
    }
  }
}
