using System;
using System.Collections.Generic;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager
{
  /// <summary>
  /// Command state manager
  /// </summary>
  public class CommandStateManager : NotifyMaster
  {
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<CommandStateManager> _instance = new Lazy<CommandStateManager>(() => new CommandStateManager());

    private readonly Stack<IUndoCommand> _undos = new Stack<IUndoCommand>();
    private readonly Stack<IUndoCommand> _redos = new Stack<IUndoCommand>();

    /// <summary>
    /// Current CommandStateManager instance
    /// </summary>
    public static CommandStateManager Instance => _instance.Value;

    /// <summary>
    /// Can undo
    /// </summary>
    public bool CanUndo => _undos.Count > 0;

    /// <summary>
    /// Can redo
    /// </summary>
    public bool CanRedo => _redos.Count > 0;

    private CommandStateManager()
    {
    }

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="command">Command of <see cref="IUndoCommand"/></param>
    public void Executed(IUndoCommand command)
    {
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
      OnPropertyChanged(nameof(CanRedo));
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
      command.Execute(null);
      OnPropertyChanged(nameof(CanUndo));
    }
  }
}
