using System;
using System.Windows.Input;
using System.Windows;


namespace Org.Vs.TailForWin.UI.StyleableWindow
{
  /// <summary>
  /// Window maximize command
  /// </summary>
  public class WindowMaximizeCommand : ICommand
  {
    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Always true</returns>
    public bool CanExecute(object parameter)
    {
      return (true);
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
      if(parameter is Window window)
      {
        if(window.WindowState == WindowState.Maximized)
          window.WindowState = WindowState.Normal;
        else
          window.WindowState = WindowState.Maximized;
      }
    }
  }
}
