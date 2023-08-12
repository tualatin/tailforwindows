namespace Org.Vs.Tail4Win.Controllers.UI.Vml.Attributes
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
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="name">Name of ViewModel</param>
    public LocatorAttribute(string name) => Name = name;
  }
}
