using NUnit.Framework;
using Org.Vs.NUnit.Tests.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestEnumExtension
  {
    [Test]
    public void TestEnumExtensionGetEnumDescription()
    {
      ETestGetDescription description = ETestGetDescription.Test1;

      var myValue = description.GetEnumDescription();

      Assert.IsNotNull(myValue);
      Assert.AreEqual("Enum description test", myValue);

      description = ETestGetDescription.Test2;

      myValue = description.GetEnumDescription();

      Assert.NotNull(myValue);
      Assert.AreEqual("Enum description test 2", myValue);
    }

    [Test]
    public void TestEnumExtensionGetEnumByDescription()
    {
      string strDescription = "Enum description test 2";
      var eTestDescription = strDescription.GetEnumByDescription<ETestGetDescription>();

      Assert.AreEqual(ETestGetDescription.Test2, eTestDescription);
    }
  }
}
