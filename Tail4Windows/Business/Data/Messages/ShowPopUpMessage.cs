﻿using System.Windows;
using System.Windows.Controls.Primitives;


namespace Org.Vs.TailForWin.Business.Data.Messages
{
  /// <summary>
  /// Show PopUp message
  /// </summary>
  public class ShowPopUpMessage
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
    public ShowPopUpMessage(UIElement balloon)
    {
      Balloon = balloon;
      Timeout = 5000;
      Animation = PopupAnimation.Slide;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="balloon">Balloon control</param>
    /// <param name="timeout">Timeout in ms</param>
    public ShowPopUpMessage(UIElement balloon, int timeout)
    {
      Balloon = balloon;
      Timeout = timeout;
      Animation = PopupAnimation.Slide;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="balloon">Balloon control</param>
    /// <param name="timeout">Timeout in ms</param>
    /// <param name="animation">Animation <see cref="PopupAnimation"/></param>
    public ShowPopUpMessage(UIElement balloon, int timeout, PopupAnimation animation)
    {
      Balloon = balloon;
      Timeout = timeout;
      Animation = animation;
    }
  }
}
