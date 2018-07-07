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
  }
}
