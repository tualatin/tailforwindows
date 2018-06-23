using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.PatternModule.Interfaces
{
  /// <summary>
  /// PatternControl view model interface
  /// </summary>
  public interface IPatternControlViewModel
  {
    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    TailData CurrenTailData
    {
      get;
      set;
    }

    /// <summary>
    /// Current working pattern
    /// </summary>
    string WorkingPattern
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="System.Windows.Controls.TextBox"/> has focus
    /// </summary>
    bool TextBoxHasFocus
    {
      get;
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
    /// <see cref="ObservableCollection{T}"/> of <see cref="MenuItem"/>
    /// </summary>
    ObservableCollection<MenuItem> MenuItems
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

    /// <summary>
    /// Close command
    /// </summary>
    ICommand CloseCommand
    {
      get;
    }
  }
}
