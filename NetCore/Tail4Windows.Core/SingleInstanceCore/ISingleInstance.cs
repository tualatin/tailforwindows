namespace Org.Vs.TailForWin.Core.SingleInstanceCore
{
  public interface ISingleInstance
  {
    void OnInstanceInvoked(string[] args);
  }
}
