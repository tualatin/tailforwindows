using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// FilterManager view model interface
  /// </summary>
  public interface IFilterManagerViewModel
  {
    /// <summary>
    /// Loaded command
    /// </summary>
    ICommand LoadedCommand
    {
      get;
    }

    /// <summary>
    /// Add <see cref="FilterData"/> command
    /// </summary>
    ICommand AddFilterDataCommand
    {
      get;
    }

    /// <summary>
    /// Close command
    /// </summary>
    ICommand CloseCommand
    {
      get;
    }

    /// <summary>
    /// Delete <see cref="FilterData"/> from FileManager
    /// </summary>
    IAsyncCommand DeleteFilterDataCommand
    {
      get;
    }

    /// <summary>
    /// Converts a local filter to global
    /// </summary>
    IAsyncCommand LocalToGlobalFilterCommand
    {
      get;
    }

    /// <summary>
    /// FileManager view
    /// </summary>
    ListCollectionView FilterManagerView
    {
      get;
      set;
    }

    /// <summary>
    /// Font command
    /// </summary>
    ICommand FontCommand
    {
      get;
    }

    /// <summary>
    /// SaveButtonVisibility
    /// </summary>
    Visibility SaveButtonVisibility
    {
      get;
      set;
    }

    /// <summary>
    /// Save command
    /// </summary>
    IAsyncCommand SaveCommand
    {
      get;
    }

    /// <summary>
    /// Undo command
    /// </summary>
    ICommand UndoCommand
    {
      get;
    }
  }
}
