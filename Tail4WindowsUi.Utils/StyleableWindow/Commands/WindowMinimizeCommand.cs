﻿using System;
using System.Windows;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Ui.Utils.StyleableWindow.Commands
{
  /// <summary>
  /// Window minimize command
  /// </summary>
  public class WindowMinimizeCommand : ICommand
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
      if ( parameter is Window window )
        window.WindowState = WindowState.Minimized;
    }
  }
}
