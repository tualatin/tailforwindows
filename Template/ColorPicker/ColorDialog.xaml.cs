using System.Windows;
using System.Windows.Media;
using System.Windows.Input;


namespace TailForWin.Template.ColorPicker
{
  /// <summary>
  /// Interaction logic for ColorDialog.xaml
  /// </summary>
  public partial class ColorDialog: Window
  {
    public ColorDialog ()
      : this (Colors.Black)
    {

    }

    public ColorDialog (Color initialColor)
    {
      InitializeComponent ( );
      colorPicker.InitialColor = initialColor;

      PreviewKeyDown += HandleEsc;
    }

    /// <summary>
    /// Gets/sets the ColorDialog color.
    /// </summary>
    public Color SelectedColor
    {
      get 
      { 
        return (colorPicker.SelectedColor); 
      }
      set 
      { 
        colorPicker.InitialColor = value; 
      }
    }

    /// <summary>
    /// Gets selected color in hex
    /// </summary>
    public string SelectedColorHex
    {
      get
      {
        return (colorPicker.SelectedHexColor);
      }
    }

    private void btnSave_Click (object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void HandleEsc (object sender, KeyEventArgs e )
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }
  }
}
