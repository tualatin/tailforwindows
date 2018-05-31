﻿using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.FindModule.Interfaces
{
  /// <summary>
  /// FindDialog view model interface
  /// </summary>
  public interface IFindDialogViewModel
  {
    /// <summary>
    /// Top position
    /// </summary>
    double TopPosition
    {
      get;
    }

    /// <summary>
    /// Left position
    /// </summary>
    double LeftPosition
    {
      get;
    }

    /// <summary>
    /// SearchField has focus
    /// </summary>
    bool SearchFieldHasFocus
    {
      get;
    }

    /// <summary>
    /// Find settings
    /// </summary>
    FindData FindSettings
    {
      get;
    }

    /// <summary>
    /// Count current matches
    /// </summary>
    string CountMatches
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
    /// Loaded command
    /// </summary>
    IAsyncCommand LoadedCommand
    {
      get;
    }

    /// <summary>
    /// Closing command
    /// </summary>
    ICommand ClosingCommand
    {
      get;
    }

    /// <summary>
    /// Search history
    /// </summary>
    IObservableDictionary<string, string> SearchHistory
    {
      get;
    }
  }
}
