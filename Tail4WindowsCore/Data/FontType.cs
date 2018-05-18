using System.Windows;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// FontType
  /// </summary>
  public class FontType : NotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FontType()
    {
      FontSize = 12;
      FontStyle = FontStyles.Normal;
      FontWeight = FontWeights.Normal;
      FontFamily = new FontFamily("Consolas");
      FontStretch = FontStretches.Normal;
    }

    private double _fontSize;

    /// <summary>
    /// Font size
    /// </summary>
    public double FontSize
    {
      get => _fontSize;
      set
      {
        if ( Equals(value, _fontSize) )
          return;

        _fontSize = value;
        OnPropertyChanged();
      }
    }

    private FontStyle _fontStyle;

    /// <summary>
    /// FontStyle
    /// </summary>
    public FontStyle FontStyle
    {
      get => _fontStyle;
      set
      {
        if ( Equals(value, _fontStyle) )
          return;

        _fontStyle = value;
        OnPropertyChanged();
      }
    }

    private FontWeight _fontWeight;

    /// <summary>
    /// FontWeight
    /// </summary>
    public FontWeight FontWeight
    {
      get => _fontWeight;
      set
      {
        if ( Equals(value, _fontWeight) )
          return;

        _fontWeight = value;
        OnPropertyChanged();
      }
    }

    private FontFamily _fontFamily;

    /// <summary>
    /// FontFamily
    /// </summary>
    public FontFamily FontFamily
    {
      get => _fontFamily;
      set
      {
        if ( Equals(value, _fontFamily) )
          return;

        _fontFamily = value;
        OnPropertyChanged();
      }
    }

    private FontStretch _fontStretch;

    /// <summary>
    /// FontStretch
    /// </summary>
    public FontStretch FontStretch
    {
      get => _fontStretch;
      set
      {
        if ( Equals(value, _fontStretch) )
          return;

        _fontStretch = value;
        OnPropertyChanged();
      }
    }
  }
}
