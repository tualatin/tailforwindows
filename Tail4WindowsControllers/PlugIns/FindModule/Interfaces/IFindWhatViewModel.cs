using System;
using System.Collections.Generic;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Interfaces
{
  /// <summary>
  /// FindDialog view model interface
  /// </summary>
  public interface IFindWhatViewModel : IViewModelBase
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
    /// Caret index
    /// </summary>
    int CaretIndex
    {
      get;
      set;
    }

    /// <summary>
    /// Closing command
    /// </summary>
    ICommand ClosingCommand
    {
      get;
    }

    /// <summary>
    /// FindNext command
    /// </summary>
    IAsyncCommand FindNextCommand
    {
      get;
    }

    /// <summary>
    /// FindAll command
    /// </summary>
    IAsyncCommand FindAllCommand
    {
      get;
    }

    /// <summary>
    /// Count command
    /// </summary>
    IAsyncCommand CountCommand
    {
      get;
    }

    /// <summary>
    /// Wrap around command
    /// </summary>
    IAsyncCommand WrapAroundCommand
    {
      get;
    }

    /// <summary>
    /// Delete histroy command
    /// </summary>
    IAsyncCommand DeleteHistoryCommand
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

    /// <summary>
    /// Can execute find command
    /// </summary>
    /// <returns><c>True</c> if it can execute otherwise <c>False</c></returns>
    bool CanExecuteFindCommand();
  }
}
