using System;


namespace Org.Vs.TailForWin.Controllers.UI.Vml.Attributes
{
  /// <summary>
  /// Locator attribute
  /// </summary>
  public class LocatorAttribute : Attribute
  {
    /// <summary>
    /// Name of ViewModel
    /// </summary>
    public string Name
    {
      get; set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="name">Name of ViewModel</param>
    public LocatorAttribute(string name) => Name = name;
  }
}
