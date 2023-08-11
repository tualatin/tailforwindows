using Newtonsoft.Json;
using Org.Vs.Tail4Win.Core.Data.Base;
using Org.Vs.Tail4Win.Core.Utils.UndoRedoManager.Interfaces;

namespace Org.Vs.Tail4Win.Core.Utils.UndoRedoManager
{
  /// <summary>
  /// StateManager
  /// </summary>
  public class StateManager : NotifyMaster, IStateManager
  {
    private readonly Stack<Command> _undos = new Stack<Command>();
    private readonly Stack<Command> _redos = new Stack<Command>();

    /// <summary>
    /// Can undo
    /// </summary>
    [JsonIgnore]
    public bool CanUndo => _undos.Count > 0;

    /// <summary>
    /// Can redo
    /// </summary>
    [JsonIgnore]
    public bool CanRedo => _redos.Count > 0;

    /// <summary>
    /// OnPropertyChanged notification
    /// </summary>
    /// <param name="propertyName">Name of property</param>
    protected void Notification(string propertyName) => OnPropertyChanged(propertyName);

    /// <summary>
    /// Change current state
    /// </summary>
    /// <param name="command"><see cref="Command"/></param>
    protected void ChangeState(Command command)
    {
      _undos.Push(command);
      command.Execute();
    }

    /// <summary>
    /// Undo
    /// </summary>
    public void Undo()
    {
      if ( !CanUndo )
        return;

      Command command = _undos.Pop();

      _redos.Push(command);
      command.Undo();
    }

    /// <summary>
    /// Redo
    /// </summary>
    public void Redo()
    {
      if ( !CanRedo )
        return;

      Command command = _redos.Pop();

      _undos.Push(command);
      command.Execute();
    }

    /// <summary>
    /// Commit all changes
    /// </summary>
    public void CommitChanges()
    {
      _undos.Clear();
      _redos.Clear();

      OnPropertyChanged(nameof(CanRedo));
      OnPropertyChanged(nameof(CanUndo));
    }
  }
}
