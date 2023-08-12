using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;
using Org.Vs.Tail4Win.Core.Data;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Global highlight option view model interface
  /// </summary>
  public interface IGlobalHighlightOptionViewModel : IOptionBaseViewModel
  {
    /// <summary>
    /// Filter manager view
    /// </summary>
    ListCollectionView FilterManagerView
    {
      get;
      set;
    }

    /// <summary>
    /// Current selected item
    /// </summary>
    FilterData SelectedItem
    {
      get;
      set;
    }

    /// <summary>
    /// Global highlight collection changed
    /// </summary>
    bool GlobalHighlightCollectionChanged
    {
      get;
    }

    /// <summary>
    /// Global highlight collection
    /// </summary>
    ObservableCollection<FilterData> GlobalHighlightCollection
    {
      get;
    }

    /// <summary>
    /// Saves current collection
    /// </summary>
    IAsyncCommand SaveCommand
    {
      get;
    }

    /// <summary>
    /// Add highlight color to source
    /// </summary>
    ICommand AddHighlightColorCommand
    {
      get;
    }

    /// <summary>
    /// Delete highlight color from source
    /// </summary>
    IAsyncCommand DeleteHighlightColorCommand
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
