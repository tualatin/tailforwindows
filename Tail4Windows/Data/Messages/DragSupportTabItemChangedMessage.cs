using System;
using System.Windows.Media;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;

namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// <see cref="DragSupportTabItem"/> properties changed
  /// </summary>
  public class DragSupportTabItemChangedMessage
  {
    /// <summary>
    /// Who sends the event
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Which window calls the event
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Background color
    /// </summary>
    public SolidColorBrush BackgroundColor
    {
      get;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sender">Who sends the event</param>
    /// <param name="backgroundColor">Background color</param>
    /// <param name="windowGuid">Which window calls the event</param>
    public DragSupportTabItemChangedMessage(object sender, SolidColorBrush backgroundColor, Guid windowGuid)
    {
      Sender = sender;
      BackgroundColor = backgroundColor;
      WindowGuid = windowGuid;
    }
  }
}
