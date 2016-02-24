using System.ComponentModel;


namespace TailForWin.Data.Enums
{
  /// <summary>
  /// Enum time format
  /// </summary>
  public enum ETimeFormat
  {
    /// <summary>
    /// International time format like GB, USA ... hh:mm
    /// </summary>
    [Description ("hh:mm")]
    HHMMd,

    /// <summary>
    /// German standard time format HH:mm
    /// </summary>
    [Description ("HH:mm")]
    HHMMD,

    /// <summary>
    /// International time format like GB, USA with seconds hh:mm:ss
    /// </summary>
    [Description ("hh:mm:ss")]
    HHMMSSd,

    /// <summary>
    /// German standard time format with seconds HH:mm:ss
    /// </summary>
    [Description ("HH:mm:ss")]
    HHMMSSD
  }
}
