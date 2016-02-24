using System.Windows;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for FancyPopUp.xaml
  /// </summary>
  public partial class FancyPopUp
  {
    public FancyPopUp ()
    {
      InitializeComponent ( );
    }

    public static readonly DependencyProperty PopUpAlertProperty = DependencyProperty.Register ("PopUpAlert", typeof (string), typeof (FancyPopUp),
      new FrameworkPropertyMetadata (string.Empty));

    public string PopUpAlert
    {
      get
      {
        return ((string) GetValue (PopUpAlertProperty));
      }
      set
      {
        SetValue (PopUpAlertProperty, value);
      }
    }

    public static readonly DependencyProperty PopUpAlertDetailProperty = DependencyProperty.Register ("PopUpAlertDetail", typeof (string), typeof (FancyPopUp),
      new FrameworkPropertyMetadata (string.Empty));

    public string PopUpAlertDetail
    {
      get
      {
        return ((string) GetValue (PopUpAlertDetailProperty));
      }
      set
      {
        SetValue (PopUpAlertDetailProperty, value);
      }
    }
  }
}
