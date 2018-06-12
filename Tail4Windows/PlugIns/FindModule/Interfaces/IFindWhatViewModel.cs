using System;
using System.Collections.Generic;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.FindModule.Interfaces
{
  /// <summary>
  /// FindDialog view model interface
  /// </summary>
  public interface IFindWhatViewModel
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
      set;
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
      set;
    }

    /// <summary>
    /// Selected item
    /// </summary>
    KeyValuePair<string, string> SelectedItem
    {
      get;
    }

    /// <summary>
    /// Window <see cref="Guid"/>
    /// </summary>
    Guid WindowGuid
    {
      get;
      set;
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
