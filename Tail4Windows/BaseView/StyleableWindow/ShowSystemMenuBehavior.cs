using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.BaseView.StyleableWindow
{
  /// <summary>
  /// Show system menu behavior
  /// </summary>
  public class ShowSystemMenuBehavior
  {
    #region TargetWindow

    /// <summary>
    /// Get target window
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Target window</returns>
    public static Window GetTargetWindow(DependencyObject sender) => (Window) sender.GetValue(TargetWindow);

    /// <summary>
    /// Set target window
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetTargetWindow(DependencyObject sender, Window window) => sender.SetValue(TargetWindow, window);

    /// <summary>
    /// Target window property
    /// </summary>
    public static readonly DependencyProperty TargetWindow = DependencyProperty.RegisterAttached("TargetWindow", typeof(Window), typeof(ShowSystemMenuBehavior));

    #endregion

    #region LeftButtonShowAt

    /// <summary>
    /// Get left button show at
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>UIEelement</returns>
    public static UIElement GetLeftButtonShowAt(DependencyObject sender) => (UIElement) sender.GetValue(LeftButtonShowAt);

    /// <summary>
    /// Set left button show at
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="element">UIElement</param>
    public static void SetLeftButtonShowAt(DependencyObject sender, UIElement element) => sender.SetValue(LeftButtonShowAt, element);

    /// <summary>
    /// Left button show at property
    /// </summary>
    public static readonly DependencyProperty LeftButtonShowAt = DependencyProperty.RegisterAttached("LeftButtonShowAt", typeof(UIElement), typeof(ShowSystemMenuBehavior),
                                                                  new UIPropertyMetadata(null, LeftButtonShowAtChanged));
    #endregion

    #region RightButtonShow

    /// <summary>
    /// Get right button show
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>If right button is shown <c>true</c> otherwise <c>false</c></returns>
    public static bool GetRightButtonShow(DependencyObject sender) => (bool) sender.GetValue(RightButtonShow);

    /// <summary>
    /// Set right button show
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="arg">Argument</param>
    public static void SetRightButtonShow(DependencyObject sender, bool arg) => sender.SetValue(RightButtonShow, arg);

    /// <summary>
    /// Right button show property
    /// </summary>
    public static readonly DependencyProperty RightButtonShow = DependencyProperty.RegisterAttached("RightButtonShow", typeof(bool), typeof(ShowSystemMenuBehavior),
                                                                  new UIPropertyMetadata(false, RightButtonShowChanged));
    #endregion

    #region LeftButtonShowAt changed

    private static void LeftButtonShowAtChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is UIElement element )
        element.MouseLeftButtonDown += LeftButtonDownShow;
    }

    private static bool leftButtonToggle = true;

    private static void LeftButtonDownShow(object sender, MouseButtonEventArgs e)
    {
      if ( leftButtonToggle )
      {
        var element = ((UIElement) sender).GetValue(LeftButtonShowAt);
        var showMenuAt = ((Visual) element).PointToScreen(new Point(0, 0));
        var targetWindow = ((UIElement) sender).GetValue(TargetWindow) as Window;

        SystemMenuManager.ShowMenu(targetWindow, showMenuAt);

        leftButtonToggle = !leftButtonToggle;
      }
      else
      {
        leftButtonToggle = !leftButtonToggle;
      }

      // Throttled execution to set leftButtonToggle to true again...
      new ThrottledExecution().InMs(250).Do(() =>
      {
        if ( !leftButtonToggle )
          leftButtonToggle = true;
      });
    }

    #endregion

    #region RightButtonShow handlers

    private static void RightButtonShowChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is UIElement element )
        element.MouseRightButtonDown += RightButtonDownShow;
    }

    private static void RightButtonDownShow(object sender, MouseButtonEventArgs e)
    {
      var element = (UIElement) sender;

      if ( !(element.GetValue(TargetWindow) is Window targetWindow) )
        return;

      var showMenuAt = targetWindow.PointToScreen(Mouse.GetPosition(targetWindow));
      SystemMenuManager.ShowMenu(targetWindow, showMenuAt);
    }

    #endregion
  }
}
