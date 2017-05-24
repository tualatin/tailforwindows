using System;
using System.Windows.Input;
using System.Windows;


namespace Org.Vs.TailForWin.UI.StyleableWindow
{
  /// <summary>
  /// Window close command
  /// </summary>
  public class WindowCloseCommand : ICommand
  {
    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Always true</returns>
    public bool CanExecute(object parameter)
    {
      return true;
    }

    /// <summary>
    /// Can execute changed event handler
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Execute 
    /// </summary>
    /// <param name="parameter">Parameter</param>
    public void Execute(object parameter)
    {
      var window = parameter as Window;
      window?.Close();
    }
  }
}
