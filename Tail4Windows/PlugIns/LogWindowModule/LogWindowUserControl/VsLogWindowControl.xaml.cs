using System.Windows;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl
{
  /// <summary>
  /// Interaction logic for VsLogWindowControl.xaml
  /// </summary>
  public partial class VsLogWindowControl
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsLogWindowControl() => InitializeComponent();

    /// <summary>
    /// ShowGridSplitControl property
    /// </summary>
    public static readonly DependencyProperty ShowGridSplitControlProperty = DependencyProperty.Register(nameof(ShowGridSplitControl), typeof(bool), typeof(VsLogWindowControl),
      new PropertyMetadata(false, ShowGridSplitControlChangedCallback));

    private static void ShowGridSplitControlChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is VsLogWindowControl control) )
        return;

      control.LogWindowListBoxMain.ShowGridSplitControl = (bool) e.NewValue;
    }

    /// <summary>
    /// ShowGridSplitControl
    /// </summary>
    public bool ShowGridSplitControl
    {
      get => (bool) GetValue(ShowGridSplitControlProperty);
      set => SetValue(ShowGridSplitControlProperty, value);
    }

    /// <summary>
    /// <see cref="LogWindowListBox"/> control
    /// </summary>
    public LogWindowListBox MyLogWindowListBox => LogWindowListBoxMain;
  }
}
