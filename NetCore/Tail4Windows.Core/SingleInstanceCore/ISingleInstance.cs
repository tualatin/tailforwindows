namespace Org.Vs.Tail4Win.Core.SingleInstanceCore
{
  public interface ISingleInstance
  {
    void OnInstanceInvoked(string[] args);
  }
}
