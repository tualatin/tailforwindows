using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestExecute
  {
    [Test]
    public void TestExecutionWithoutDispatcher()
    {
      const string message = "execution test...";
      Assert.Catch<InvalidOperationException>(() => Execute.BeginInvokeOnUiThread(() => Console.WriteLine(message)));

      SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
      Execute.InitializeWithSynchronizationContext(SynchronizationContext.Current);

      using ( var sw = new StringWriter() )
      {
        Console.SetOut(sw);
        Execute.InvokeOnUiThread(() => Console.Write(message));

        Assert.AreEqual(message, sw.ToString());
      }
    }
  }
}
