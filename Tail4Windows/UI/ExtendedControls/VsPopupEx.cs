using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI.ExtendedControls
{
  /// <summary>
  /// Virtual Studios Popup extended
  /// </summary>
  public class VsPopupEx : Popup
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsPopupEx()
    {
      Placement = PlacementMode.Bottom;
      PopupAnimation = PopupAnimation.Slide;
      StaysOpen = false;
    }

    /// <summary>
    /// OnPreviewMouseLeftButtonDown
    /// </summary>
    /// <param name="e"><see cref="MouseButtonEventArgs"/></param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      bool isOpen = IsOpen;
      base.OnPreviewMouseLeftButtonDown(e);

      if ( isOpen && !IsOpen )
        e.Handled = true;
    }
  }
}
