﻿using System;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.BaseView.StyleableWindow
{
  /// <summary>
  /// Opens the settings dialog
  /// </summary>
  public class OpenSettingsDialogBehavior : ICommand
  {
    /// <summary>
    /// Can execute changed event handler
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>Always <c>True</c></returns>
    public bool CanExecute(object parameter) => true;

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    public void Execute(object parameter) => EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenSettingsDialogMessage(true));
  }
}