using System.Windows;


namespace Org.Vs.NUnit.Tests
{
  public class MockAppTest
  {
    private static Application _application = new Application
    {
      ShutdownMode = ShutdownMode.OnExplicitShutdown
    };
  }
}
