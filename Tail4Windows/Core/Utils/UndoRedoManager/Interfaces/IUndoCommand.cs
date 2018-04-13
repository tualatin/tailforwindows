using System.Windows.Input;


namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager.Interfaces
{
  /// <summary>
  /// UndoCommand interface
  /// </summary>
  public interface IUndoCommand : ICommand
  {
    /// <summary>
    /// Undo changes
    /// </summary>
    void Undo();
  }
}
