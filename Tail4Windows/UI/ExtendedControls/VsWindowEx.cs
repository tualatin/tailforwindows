using System;
using System.ComponentModel;
using System.Windows;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.ExtendedControls
{
  /// <summary>
  /// Virtual Studios WindowEx class
  /// </summary>
  public class VsWindowEx : Window
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public VsWindowEx()
    {
      SourceInitialized += (o, e) =>
      {
        this.HideMinimizeMaximizeButtons();
      };
      Closing += OnVsWindowExClosing;

      ShouldClose = true;
    }

    /// <summary>
    /// Hides / real close the window
    /// </summary>
    public bool ShouldClose
    {
      private get;
      set;
    }

    /// <summary>
    /// Dialog can close
    /// </summary>
    public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register(nameof(CanClose), typeof(bool), typeof(VsWindowEx), new PropertyMetadata(false, CanCloseChanged));

    private static void CanCloseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( e.Property != CanCloseProperty || !(e.NewValue is bool canClose) )
        return;

      if ( canClose && sender is VsWindowEx wnd )
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

    /// <summary>
    /// Parent Guid property
    /// </summary>
    public static readonly DependencyProperty ParentGuidProperty = DependencyProperty.Register(nameof(ParentGuid), typeof(Guid), typeof(VsWindowEx), new PropertyMetadata(Guid.Empty));

    /// <summary>
    /// Parent Guid
    /// </summary>
    public Guid ParentGuid
    {
      get => (Guid) GetValue(ParentGuidProperty);
      set => SetValue(ParentGuidProperty, value);
    }

    private void OnVsWindowExClosing(object sender, CancelEventArgs e)
    {
      if ( ShouldClose )
        return;

      e.Cancel = true;
      Hide();
    }
  }
}
