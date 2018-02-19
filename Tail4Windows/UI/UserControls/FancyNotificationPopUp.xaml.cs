using System.Windows;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Controllers;
using FlowDirection = System.Windows.FlowDirection;


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

      var formattedText = new FormattedText(
        text,
        SettingsHelperController.CurrentSettings.CurrentCultureInfo,
        FlowDirection.LeftToRight,
        new Typeface(popUp.TextBlockDetail.FontFamily, popUp.TextBlockDetail.FontStyle, popUp.TextBlockDetail.FontWeight, popUp.TextBlockDetail.FontStretch),
        popUp.TextBlockDetail.FontSize,
        Brushes.Black,
        new NumberSubstitution(),
        TextFormattingMode.Display);

      var size = new Size(formattedText.Width, formattedText.Height);

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
