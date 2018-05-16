using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// FontInfo
  /// </summary>
  public class FontInfo : NotifyMaster
  {
    private FontType _fontType;

    /// <summary>
    /// <see cref="FontType"/>
    /// </summary>
    public FontType FontType
    {
      get => _fontType;
      set
      {
        if ( Equals(value, _fontType) )
          return;

        _fontType = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// <see cref="FamilyTypeface"/>
    /// </summary>
    public FamilyTypeface Typeface
    {
      get
      {
        FamilyTypeface ftf = new FamilyTypeface
        {
          Stretch = FontType.FontStretch,
          Weight = FontType.FontWeight,
          Style = FontType.FontStyle
        };
        return ftf;
      }
    }

    /// <summary>
    /// Standarc constructor
    /// </summary>
    public FontInfo()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="family"><see cref="FontFamily"/></param>
    /// <param name="size">FontSize</param>
    /// <param name="style"><see cref="FontStyle"/></param>
    /// <param name="weight"><see cref="FontWeight"/></param>
    /// <param name="stretch"><see cref="FontStretch"/></param>
    public FontInfo(FontFamily family, double size, FontStyle style, FontWeight weight, FontStretch stretch) => FontType = new FontType
    {
      FontFamily = family,
      FontSize = size,
      FontStyle = style,
      FontWeight = weight,
      FontStretch = stretch
    };

    #region Static Utils

    /// <summary>
    /// <see cref="FamilyTypeface"/> to string
    /// </summary>
    /// <param name="ttf"><see cref="FamilyTypeface"/></param>
    /// <returns><see cref="FamilyTypeface"/> as string</returns>
    public static string TypefaceToString(FamilyTypeface ttf)
    {
      var sb = new StringBuilder(ttf.Stretch.ToString());
      sb.Append(" / ");
      sb.Append(ttf.Weight.ToString());
      sb.Append(" / ");
      sb.Append(ttf.Style.ToString());

      return sb.ToString();
    }

    /// <summary>
    /// ApplyFont
    /// </summary>
    /// <param name="control"><see cref="Control"/></param>
    /// <param name="font"><see cref="FontInfo"/></param>
    public static void ApplyFont(Control control, FontInfo font)
    {
      control.FontFamily = font.FontType.FontFamily;
      control.FontSize = font.FontType.FontSize;
      control.FontStyle = font.FontType.FontStyle;
      control.FontWeight = font.FontType.FontWeight;
      control.FontStretch = font.FontType.FontStretch;
    }

    /// <summary>
    /// GetControlFont
    /// </summary>
    /// <param name="control"><see cref="Control"/></param>
    /// <returns><see cref="FontInfo"/></returns>
    public static FontInfo GetControlFont(Control control)
    {
      FontInfo font = new FontInfo
      {
        FontType = new FontType
        {
          FontFamily = control.FontFamily,
          FontSize = control.FontSize,
          FontStyle = control.FontStyle,
          FontWeight = control.FontWeight,
          FontStretch = control.FontStretch
        }
      };
      return font;
    }

    #endregion
  }
}
