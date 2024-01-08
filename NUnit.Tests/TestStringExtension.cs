using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestStringExtension
  {
    [Test]
    public void TestStringToBool()
    {
      ClassicAssert.IsInstanceOf<bool>("True".ConvertToBool());
      ClassicAssert.AreEqual(false, "blabla".ConvertToBool());
      ClassicAssert.AreEqual(true, "100".ConvertToBool(true));
      ClassicAssert.AreEqual(false, ((string) null).ConvertToBool());
    }

    [Test]
    public void TestStringToInt()
    {
      ClassicAssert.AreEqual(10, "10".ConvertToInt());
      ClassicAssert.AreEqual(-1, "True".ConvertToInt());
      ClassicAssert.AreEqual(-10, "True".ConvertToInt(-10));
    }

    [Test]
    public void TestStringToDouble()
    {
      ClassicAssert.AreEqual(3.2d, "3.2".ConvertToDouble());
      ClassicAssert.AreEqual(double.NaN, "True".ConvertToDouble());
    }

    [Test]
    public void TestStringToFloat()
    {
      ClassicAssert.AreEqual(3.45f, "3.45".ConvertToFloat());
      ClassicAssert.AreEqual(float.NaN, "True".ConvertToFloat());
    }

    [Test]
    public void TestStringToDecimal()
    {
      ClassicAssert.AreEqual(3.56m, "3.56".ConvertToDecimal());
      ClassicAssert.AreEqual(decimal.MinValue, "blabla".ConvertToDecimal());
    }

    [Test]
    public void TestStringToThreeStateBool()
    {
      ClassicAssert.IsInstanceOf<bool>("True".ConvertToThreeStateBool());
      ClassicAssert.IsFalse("False".ConvertToThreeStateBool());
      ClassicAssert.IsNull("".ConvertToThreeStateBool());
      ClassicAssert.IsNull("blablabla".ConvertToThreeStateBool());
    }

    [Test]
    public void TestStringMeasureAndCutIt()
    {
      var fontFamily = new FontFamily("Tahoma");
      var typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretch.FromOpenTypeStretch(1));

      ClassicAssert.AreEqual("hello world", "hello world".MeasureTextAndCutIt(typeface, 12f, 150));

      string item = "Hello world this a long test, please cut it.".MeasureTextAndCutIt(typeface, 12f, 150);
      ClassicAssert.AreEqual("Hello world this a long...", item);
    }
  }
}
