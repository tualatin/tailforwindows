using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using NUnit.Framework;
using Org.Vs.TailForWin.Converters;

using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class ConverterTest
  {
    [Test]
    public void Test_ColorToBrush_Converter()
    {
      var converter = new ColorToBrushConverter();
      var convertedBrush = converter.Convert(Color.Blue, typeof(Brush), null, CultureInfo.CurrentCulture);
      Assert.That(convertedBrush is SolidColorBrush);
    }

    [Test]
    public void Test_FontToString_Converter()
    {
      var converter = new FontToStringConverter();
      Font font = new Font("Tahoma", 10, FontStyle.Italic | FontStyle.Bold);
      var convertedFont = converter.Convert(font, typeof(string), null, CultureInfo.CurrentCulture);

      Assert.That(convertedFont is string);
      Assert.IsNotEmpty((string) convertedFont);
      Assert.AreEqual("Tahoma (10) Italic Bold", convertedFont);
    }
  }
}
