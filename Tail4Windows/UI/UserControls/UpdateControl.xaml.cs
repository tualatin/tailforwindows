using System.Windows;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Interaction logic for UpdateControl.xaml
  /// </summary>
  public partial class UpdateControl
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public UpdateControl() => InitializeComponent();

    /// <summary>
    /// ApplicationVersion property
    /// </summary>
    public static readonly DependencyProperty ApplicationVersionProperty = DependencyProperty.Register("ApplicationVersion", typeof(string), typeof(UpdateControl), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Application version
    /// </summary>
    public string ApplicationVersion
    {
      get => (string) GetValue(ApplicationVersionProperty);
      set => SetValue(ApplicationVersionProperty, value);
    }

    /// <summary>
    /// WebVersion property
    /// </summary>
    public static readonly DependencyProperty WebVersionProperty = DependencyProperty.Register("WebVersion", typeof(string), typeof(UpdateControl), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Web version
    /// </summary>
    public string WebVersion
    {
      get => (string) GetValue(WebVersionProperty);
      set => SetValue(WebVersionProperty, value);
    }

    /// <summary>
    /// Update hint property
    /// </summary>
    public static readonly DependencyProperty UpdateHintProperty = DependencyProperty.Register("UpdateHint", typeof(string), typeof(UpdateControl), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Applicaton version
    /// </summary>
    public string UpdateHint
    {
      get => (string) GetValue(UpdateHintProperty);
      set => SetValue(UpdateHintProperty, value);
    }
  }
}
