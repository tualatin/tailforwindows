using System.Windows;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Interaction logic for FancyNotificationPopUp.xaml
  /// </summary>
  public partial class FancyNotificationPopUp
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FancyNotificationPopUp() => InitializeComponent();

    /// <summary>
    /// PopUpAlertProperty
    /// </summary>
    public static readonly DependencyProperty PopUpAlertProperty = DependencyProperty.Register("PopUpAlert", typeof(string), typeof(FancyNotificationPopUp),
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
    public static readonly DependencyProperty PopUpAlertDetailProperty = DependencyProperty.Register("PopUpAlertDetail", typeof(string), typeof(FancyNotificationPopUp),
      new PropertyMetadata(string.Empty, PopUpAlertDetailChanged));

    private static void PopUpAlertDetailChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(e.NewValue is string text) || !(sender is FancyNotificationPopUp popUp) )
        return;

      var typeface = new Typeface(popUp.TextBlockDetail.FontFamily, popUp.TextBlockDetail.FontStyle, popUp.TextBlockDetail.FontWeight, popUp.TextBlockDetail.FontStretch);
      var size = text.GetMeasureTextSize(typeface, popUp.TextBlockDetail.FontSize);

      if ( size.Width > popUp.Width )
        popUp.Width = size.Width + 45;
    }

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
