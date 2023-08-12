namespace Org.Vs.Tail4Win.Core.Interfaces
{
  /// <summary>
  /// Property notification interface
  /// </summary>
  public interface IPropertyNotify
  {
    /// <summary>
    /// Call OnPropertyChanged
    /// </summary>
    void RaiseOnPropertyChanged();
  }
}
