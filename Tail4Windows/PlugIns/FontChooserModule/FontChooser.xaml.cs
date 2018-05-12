using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.FontChooserModule
{
  /// <summary>
  /// Interaction logic for FontChooser.xaml
  /// </summary>
  public partial class FontChooser
  {
    /// <summary>
    /// SelectedFont <see cref="FontInfo"/>
    /// </summary>
    public FontInfo SelectedFont => new FontInfo
    {
      FontType = new FontType
      {
        FontFamily = TxtSampleText.FontFamily,
        FontStretch = TxtSampleText.FontStretch,
        FontWeight = TxtSampleText.FontWeight,
        FontStyle = TxtSampleText.FontStyle,
        FontSize = TxtSampleText.FontSize
      }
    };

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FontChooser() => InitializeComponent();
  }
}
