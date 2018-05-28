using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;


namespace Org.Vs.TailForWin.UI.FloatWindow
{
  /// <summary>
  /// Floating window
  /// </summary>
  public class VsFloatingWindow : Window
  {
    #region Dependency properties

    /// <summary>
    /// MinimizeButtonVisibilityProperty property
    /// </summary>
    public static readonly DependencyProperty MinimizeButtonVisibilityProperty = DependencyProperty.Register(nameof(MinimizeButtonVisibility), typeof(Visibility), typeof(VsFloatingWindow),
      new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Gets/sets MinimizeButtonVisibility
    /// </summary>
    public Visibility MinimizeButtonVisibility
    {
      get => (Visibility) GetValue(MinimizeButtonVisibilityProperty);
      set => SetValue(MinimizeButtonVisibilityProperty, value);
    }

    /// <summary>
    /// TitleBackgroundColorProperty property
    /// </summary>
    public static readonly DependencyProperty TitleBackgroundColorProperty = DependencyProperty.Register(nameof(TitleBackgroundColor), typeof(SolidColorBrush), typeof(VsFloatingWindow),
      new PropertyMetadata((SolidColorBrush) Application.Current.TryFindResource("BrushSolidYellow")));

    /// <summary>
    /// Gets/sets TitleBackgroundColor
    /// </summary>
    public SolidColorBrush TitleBackgroundColor
    {
      get => (SolidColorBrush) GetValue(TitleBackgroundColorProperty);
      set => SetValue(TitleBackgroundColorProperty, value);
    }

    #endregion

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
    // ReSharper disable once MemberCanBeProtected.Global
    public VsFloatingWindow()
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
