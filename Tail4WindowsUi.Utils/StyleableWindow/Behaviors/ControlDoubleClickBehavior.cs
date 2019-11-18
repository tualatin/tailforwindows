using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Ui.Utils.StyleableWindow.Behaviors
{
  /// <summary>
  /// Double click behavior
  /// </summary>
  public class ControlDoubleClickBehavior
  {
    /// <summary>
    /// Get execute command
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Execute command</returns>
    public static ICommand GetExecuteCommand(DependencyObject sender) => (ICommand) sender.GetValue(ExecuteCommand);

    /// <summary>
    /// Set execute command
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="command">Command</param>
    public static void SetExecuteCommand(DependencyObject sender, ICommand command) => sender.SetValue(ExecuteCommand, command);

    /// <summary>
    /// Execute command property
    /// </summary>
    public static readonly DependencyProperty ExecuteCommand = DependencyProperty.RegisterAttached("ExecuteCommand", typeof(ICommand), typeof(ControlDoubleClickBehavior),
      new UIPropertyMetadata(null, OnExecuteCommandChanged));

    /// <summary>
    /// Get execute command parameter
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Execute command parameter</returns>
    public static Window GetExecuteCommandParameter(DependencyObject sender) => (Window) sender.GetValue(ExecuteCommandParameter);

    /// <summary>
    /// Set execute command parameter
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="command">Command</param>
    public static void SetExecuteCommandParameter(DependencyObject sender, ICommand command) => sender.SetValue(ExecuteCommandParameter, command);

    /// <summary>
    /// Execute command parameter property
    /// </summary>
    public static readonly DependencyProperty ExecuteCommandParameter = DependencyProperty.RegisterAttached("ExecuteCommandParameter", typeof(Window), typeof(ControlDoubleClickBehavior));

    private static void OnExecuteCommandChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      switch( sender )
      {
      case Control control:

        control.MouseDoubleClick += ControlMouseDoubleClick;
        break;

      case Border border:

        border.MouseLeftButtonDown += BorderMouseLeftButtonDown;
        break;
      }
    }

    static void BorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if( e.ClickCount != 2 )
        return;
      if( !(sender is Border border) )
        return;

      var commandParameter = border.GetValue(ExecuteCommandParameter);

      if ( border.GetValue(ExecuteCommand) is ICommand command && command.CanExecute(e) )
        command.Execute(commandParameter);
    }

    private static void ControlMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if( !(sender is Control control) )
        return;

      var commandParameter = control.GetValue(ExecuteCommandParameter);

      if ( control.GetValue(ExecuteCommand) is ICommand command && command.CanExecute(e) )
        command.Execute(commandParameter);
    }
  }
}
