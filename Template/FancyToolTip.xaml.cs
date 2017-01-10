using System.Windows;


namespace Org.Vs.TailForWin.Template
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
      get
      {
        return ((string)GetValue(InfoTextProperty));
      }
      set
      {
        SetValue(InfoTextProperty, value);
      }
    }

    public static readonly DependencyProperty ApplicationTextProperty = DependencyProperty.Register("ApplicationText", typeof(string), typeof(FancyToolTip),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Application text
    /// </summary>
    public string ApplicationText
    {
      get
      {
        return ((string)GetValue(ApplicationTextProperty));
      }
      set
      {
        SetValue(ApplicationTextProperty, value);
      }
    }

    public static readonly DependencyProperty ToolTipDetailProperty = DependencyProperty.Register("ToolTipDetail", typeof(string), typeof(FancyToolTip),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Tool tip details
    /// </summary>
    public string ToolTipDetail
    {
      get
      {
        return ((string)GetValue(ToolTipDetailProperty));
      }
      set
      {
        SetValue(ToolTipDetailProperty, value);
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FancyToolTip()
    {
      InitializeComponent();
    }
  }
}
