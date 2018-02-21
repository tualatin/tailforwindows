using System.Windows;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Interaction logic for FancyPopUp.xaml
  /// </summary>
  public partial class FancyAlertPopUp
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FancyAlertPopUp() => InitializeComponent();

    /// <summary>
    /// PopUpAlertProperty
    /// </summary>
    public static readonly DependencyProperty PopUpAlertProperty = DependencyProperty.Register("PopUpAlert", typeof(string), typeof(FancyAlertPopUp),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// PopUpAlert
    /// </summary>
    public string PopUpAlert
    {
      get => (string) GetValue(PopUpAlertProperty);
      set => SetValue(PopUpAlertProperty, value);
    }

    /// <summary>
    /// PopUpAlertDetailProperty
    /// </summary>
    public static readonly DependencyProperty PopUpAlertDetailProperty = DependencyProperty.Register("PopUpAlertDetail", typeof(string), typeof(FancyAlertPopUp),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// PopUpAlertDetails
    /// </summary>
    public string PopUpAlertDetail
    {
      get => (string) GetValue(PopUpAlertDetailProperty);
      set => SetValue(PopUpAlertDetailProperty, value);
    }
  }
}
