﻿using System;
using System.Windows;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI.StyleableWindow.Commands
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
    public bool CanExecute(object parameter) => true;

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
