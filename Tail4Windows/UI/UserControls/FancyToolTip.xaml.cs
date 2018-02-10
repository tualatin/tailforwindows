using System.Windows;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Interaction logic for FancyToolTip.xaml
  /// </summary>
  public partial class FancyToolTip
  {
    #region InfoText dependency property

    /// <summary>
    /// The tool tip details.
    /// </summary>
    public static readonly DependencyProperty InfoTextProperty = DependencyProperty.Register("InfoText", typeof(string), typeof(FancyToolTip), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// A property wrapper for the <see cref="InfoTextProperty"/>
    /// dependency property:<br/>
    /// The tooltip details.
    /// </summary>
    public string InfoText
    {
      get => (string) GetValue(InfoTextProperty);
      set => SetValue(InfoTextProperty, value);
    }

    /// <summary>
    /// Application text property
    /// </summary>
    public static readonly DependencyProperty ApplicationTextProperty = DependencyProperty.Register("ApplicationText", typeof(string), typeof(FancyToolTip),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Application text
    /// </summary>
    public string ApplicationText
    {
      get => (string) GetValue(ApplicationTextProperty);
      set => SetValue(ApplicationTextProperty, value);
    }

    /// <summary>
    /// ToolTip detail property
    /// </summary>
    public static readonly DependencyProperty ToolTipDetailProperty = DependencyProperty.Register("ToolTipDetail", typeof(string), typeof(FancyToolTip),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Tool tip details
    /// </summary>
    public string ToolTipDetail
    {
      get => (string) GetValue(ToolTipDetailProperty);
      set
      {
        if ( string.IsNullOrEmpty(value) )
        {
          SetValue(ToolTipDetailProperty, value);
          return;
        }

        var cutStr = value.Length > 30 ? $"{value.Substring(0, 30)}..." : value;

        SetValue(ToolTipDetailProperty, cutStr);
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FancyToolTip() => InitializeComponent();
  }
}
