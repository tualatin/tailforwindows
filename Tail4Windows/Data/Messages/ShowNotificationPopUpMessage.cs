using System.Windows;
using System.Windows.Controls.Primitives;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Show notification PopUp message
  /// </summary>
  public class ShowNotificationPopUpMessage
  {
    /// <summary>
    /// Balloon <see cref="UIElement"/>
    /// </summary>
    public UIElement Balloon
    {
      get;
    }

    /// <summary>
    /// PopUp animation <see cref="PopupAnimation"/>
    /// </summary>
    public PopupAnimation Animation
    {
      get;
    }

    /// <summary>
    /// Timeout
    /// </summary>
    public int Timeout
    {
      get;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="balloon">Balloon control</param>
    /// <param name="timeout">Timeout in ms</param>
    /// <param name="animation">Animation <see cref="PopupAnimation"/></param>
    public ShowNotificationPopUpMessage(UIElement balloon, int timeout = 5000, PopupAnimation animation = PopupAnimation.Fade)
    {
      Balloon = balloon;
      Timeout = timeout;
      Animation = animation;
    }
  }
}
