namespace Org.Vs.Tail4Win.Shared.Interfaces
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
