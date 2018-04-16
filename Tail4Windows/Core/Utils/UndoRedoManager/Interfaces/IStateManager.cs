namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager.Interfaces
{
  /// <summary>
  /// StateManager interface
  /// </summary>
  public interface IStateManager
  {
    /// <summary>
    /// Can undo
    /// </summary>
    bool CanUndo
    {
      get;
    }

    /// <summary>
    /// Can redo
    /// </summary>
    bool CanRedo
    {
      get;
    }

    /// <summary>
    /// Undo
    /// </summary>
    void Undo();

    /// <summary>
    /// Redo
    /// </summary>
    void Redo();
  }
}
