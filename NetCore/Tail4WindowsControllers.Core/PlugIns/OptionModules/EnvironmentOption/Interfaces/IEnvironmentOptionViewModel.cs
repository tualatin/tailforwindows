﻿using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;

namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Environment option view model interface
  /// </summary>
  public interface IEnvironmentOptionViewModel : IOptionBaseViewModel
  {
    /// <summary>
    /// AddToSendTo command
    /// </summary>
    IAsyncCommand AddToSendToCommand
    {
      get;
    }

    /// <summary>
    /// Selection changed command
    /// </summary>
    ICommand SelectionChangedCommand
    {
      get;
    }

    /// <summary>
    /// SendTo button text
    /// </summary>
    string SendToButtonText
    {
      get; set;
    }
  }
}
