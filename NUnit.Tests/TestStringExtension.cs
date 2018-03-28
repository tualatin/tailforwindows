using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestStringExtension
  {
    [Test]
    public void TestStringToBool()
    {
      Assert.IsInstanceOf<bool>("True".ConvertToBool());
      Assert.AreEqual(false, "blabla".ConvertToBool());
      Assert.AreEqual(true, "100".ConvertToBool(true));
      Assert.AreEqual(false, ((string) null).ConvertToBool());
    }

    [Test]
    public void TestStringToInt()
    {
      Assert.AreEqual(10, "10".ConverToInt());
      Assert.AreEqual(-1, "True".ConverToInt());
      Assert.AreEqual(-10, "True".ConverToInt(-10));
    }

    [Test]
    public void TestStringToDouble()
    {
      Assert.AreEqual(3.2d, "3.2".ConvertToDouble());
      Assert.AreEqual(double.NaN, "True".ConvertToDouble());
    }

    [Test]
    public void TestStringToFloat()
    {
      Assert.AreEqual(3.45f, "3.45".ConvertToFloat());
      Assert.AreEqual(float.NaN, "True".ConvertToFloat());
    }

    [Test]
    public void TestStringToDecimal()
    {
      Assert.AreEqual(3.56m, "3.56".ConvertToDecimal());
      Assert.AreEqual(decimal.MinValue, "blabla".ConvertToDecimal());
    }

    [Test]
    public void TestStringToThreeStateBool()
    {
      Assert.IsInstanceOf<bool>("True".ConvertToThreeStateBool());
      Assert.IsFalse("False".ConvertToThreeStateBool());
      Assert.IsNull("".ConvertToThreeStateBool());
      Assert.IsNull("blablabla".ConvertToThreeStateBool());
    }

    [Test]
    public void TestStringMeasureAndCutIt()
    {
      var fontFamily = new FontFamily("Tahoma");
      var typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretch.FromOpenTypeStretch(1));

      Assert.AreEqual("hello world", "hello world".MeasureTextAndCutIt(typeface, 12f, 150));

      string item = "Hello world this a long test, please cut it.".MeasureTextAndCutIt(typeface, 12f, 150);
      Assert.AreEqual("Hello world this a long...", item);
    }
  }
}
