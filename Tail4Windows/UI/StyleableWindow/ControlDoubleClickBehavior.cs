using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;


namespace Org.Vs.TailForWin.UI.StyleableWindow
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
    public static ICommand GetExecuteCommand(DependencyObject sender)
    {
      return (ICommand) sender.GetValue(ExecuteCommand);
    }

    /// <summary>
    /// Set execute command
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="command">Command</param>
    public static void SetExecuteCommand(DependencyObject sender, ICommand command)
    {
      sender.SetValue(ExecuteCommand, command);
    }

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
    public static Window GetExecuteCommandParameter(DependencyObject sender)
    {
      return (Window) sender.GetValue(ExecuteCommandParameter);
    }

    /// <summary>
    /// Set execute command parameter
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="command">Command</param>
    public static void SetExecuteCommandParameter(DependencyObject sender, ICommand command)
    {
      sender.SetValue(ExecuteCommandParameter, command);
    }

    /// <summary>
    /// Execute command parameter property
    /// </summary>
    public static readonly DependencyProperty ExecuteCommandParameter = DependencyProperty.RegisterAttached("ExecuteCommandParameter", typeof(Window), typeof(ControlDoubleClickBehavior));

    private static void OnExecuteCommandChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if(sender is Control control)
        control.MouseDoubleClick += Control_MouseDoubleClick;
      else if(sender is Border border)
        border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
    }

    static void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(e.ClickCount == 2)
      {
        var border = sender as Border;

        if(border != null)
        {
          var command = border.GetValue(ExecuteCommand) as ICommand;
          var commandParameter = border.GetValue(ExecuteCommandParameter);

          if(command != null && command.CanExecute(e))
            command.Execute(commandParameter);
        }
      }
    }

    private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if(sender is Control control)
      {
        var command = control.GetValue(ExecuteCommand) as ICommand;
        var commandParameter = control.GetValue(ExecuteCommandParameter);

        if(command != null && command.CanExecute(e))
          command.Execute(commandParameter);
      }
    }
  }
}
