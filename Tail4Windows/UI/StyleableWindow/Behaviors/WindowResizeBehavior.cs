﻿using System.Windows;
using System.Windows.Controls.Primitives;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.UI.StyleableWindow.Behaviors
{
  /// <summary>
  /// Window resize behavior
  /// </summary>
  public static class WindowResizeBehavior
  {
    /// <summary>
    /// Get top left resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetTopLeftResize(DependencyObject sender) => (Window) sender.GetValue(TopLeftResize);

    /// <summary>
    /// Set top left resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetTopLeftResize(DependencyObject sender, Window window) => sender.SetValue(TopLeftResize, window);

    /// <summary>
    /// Top left resize property
    /// </summary>
    public static readonly DependencyProperty TopLeftResize = DependencyProperty.RegisterAttached("TopLeftResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnTopLeftResizeChanged));

    private static void OnTopLeftResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragTopLeft;
    }

    /// <summary>
    /// Get top right resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetTopRightResize(DependencyObject sender) => (Window) sender.GetValue(TopRightResize);

    /// <summary>
    /// Set top right resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetTopRightResize(DependencyObject sender, Window window) => sender.SetValue(TopRightResize, window);

    /// <summary>
    /// Top right resize property
    /// </summary>
    public static readonly DependencyProperty TopRightResize = DependencyProperty.RegisterAttached("TopRightResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnTopRightResizeChanged));

    private static void OnTopRightResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragTopRight;
    }

    /// <summary>
    /// Get bottom right resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetBottomRightResize(DependencyObject sender) => (Window) sender.GetValue(BottomRightResize);

    /// <summary>
    /// Set bottom right resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetBottomRightResize(DependencyObject sender, Window window) => sender.SetValue(BottomRightResize, window);

    /// <summary>
    /// Bottom right resize property
    /// </summary>
    public static readonly DependencyProperty BottomRightResize = DependencyProperty.RegisterAttached("BottomRightResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnBottomRightResizeChanged));

    private static void OnBottomRightResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragBottomRight;
    }

    /// <summary>
    /// Get bottom left resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetBottomLeftResize(DependencyObject sender) => (Window) sender.GetValue(BottomLeftResize);

    /// <summary>
    /// Set bottom left resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetBottomLeftResize(DependencyObject sender, Window window) => sender.SetValue(BottomLeftResize, window);

    /// <summary>
    /// Bottom left resize property
    /// </summary>
    public static readonly DependencyProperty BottomLeftResize = DependencyProperty.RegisterAttached("BottomLeftResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnBottomLeftResizeChanged));

    private static void OnBottomLeftResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragBottomLeft;
    }

    /// <summary>
    /// Get left resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetLeftResize(DependencyObject sender) => (Window) sender.GetValue(LeftResize);

    /// <summary>
    /// Set left resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetLeftResize(DependencyObject sender, Window window) => sender.SetValue(LeftResize, window);

    /// <summary>
    /// Left resize property
    /// </summary>
    public static readonly DependencyProperty LeftResize = DependencyProperty.RegisterAttached("LeftResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnLeftResizeChanged));

    private static void OnLeftResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragLeft;
    }

    /// <summary>
    /// Get right resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetRightResize(DependencyObject sender) => (Window) sender.GetValue(RightResize);

    /// <summary>
    /// Set right resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetRightResize(DependencyObject sender, Window window) => sender.SetValue(RightResize, window);

    /// <summary>
    /// Right resize property
    /// </summary>
    public static readonly DependencyProperty RightResize = DependencyProperty.RegisterAttached("RightResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnRightResizeChanged));

    private static void OnRightResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragRight;
    }

    /// <summary>
    /// Get top resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetTopResize(DependencyObject sender) => (Window) sender.GetValue(TopResize);

    /// <summary>
    /// Set top window
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetTopResize(DependencyObject sender, Window window) => sender.SetValue(TopResize, window);

    /// <summary>
    /// Top resize window
    /// </summary>
    public static readonly DependencyProperty TopResize = DependencyProperty.RegisterAttached("TopResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnTopResizeChanged));

    private static void OnTopResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragTop;
    }

    /// <summary>
    /// Get bottom resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <returns>Resized window</returns>
    public static Window GetBottomResize(DependencyObject sender) => (Window) sender.GetValue(BottomResize);

    /// <summary>
    /// Set bottom resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="window">Window</param>
    public static void SetBottomResize(DependencyObject sender, Window window) => sender.SetValue(BottomResize, window);

    /// <summary>
    /// Bottom resize property
    /// </summary>
    public static readonly DependencyProperty BottomResize = DependencyProperty.RegisterAttached("BottomResize", typeof(Window), typeof(WindowResizeBehavior),
      new UIPropertyMetadata(null, OnBottomResizeChanged));

    private static void OnBottomResizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( sender is Thumb thumb )
        thumb.DragDelta += DragBottom;
    }

    private static void DragLeft(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(LeftResize) is Window window) )
        return;

      double horizontalChange = window.SafeWidthChange(e.HorizontalChange, false);
      window.Width -= horizontalChange;
      window.Left += horizontalChange;
    }

    private static void DragRight(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(RightResize) is Window window) )
        return;

      double horizontalChange = window.SafeWidthChange(e.HorizontalChange);
      window.Width += horizontalChange;
    }

    private static void DragTop(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(TopResize) is Window window) )
        return;

      double verticalChange = window.SafeHeightChange(e.VerticalChange, false);
      window.Height -= verticalChange;
      window.Top += verticalChange;
    }

    private static void DragBottom(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(BottomResize) is Window window) )
        return;

      double verticalChange = window.SafeHeightChange(e.VerticalChange);
      window.Height += verticalChange;
    }

    private static void DragTopLeft(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(TopLeftResize) is Window window) )
        return;

      double verticalChange = window.SafeHeightChange(e.VerticalChange, false);
      double horizontalChange = window.SafeWidthChange(e.HorizontalChange, false);

      window.Width -= horizontalChange;
      window.Left += horizontalChange;
      window.Height -= verticalChange;
      window.Top += verticalChange;
    }

    private static void DragTopRight(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(TopRightResize) is Window window) )
        return;

      double verticalChange = window.SafeHeightChange(e.VerticalChange, false);
      double horizontalChange = window.SafeWidthChange(e.HorizontalChange);

      window.Width += horizontalChange;
      window.Height -= verticalChange;
      window.Top += verticalChange;
    }

    private static void DragBottomRight(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(BottomRightResize) is Window window) )
        return;

      double verticalChange = window.SafeHeightChange(e.VerticalChange);
      double horizontalChange = window.SafeWidthChange(e.HorizontalChange);

      window.Width += horizontalChange;
      window.Height += verticalChange;
    }

    private static void DragBottomLeft(object sender, DragDeltaEventArgs e)
    {
      if ( !(sender is Thumb thumb) )
        return;
      if ( !(thumb.GetValue(BottomLeftResize) is Window window) )
        return;

      double verticalChange = window.SafeHeightChange(e.VerticalChange);
      double horizontalChange = window.SafeWidthChange(e.HorizontalChange, false);

      window.Width -= horizontalChange;
      window.Left += horizontalChange;
      window.Height += verticalChange;
    }

    private static double SafeWidthChange(this Window window, double change, bool positive = true)
    {
      Arg.NotNull(window, nameof(window));

      double result = positive ? window.Width + change : window.Width - change;

      if ( result <= window.MinWidth )
        return 0;

      if ( result >= window.MaxWidth )
        return 0;

      return result < 0 ? 0 : change;
    }

    private static double SafeHeightChange(this Window window, double change, bool positive = true)
    {
      Arg.NotNull(window, nameof(window));

      double result = positive ? window.Height + change : window.Height - change;

      if ( result <= window.MinHeight )
        return 0;

      if ( result >= window.MaxHeight )
        return 0;

      return result < 0 ? 0 : change;
    }
  }
}
