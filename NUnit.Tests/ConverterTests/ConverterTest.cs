using System.Globalization;
using System.Windows.Media;
using NUnit.Framework;
using Org.Vs.TailForWin.UI.Converters;


namespace Org.Vs.NUnit.Tests.ConverterTests
{
  [TestFixture]
  public class ConverterTest
  {
    [Test]
    public void TestColorToBrushConverter()
    {
      var converter = new ColorToSolidColorBrushConverter();
      var convertedBrush = converter.Convert(Colors.Blue, typeof(Brush), null, CultureInfo.CurrentCulture);
      Assert.That(convertedBrush is SolidColorBrush);
      Assert.AreEqual("#FF0000FF", convertedBrush.ToString());
    }

    [Test]
    public void TestInvserseNullToBoolConverter()
    {
      var converter = new InverseNullToBoolConverter();
      var convertedValue = converter.Convert(null, null, null, CultureInfo.CurrentCulture);
      Assert.That(convertedValue is bool);
      Assert.AreEqual(false, convertedValue);
    }

    [Test]
    public void TestStringToIntConverter()
    {
      var converter = new StringToIntConverter();
      var convertedInt = converter.Convert("test", typeof(int), null, CultureInfo.CurrentCulture);
      Assert.IsNull(convertedInt);

      convertedInt = converter.Convert(-1, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.That(convertedInt is int);
      Assert.AreEqual(0, convertedInt);

      convertedInt = converter.Convert(1080, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.AreEqual(1080, convertedInt);
    }
  }
}
