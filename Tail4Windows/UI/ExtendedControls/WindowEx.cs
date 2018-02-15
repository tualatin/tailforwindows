﻿using System.Windows;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.ExtendedControls
{
  /// <summary>
  /// WindowEx class
  /// </summary>
  public class WindowEx : Window
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public WindowEx()
    {
      SourceInitialized += (o, e) =>
      {
        this.HideMinimizeMaximizeButtons();
      };
    }

    /// <summary>
    /// Dialog can close
    /// </summary>
    public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(WindowEx), new PropertyMetadata(false, CanCloseChanged));

    private static void CanCloseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( e.Property != CanCloseProperty || !(e.NewValue is bool canClose) )
        return;

      if ( canClose && sender is WindowEx wnd )
        wnd.Close();
    }

    /// <summary>
    /// Can close
    /// </summary>
    public bool CanClose
    {
      get => (bool) GetValue(CanCloseProperty);
      set => SetValue(CanCloseProperty, value);
    }
  }
}