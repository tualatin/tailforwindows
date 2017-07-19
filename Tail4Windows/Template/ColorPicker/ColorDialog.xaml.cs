using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace Org.Vs.TailForWin.Template.ColorPicker
{
  /// <summary>
  /// Interaction logic for ColorDialog.xaml
  /// </summary>
  public partial class ColorDialog
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorDialog()
      : this(Colors.Black)
    {

    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialColor">Initialize color</param>
    public ColorDialog(Color initialColor)
    {
      InitializeComponent();
      ColorPicker.InitialColor = initialColor;

      PreviewKeyDown += HandleEsc;
    }

    /// <summary>
    /// Gets/sets the ColorDialog color.
    /// </summary>
    public Color SelectedColor
    {
      get => ColorPicker.SelectedColor;
      set => ColorPicker.InitialColor = value;
    }

    /// <summary>
    /// Gets selected color in hex
    /// </summary>
    public string SelectedColorHex => ColorPicker.SelectedHexColor;

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        btnCancel_Click(sender, e);
    }
  }
}
