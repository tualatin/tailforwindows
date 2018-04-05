using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// DropDown button behavior
  /// </summary>
  public class DropDownButtonBehavior : Behavior<Button>
  {
    private long _attachedCount;
    private bool _isContextMenuOpen;

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObjectClick), true);
    }

    private void AssociatedObjectClick(object sender, RoutedEventArgs e)
    {
      if ( !(sender is Button source) || source.ContextMenu == null )
        return;

      // Only open the ContextMenu when it is not already open. If it is already open,
      // when the button is pressed the ContextMenu will lose focus and automatically close.
      if ( _isContextMenuOpen )
        return;

      source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenuClosed), true);
      Interlocked.Increment(ref _attachedCount);

      // If there is a drop-down assigned to this button, then position and display it 
      source.ContextMenu.PlacementTarget = source;
      source.ContextMenu.Placement = PlacementMode.Bottom;
      source.ContextMenu.IsOpen = true;
      _isContextMenuOpen = true;
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObjectClick));
    }

    private void ContextMenuClosed(object sender, RoutedEventArgs e)
    {
      _isContextMenuOpen = false;

      if ( !(sender is ContextMenu contextMenu) )
        return;

      contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenuClosed));
      Interlocked.Decrement(ref _attachedCount);
    }
  }
}
