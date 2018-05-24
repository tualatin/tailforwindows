﻿using System.ComponentModel;
using System.Windows;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;


namespace Org.Vs.TailForWin.UI.FloatWindow
{
  /// <summary>
  /// Floating window
  /// </summary>
  public class VsFloatingWindow : Window
  {
    #region Properties

    /// <summary>
    /// Hides / real close the window
    /// </summary>
    public bool ShouldClose
    {
      private get;
      set;
    }

    #endregion

    static VsFloatingWindow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VsFloatingWindow), new FrameworkPropertyMetadata(typeof(VsFloatingWindow)));

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected VsFloatingWindow()
    {
      WindowStyle = WindowStyle.None;
      AllowsTransparency = true;
      Style = (Style) Application.Current.TryFindResource("VsFloatingWindowStyle");
      Topmost = true;

      Closing += VsFloatingWindowClosing;
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<SetFloatingTopmostFlagMessage>(TopmostChanged);
    }

    private void TopmostChanged(SetFloatingTopmostFlagMessage obj) => Topmost = obj.Topmost;

    private void VsFloatingWindowClosing(object sender, CancelEventArgs e)
    {
      if ( ShouldClose )
        return;

      e.Cancel = true;
      Hide();
    }
  }
}
