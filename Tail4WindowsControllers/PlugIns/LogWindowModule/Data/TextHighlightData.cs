using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Data
{
  /// <summary>
  /// Text highlight data
  /// </summary>
  public class TextHighlightData : NotifyMaster
  {
    private string _text;

    /// <summary>
    /// Text
    /// </summary>
    public string Text
    {
      get => _text;
      set
      {
        if ( Equals(value, _text) )
          return;

        _text = value;
        OnPropertyChanged();
      }
    }

    private string _textHighlightColorHex;

    /// <summary>
    /// Text highlight color as hex string
    /// </summary>
    public string TextHighlightColorHex
    {
      get => _textHighlightColorHex;
      set
      {
        if ( Equals(value, _textHighlightColorHex) )
          return;

        _textHighlightColorHex = value;
        OnPropertyChanged();
      }
    }

    private string _oldTextHighlightColorHex;

    /// <summary>
    /// Old text highlight color as hex string
    /// </summary>
    public string OldTextHighlightColorHex
    {
      get => _oldTextHighlightColorHex;
      set
      {
        if (Equals(value, _oldTextHighlightColorHex))
          return;

        _oldTextHighlightColorHex = value;
        OnPropertyChanged();
      }
    }

    private string _textBackgroundColorHex;

    /// <summary>
    /// Text background color as hex string
    /// </summary>
    public string TextBackgroundColorHex
    {
      get => _textBackgroundColorHex;
      set
      {
        if (Equals(value, _textBackgroundColorHex))
          return;

        _textBackgroundColorHex = value;
        OnPropertyChanged();
      }
    }

    private bool _isFindWhat;

    /// <summary>
    /// Is FindWhat result
    /// </summary>
    public bool IsFindWhat
    {
      get => _isFindWhat;
      set
      {
        if (value == _isFindWhat)
          return;

        _isFindWhat = value;
        OnPropertyChanged();
      }
    }

    private double _opacity;

    /// <summary>
    /// Opacity
    /// </summary>
    public double Opacity
    {
      get => _opacity;
      set
      {
        if (Equals(value, _opacity))
          return;

        _opacity = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TextHighlightData() => Opacity = 1.0;
  }
}
