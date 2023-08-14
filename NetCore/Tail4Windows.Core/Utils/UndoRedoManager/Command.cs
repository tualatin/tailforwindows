namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager
{
  /// <summary>
  /// Command
  /// </summary>
  public class Command
  {
    private readonly Action _action;
    private readonly Action _undoAction;
    private readonly Action<string> _notificationAction;
    private readonly string _propertyName;


    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="action"><see cref="Action"/></param>
    /// <param name="undoAction"><see cref="Action"/></param>
    /// <param name="propertyName">Name of property</param>
    /// <param name="notification">Notification action</param>
    public Command(Action action, Action undoAction, string propertyName, Action<string> notification)
    {
      _undoAction = undoAction;
      _action = action;
      _propertyName = propertyName;
      _notificationAction = notification;
    }

    /// <summary>
    /// Execute
    /// </summary>
    public void Execute()
    {
      _action();
      _notificationAction(_propertyName);
    }

    /// <summary>
    /// Undo
    /// </summary>
    public void Undo()
    {
      _undoAction();
      _notificationAction(_propertyName);
    }
  }
}
