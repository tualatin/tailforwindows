using System.Windows;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for FancyPopUp.xaml
  /// </summary>
  public partial class FancyPopUp
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FancyPopUp()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty PopUpAlertProperty = DependencyProperty.Register("PopUpAlert", typeof(string), typeof(FancyPopUp),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// PopUpAlert
    /// </summary>
    public string PopUpAlert
    {
      get
      {
        return ((string) GetValue(PopUpAlertProperty));
      }
      set
      {
        SetValue(PopUpAlertProperty, value);
      }
    }

    public static readonly DependencyProperty PopUpAlertDetailProperty = DependencyProperty.Register("PopUpAlertDetail", typeof(string), typeof(FancyPopUp),
      new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// PopUpAlertDetails
    /// </summary>
    public string PopUpAlertDetail
    {
      get
      {
        return ((string) GetValue(PopUpAlertDetailProperty));
      }
      set
      {
        SetValue(PopUpAlertDetailProperty, value);
      }
    }
  }
}
