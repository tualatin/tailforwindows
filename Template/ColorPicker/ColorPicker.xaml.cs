using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Org.Vs.TailForWin.Template.ColorPicker
{
  /// <summary>
  /// A simple WPF color picker.  The basic idea is to use a Color swatch image and then pick out a single
  /// pixel and use that pixel's RGB values along with the Alpha slider to form a SelectedColor.
  /// 
  /// This class is from Sacha Barber at http://sachabarber.net/?p=424 and http://www.codeproject.com/KB/WPF/WPFColorPicker.aspx.
  /// 
  /// This class borrows an idea or two from the following sources:
  ///  - AlphaSlider and Preview box; Based on an article by ShawnVN's Blog; 
  ///    http://weblogs.asp.net/savanness/archive/2006/12/05/colorcomb-yet-another-color-picker-dialog-for-wpf.aspx.
  ///  - 1*1 pixel copy; Based on an article by Lee Brimelow; http://thewpfblog.com/?p=62.
  /// 
  /// Enhanced by Mark Treadwell (1/2/10):
  ///  - Left click to select the color with no mouse move
  ///  - Set tab behavior
  ///  - Set an initial color (note that the search to set the cursor ellipse delays the initial display)
  ///  - Fix single digit hex displays
  ///  - Add Mouse Wheel support to change the Alpha value
  ///  - Modify color select dragging behavior
  /// </summary>     
  public partial class ColorPicker
  {
    private readonly DrawingAttributes drawingAttributes = new DrawingAttributes();
    private Color selectedColor = Colors.Transparent;
    private bool isMouseDown;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorPicker ()
      : this(Colors.Black)
    {
    }

    /// <summary>
    /// Constrcutor
    /// </summary>
    /// <param name="initialColor">Initialize color</param>
    public ColorPicker (Color initialColor)
    {
      InitializeComponent();
      selectedColor = initialColor;
    }

    #region Public Properties

    /// <summary>
    /// Gets or privately sets the Selected Color.
    /// </summary>
    public Color SelectedColor
    {
      get
      {
        return (selectedColor);
      }
      private set
      {
        if (selectedColor == value)
          return;

        selectedColor = value;
        CreateAlphaLinearBrush();
        UpdateTextBoxes();
        UpdateInk();
      }
    }

    /// <summary>
    /// Sets the initial Selected Color.
    /// </summary>
    public Color InitialColor
    {
      set
      {
        SelectedColor = value;
        CreateAlphaLinearBrush();
        AlphaSlider.Value = value.A;
        UpdateCursorEllipse(value);
      }
    }

    /// <summary>
    /// Gets selected color in hex
    /// </summary>
    public string SelectedHexColor
    {
      get
      {
        return (txtAll.Text);
      }
    }

    #endregion

    #region HelperFunctions

    /// <summary>
    /// Creates a new LinearGradientBrush background for the Alpha area slider.  This is based on the current color.
    /// </summary>
    private void CreateAlphaLinearBrush ()
    {
      Color startColor = Color.FromArgb(0, SelectedColor.R, SelectedColor.G, SelectedColor.B);
      Color endColor = Color.FromArgb(255, SelectedColor.R, SelectedColor.G, SelectedColor.B);
      LinearGradientBrush alphaBrush = new LinearGradientBrush(startColor, endColor, new Point(0, 0), new Point(1, 0));
      AlphaBorder.Background = alphaBrush;
    }

    /// <summary>
    /// Update the mouse cursor ellipse position.
    /// </summary>
    private void UpdateCursorEllipse (Color searchColor)
    {
      // Scan the canvas image for a color which matches the search color
      Color tempColor = new Color();
      byte[] pixels = new byte[4];
      int searchY;
      int searchX = 0;
      searchColor.A = 255;

      for (searchY = 0; searchY <= canvasImage.Width - 1; searchY++)
      {
        for (searchX = 0; searchX <= canvasImage.Height - 1; searchX++)
        {
          CroppedBitmap cb = new CroppedBitmap(ColorImage.Source as BitmapSource, new Int32Rect(searchX, searchY, 1, 1));
          cb.CopyPixels(pixels, 4, 0);
          tempColor = Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);

          if (tempColor == searchColor)
            break;
        }

        if (tempColor == searchColor)
          break;
      }

      // Default to the top left if no match is found
      if (tempColor != searchColor)
      {
        searchX = 0;
        searchY = 0;
      }

      // Update the mouse cursor ellipse position
      ellipsePixel.SetValue(Canvas.LeftProperty, (searchX - (ellipsePixel.Width / 2.0)));
      ellipsePixel.SetValue(Canvas.TopProperty, (searchY - (ellipsePixel.Width / 2.0)));
    }

    /// <summary>
    /// Sets a new Selected Color based on the color of the pixel under the mouse pointer.
    /// </summary>
    private void UpdateColor ()
    {
      // Test to ensure we do not get bad mouse positions along the edges
      int imageX = (int) Mouse.GetPosition(canvasImage).X;
      int imageY = (int) Mouse.GetPosition(canvasImage).Y;

      if ((imageX < 0) || (imageY < 0) || (imageX > ColorImage.Width - 1) || (imageY > ColorImage.Height - 1))
        return;

      // Get the single pixel under the mouse into a bitmap and copy it to a byte array
      CroppedBitmap cb = new CroppedBitmap(ColorImage.Source as BitmapSource, new Int32Rect(imageX, imageY, 1, 1));
      byte[] pixels = new byte[4];
      cb.CopyPixels(pixels, 4, 0);

      // Update the mouse cursor position and the Selected Color
      ellipsePixel.SetValue(Canvas.LeftProperty, Mouse.GetPosition(canvasImage).X - (ellipsePixel.Width / 2.0));
      ellipsePixel.SetValue(Canvas.TopProperty, Mouse.GetPosition(canvasImage).Y - (ellipsePixel.Width / 2.0));
      canvasImage.InvalidateVisual();

      // Set the Selected Color based on the cursor pixel and Alpha Slider value
      SelectedColor = Color.FromArgb((byte) AlphaSlider.Value, pixels[2], pixels[1], pixels[0]);
    }

    /// <summary>
    /// Update text box values based on the Selected Color.
    /// </summary>
    private void UpdateTextBoxes ()
    {
      txtAlpha.Text = SelectedColor.A.ToString(CultureInfo.InvariantCulture);
      txtAlphaHex.Text = SelectedColor.A.ToString("X2");
      txtRed.Text = SelectedColor.R.ToString(CultureInfo.InvariantCulture);
      txtRedHex.Text = SelectedColor.R.ToString("X2");
      txtGreen.Text = SelectedColor.G.ToString(CultureInfo.InvariantCulture);
      txtGreenHex.Text = SelectedColor.G.ToString("X2");
      txtBlue.Text = SelectedColor.B.ToString(CultureInfo.InvariantCulture);
      txtBlueHex.Text = SelectedColor.B.ToString("X2");
      txtAll.Text = string.Format("#{0}{1}{2}{3}", txtAlphaHex.Text, txtRedHex.Text, txtGreenHex.Text, txtBlueHex.Text);
    }

    /// <summary>
    /// Updates the Ink strokes based on the Selected Color.
    /// </summary>
    private void UpdateInk ()
    {
      drawingAttributes.Color = SelectedColor;
      drawingAttributes.StylusTip = StylusTip.Ellipse;
      drawingAttributes.Width = 5;

      // Update drawing attributes on previewPresenter
      foreach (Stroke s in previewPresenter.Strokes)
      {
        s.DrawingAttributes = drawingAttributes;
      }
    }

    #endregion

    #region Events

    /// <summary>
    /// 
    /// </summary>
    private void AlphaSlider_MouseWheel (object sender, MouseWheelEventArgs e)
    {
      int change = e.Delta / Math.Abs(e.Delta);
      AlphaSlider.Value = AlphaSlider.Value + change;
    }

    /// <summary>
    /// Update SelectedColor Alpha based on Slider value.
    /// </summary>
    private void AlphaSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      SelectedColor = Color.FromArgb((byte) AlphaSlider.Value, SelectedColor.R, SelectedColor.G, SelectedColor.B);
    }

    /// <summary>
    /// Update the SelectedColor if moving the mouse with the left button down.
    /// </summary>
    private void CanvasImage_MouseMove (object sender, MouseEventArgs e)
    {
      if (isMouseDown)
        UpdateColor();
    }

    /// <summary>
    /// Handle MouseDown event.
    /// </summary>
    private void CanvasImage_MouseDown (object sender, MouseButtonEventArgs e)
    {
      isMouseDown = true;
      UpdateColor();
    }

    /// <summary>
    /// Handle MouseUp event.
    /// </summary>
    private void CanvasImage_MouseUp (object sender, MouseButtonEventArgs e)
    {
      isMouseDown = false;
    }

    /// <summary>
    /// Apply the new Swatch image based on user requested swatch.
    /// </summary>
    private void Swatch_MouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      Image img = (sender as Image);

      if (img != null)
        ColorImage.Source = img.Source;

      UpdateCursorEllipse(SelectedColor);
    }

    #endregion
  }
}
