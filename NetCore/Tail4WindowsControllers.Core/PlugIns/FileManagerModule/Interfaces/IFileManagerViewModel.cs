using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;
using Org.Vs.Tail4Win.Controllers.UI.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// FileManager view model interface
  /// </summary>
  public interface IFileManagerViewModel : IViewModelBase, INotifyPropertyChanged
  {
    /// <summary>
    /// Add <see cref="TailData"/> command
    /// </summary>
    ICommand AddTailDataCommand
    {
      get;
    }

    /// <summary>
    /// Categories
    /// </summary>
    ObservableCollection<string> Categories
    {
      get;
      set;
    }

    /// <summary>
    /// Close command
    /// </summary>
    ICommand CloseCommand
    {
      get;
    }

    /// <summary>
    /// MouseDoubleClick command
    /// </summary>
    ICommand DataGridMouseDoubleClickCommand
    {
      get;
    }

    /// <summary>
    /// Delete <see cref="TailData"/> from FileManager
    /// </summary>
    IAsyncCommand DeleteTailDataCommand
    {
      get;
    }

    /// <summary>
    /// FileManager view
    /// </summary>
    ListCollectionView FileManagerView
    {
      get;
      set;
    }

    /// <summary>
    /// Filter command
    /// </summary>
    ICommand FilterCommand
    {
      get;
    }

    /// <summary>
    /// Filter has focus
    /// </summary>
    bool FilterHasFocus
    {
      get;
      set;
    }

    /// <summary>
    /// Current filter text
    /// </summary>
    string FilterText
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
    /// Group by click command
    /// </summary>
    ICommand GroupByClickCommand
    {
      get;
    }

    /// <summary>
    /// Open command
    /// </summary>
    ICommand OpenCommand
    {
      get;
    }

    /// <summary>
    /// Open file command
    /// </summary>
    IAsyncCommand OpenFileCommand
    {
      get;
    }

    /// <summary>
    /// Parent Guid
    /// </summary>
    Guid ParentGuid
    {
      get;
      set;
    }

    /// <summary>
    /// Current Window ID
    /// </summary>
    Guid WindowId
    {
      get;
      set;
    }

    /// <summary>
    /// PatternControl command
    /// </summary>
    ICommand PatternControlCommand
    {
      get;
    }

    /// <summary>
    /// Preview drag enter command
    /// </summary>
    ICommand PreviewDragEnterCommand
    {
      get;
    }

    /// <summary>
    /// Save command
    /// </summary>
    IAsyncCommand SaveCommand
    {
      get;
    }

    /// <summary>
    /// Open Windows events command
    /// </summary>
    ICommand OpenWindowsEventsCommand
    {
      get;
    }

    /// <summary>
    /// Selected category
    /// </summary>
    string SelectedCategory
    {
      get;
      set;
    }

    /// <summary>
    /// SelectedItems
    /// </summary>
    IList SelectedItems
    {
      get;
      set;
    }

    /// <summary>
    /// Undo command
    /// </summary>
    ICommand UndoCommand
    {
      get;
    }

    /// <summary>
    /// Copy element command
    /// </summary>
    ICommand CopyElementCommand
    {
      get;
    }

    /// <summary>
    /// Open containing folder command
    /// </summary>
    IAsyncCommand OpenContainingFolderCommand
    {
      get;
    }
  }
}
