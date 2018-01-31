using System.Windows;


namespace Org.Vs.TailForWin.BaseView.StyleableWindow
{
  /// <summary>
  /// Window drag behavior
  /// </summary>
  public static class WindowDragBehavior
  {
    /// <summary>
    /// Get left mouse button drag
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Draged window by left mouse button</returns>
    public static Window GetLeftMouseButtonDrag(DependencyObject sender) => (Window) sender.GetValue(LeftMouseButtonDrag);

    /// <summary>
    /// Set left mouse button drag
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetLeftMouseButtonDrag(DependencyObject sender, Window window) => sender.SetValue(LeftMouseButtonDrag, window);

    /// <summary>
    /// Left mouse button drag property
    /// </summary>
    public static readonly DependencyProperty LeftMouseButtonDrag = DependencyProperty.RegisterAttached("LeftMouseButtonDrag", typeof(Window), typeof(WindowDragBehavior),
                                                                      new UIPropertyMetadata(null, OnLeftMouseButtonDragChanged));

    private static void OnLeftMouseButtonDragChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if( sender is UIElement element )
        element.MouseLeftButtonDown += ButtonDown;
    }

    private static void ButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if( !(sender is UIElement element) )
        return;
      var targetWindow = element.GetValue(LeftMouseButtonDrag) as Window;      targetWindow?.DragMove();
    }
  }
}
