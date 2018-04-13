using System;
using System.ComponentModel;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Core.Utils.UndoRedoManager.Commands
{
  /// <summary>
  /// RedoCommand
  /// </summary>
  public class RedoCommand : ICommand
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public RedoCommand () => CommandStateManager.Instance.PropertyChanged += InstancePropertyChanged;

    /// <summary>
    /// Can execute changed
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Can execute
    /// </summary>
    /// <param name="parameter">Parameter</param>
    /// <returns>True if it can execute otherwise false</returns>
    public bool CanExecute(object parameter) => CommandStateManager.Instance.CanRedo;

    /// <summary>
    /// Execute command
    /// </summary>
    /// <param name="parameter">Parameter</param>
    public void Execute(object parameter) => CommandStateManager.Instance.Redo();

    private void InstancePropertyChanged(object sender, PropertyChangedEventArgs e) => CanExecuteChanged?.Invoke(this, e);
  }
}
