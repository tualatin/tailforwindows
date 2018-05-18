namespace Org.Vs.TailForWin.Core.Interfaces
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
