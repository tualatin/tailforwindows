using System.ComponentModel;

// ReSharper disable InconsistentNaming


namespace Org.Vs.Tail4Win.Shared.Enums
{
  /// <summary>
  /// Enum date format
  /// </summary>
  public enum EDateFormat
  {
    /// <summary>
    /// Date format year - month - day
    /// </summary>
    [Description("yyyy-MM-dd")]
    YYYYMMDD,

    /// <summary>
    /// Short date format year - month - day
    /// </summary>
    [Description("yy-M-d")]
    YMDD,

    /// <summary>
    /// Date format day.month.year
    /// </summary>
    [Description("dd.MM.yyyy")]
    DDMMYYYY,

    /// <summary>
    /// Short date format day.month.year
    /// </summary>
    [Description("d.M.yy")]
    DMYY
  }
}
