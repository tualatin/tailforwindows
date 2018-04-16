using System;


namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager
{
  /// <summary>
  /// Command
  /// </summary>
  public class Command
  {
    private readonly Action _action;
    private readonly Action _undoAction;

    /// <summary>
    /// Standarc constructor
    /// </summary>
    /// <param name="action"><see cref="Action"/></param>
    /// <param name="undoAction"><see cref="Action"/></param>
    public Command(Action action, Action undoAction)
    {
      _undoAction = undoAction;
      _action = action;
    }

    /// <summary>
    /// Execute
    /// </summary>
    public void Execute() => _action();

    /// <summary>
    /// Undo
    /// </summary>
    public void Undo() => _undoAction();
  }
}
