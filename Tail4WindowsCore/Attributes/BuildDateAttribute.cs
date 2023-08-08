using System;


namespace Org.Vs.TailForWin.Core.Attributes
{
  /// <summary>
  /// BuildDate attribute
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly)]
  public class BuildDateAttribute : Attribute
  {
    /// <summary>
    /// Date
    /// </summary>
    public string Date
    {
      get;
      set;
    }

    /// <summary>
    /// Get current build date
    /// </summary>
    /// <param name="date">Build date</param>
    public BuildDateAttribute(string date) => Date = date;
  }
}
