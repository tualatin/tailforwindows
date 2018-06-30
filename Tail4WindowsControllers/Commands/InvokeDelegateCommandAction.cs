using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.Controllers.Commands
{
  /// <summary>
  /// Invoke deletegate command action
  /// </summary>
  public sealed class InvokeDelegateCommandAction : TriggerAction<DependencyObject>
  {
    /// <summary>
    /// Command parameter property
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeDelegateCommandAction), null);

    /// <summary>
    /// Command property
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeDelegateCommandAction), null);

    /// <summary>
    /// Invoke parameter property
    /// </summary>
    public static readonly DependencyProperty InvokeParameterProperty = DependencyProperty.Register("InvokeParameter", typeof(object), typeof(InvokeDelegateCommandAction), null);

    private string _commandName;

    /// <summary>
    /// Invoke parameter
    /// </summary>
    public object InvokeParameter
    {
      get => GetValue(InvokeParameterProperty);
      set => SetValue(InvokeParameterProperty, value);
    }

    /// <summary>
    /// Command
    /// </summary>
    public ICommand Command
    {
      get => (ICommand) GetValue(CommandProperty);
      set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Command name
    /// </summary>
    public string CommandName
    {
      get => _commandName;
      set
      {
        if ( _commandName != null && _commandName == value )
          return;

        _commandName = value;
      }
    }

    /// <summary>
    /// Command parameter
    /// </summary>
    public object CommandParameter
    {
      get => GetValue(CommandParameterProperty);
      set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Invoke command
    /// </summary>
    /// <param name="parameter">Command parameter</param>
    protected override void Invoke(object parameter)
    {
      InvokeParameter = parameter;

      if ( AssociatedObject == null )
        return;

      ICommand command = ResolveCommand();

      if ( command != null && command.CanExecute(CommandParameter) )
        command.Execute(CommandParameter);
    }

    private ICommand ResolveCommand()
    {
      ICommand command = null;

      if ( Command != null )
        return Command;

      if ( AssociatedObject is FrameworkElement frameworkElement )
      {
        object dataContext = frameworkElement.DataContext;

        if ( dataContext != null )
        {
          PropertyInfo commandPropertyInfo = dataContext.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => typeof(ICommand).IsAssignableFrom(p.PropertyType) && string.Equals(p.Name, CommandName, StringComparison.Ordinal));

          if ( commandPropertyInfo != null )
            command = (ICommand) commandPropertyInfo.GetValue(dataContext, null);
        }
      }
      return command;
    }
  }
}
