using System.Collections.Generic;
using log4net;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager
{
  /// <summary>
  /// StateManager
  /// </summary>
  public class StateManager : NotifyMaster, IStateManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(StateManager));

    private readonly Stack<Command> _undos = new Stack<Command>();
    private readonly Stack<Command> _redos = new Stack<Command>();

    /// <summary>
    /// Can undo
    /// </summary>
    public bool CanUndo => _undos.Count > 0;

    /// <summary>
    /// Can redo
    /// </summary>
    public bool CanRedo => _redos.Count > 0;

    /// <summary>
    /// Change current state
    /// </summary>
    /// <param name="command"><see cref="Command"/></param>
    protected void ChangeState(Command command)
    {
      command.Execute();
      _undos.Push(command);

      OnPropertyChanged(nameof(CanUndo));
    }

    /// <summary>
    /// Undo
    /// </summary>
    public void Undo()
    {
      if ( !CanUndo )
        return;

      var command = _undos.Pop();

      _redos.Push(command);
      command.Undo();

      LOG.Trace($"Undo, stack count {_undos.Count}");
      OnPropertyChanged(nameof(CanUndo));
    }

    /// <summary>
    /// Redo
    /// </summary>
    public void Redo()
    {
      if ( !CanRedo )
        return;

      var command = _redos.Pop();

      _undos.Push(command);
      command.Execute();

      LOG.Trace($"Redo, stack count {_redos.Count}");
      OnPropertyChanged(nameof(CanRedo));
    }

    /// <summary>
    /// Clears whole stack
    /// </summary>
    public void Clear()
    {
      _undos.Clear();
      _redos.Clear();

      OnPropertyChanged(nameof(CanUndo));
      OnPropertyChanged(nameof(CanRedo));
    }
  }
}
