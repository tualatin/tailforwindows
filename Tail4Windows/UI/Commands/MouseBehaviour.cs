using System.Windows;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI.Commands
{
  /// <summary>
  /// Mouse behaviours
  /// </summary>
  public class MouseBehaviour
  {
    #region MouseUp

    /// <summary>
    /// MouseUp command
    /// </summary>
    public static readonly DependencyProperty MouseUpCommandProperty = DependencyProperty.RegisterAttached("MouseUpCommand", typeof(ICommand), typeof(MouseBehaviour),
      new FrameworkPropertyMetadata(MouseUpCommandChanged));

    private static void MouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseUp += ElementMouseUp;
    }

    private static void ElementMouseUp(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseUpCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseUp command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseUpCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseUpCommandProperty, value);
    }

    /// <summary>
    /// Get MouseUp command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseUp command</returns>
    public static ICommand GetMouseUpCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseUpCommandProperty);
    }

    #endregion

    #region MouseDown

    /// <summary>
    /// MouseDown command
    /// </summary>
    public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand), typeof(MouseBehaviour),
      new FrameworkPropertyMetadata(MouseDownCommandChanged));

    private static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseDown += ElementMouseDown;
    }

    private static void ElementMouseDown(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseDownCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseDown command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseDownCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseDownCommandProperty, value);
    }

    /// <summary>
    /// Get MouseDown command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseDown command</returns>
    public static ICommand GetMouseDownCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseDownCommandProperty);
    }

    #endregion

    #region MouseLeave

    /// <summary>
    /// MouseLeave command
    /// </summary>
    public static readonly DependencyProperty MouseLeaveCommandProperty = DependencyProperty.RegisterAttached("MouseLeaveCommand", typeof(ICommand), typeof(MouseBehaviour),
      new FrameworkPropertyMetadata(MouseLeaveCommandChanged));

    private static void MouseLeaveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseLeave += ElementMouseLeave;
    }

    private static void ElementMouseLeave(object sender, MouseEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseLeaveCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseLeave command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseLeaveCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseLeaveCommandProperty, value);
    }

    /// <summary>
    /// Get MouseLeave command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseLeave command</returns>
    public static ICommand GetMouseLeaveCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseLeaveCommandProperty);
    }

    #endregion

    #region MouseLeftButtonDown

    /// <summary>
    /// MouseLeftButtonDown command
    /// </summary>
    public static readonly DependencyProperty MouseLeftButtonDownCommandProperty = DependencyProperty.RegisterAttached("MouseLeftButtonDownCommand", typeof(ICommand),
      typeof(MouseBehaviour), new FrameworkPropertyMetadata(MouseLeftButtonDownCommandChanged));

    private static void MouseLeftButtonDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseLeftButtonDown += ElementMouseLeftButtonDown;
    }

    private static void ElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseLeftButtonDownCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseLeftButtonDown command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseLeftButtonDownCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseLeftButtonDownCommandProperty, value);
    }

    /// <summary>
    /// Get MouseLeftButtonDown command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseLeftButtonDown command</returns>
    public static ICommand GetMouseLeftButtonDownCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseLeftButtonDownCommandProperty);
    }

    #endregion

    #region MouseLeftButtonUp

    /// <summary>
    /// MouseLeftButtonUp command
    /// </summary>
    public static readonly DependencyProperty MouseLeftButtonUpCommandProperty = DependencyProperty.RegisterAttached("MouseLeftButtonUpCommand", typeof(ICommand),
      typeof(MouseBehaviour), new FrameworkPropertyMetadata(MouseLeftButtonUpCommandChanged));

    private static void MouseLeftButtonUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseLeftButtonUp += ElementMouseLeftButtonUp;
    }

    private static void ElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseLeftButtonUpCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// SetMouseLeftButtonUp command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseLeftButtonUpCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseLeftButtonUpCommandProperty, value);
    }

    /// <summary>
    /// Get MouseLeftButtonUp command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseLeftButtonUp command</returns>
    public static ICommand GetMouseLeftButtonUpCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseLeftButtonUpCommandProperty);
    }

    #endregion

    #region MouseMove

    /// <summary>
    /// MouseMove command
    /// </summary>
    public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand), typeof(MouseBehaviour),
      new FrameworkPropertyMetadata(MouseMoveCommandChanged));

    private static void MouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseMove += ElementMouseMove;
    }

    private static void ElementMouseMove(object sender, MouseEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseMoveCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseMove command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseMoveCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseMoveCommandProperty, value);
    }

    /// <summary>
    /// Get MouseMove command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseMove command</returns>
    public static ICommand GetMouseMoveCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseMoveCommandProperty);
    }

    #endregion

    #region MouseRightButtonDown

    /// <summary>
    /// MouseRightButtonDown command
    /// </summary>
    public static readonly DependencyProperty MouseRightButtonDownCommandProperty = DependencyProperty.RegisterAttached("MouseRightButtonDownCommand", typeof(ICommand),
      typeof(MouseBehaviour), new FrameworkPropertyMetadata(MouseRightButtonDownCommandChanged));

    private static void MouseRightButtonDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseRightButtonDown += ElementMouseRightButtonDown;
    }

    private static void ElementMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseRightButtonDownCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseRightButtonDown command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseRightButtonDownCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseRightButtonDownCommandProperty, value);
    }

    /// <summary>
    /// Get MouseRightButtonDown command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseRightButtonDown command</returns>
    public static ICommand GetMouseRightButtonDownCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseRightButtonDownCommandProperty);
    }

    #endregion

    #region MouseRightButtonUp

    /// <summary>
    /// MouseRightButtonUp command
    /// </summary>
    public static readonly DependencyProperty MouseRightButtonUpCommandProperty = DependencyProperty.RegisterAttached("MouseRightButtonUpCommand", typeof(ICommand),
      typeof(MouseBehaviour), new FrameworkPropertyMetadata(MouseRightButtonUpCommandChanged));

    private static void MouseRightButtonUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseRightButtonUp += ElementMouseRightButtonUp;
    }

    private static void ElementMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseRightButtonUpCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseRightButtonUp command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseRightButtonUpCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseRightButtonUpCommandProperty, value);
    }

    /// <summary>
    /// Get MouseRightButtonUp
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseRightButtonUp command</returns>
    public static ICommand GetMouseRightButtonUpCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseRightButtonUpCommandProperty);
    }

    #endregion

    #region MouseWheel

    /// <summary>
    /// MouseWheel command
    /// </summary>
    public static readonly DependencyProperty MouseWheelCommandProperty = DependencyProperty.RegisterAttached("MouseWheelCommand", typeof(ICommand), typeof(MouseBehaviour),
      new FrameworkPropertyMetadata(MouseWheelCommandChanged));

    private static void MouseWheelCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)d;
      element.MouseWheel += ElementMouseWheel;
    }

    private static void ElementMouseWheel(object sender, MouseWheelEventArgs e)
    {
      FrameworkElement element = (FrameworkElement)sender;
      ICommand command = GetMouseWheelCommand(element);

      command.Execute(e);
    }

    /// <summary>
    /// Set MouseWheel command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <param name="value">Command value</param>
    public static void SetMouseWheelCommand(UIElement element, ICommand value)
    {
      element.SetValue(MouseWheelCommandProperty, value);
    }

    /// <summary>
    /// Get MouseWheel command
    /// </summary>
    /// <param name="element">Control element</param>
    /// <returns>Current MouseWheel command</returns>
    public static ICommand GetMouseWheelCommand(UIElement element)
    {
      return (ICommand)element.GetValue(MouseWheelCommandProperty);
    }

    #endregion
  }
}
