using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Ui.Utils.Converters;


namespace Org.Vs.NUnit.Tests.ConverterTests
{
  [TestFixture]
  public class ConverterTest
  {
    [SetUp]
    protected void SetUp()
    {
      if ( Application.Current == null )
        Application.LoadComponent(new Uri("/T4W;component/app.xaml", UriKind.Relative));
    }

    [Test]
    public void TestColorToBrushConverter()
    {
      var converter = new ColorToSolidColorBrushConverter();
      var convertedBrush = converter.Convert(Colors.Blue, typeof(Brush), null, CultureInfo.CurrentCulture);
      Assert.That(convertedBrush is SolidColorBrush);
      ClassicAssert.AreEqual("#FF0000FF", convertedBrush.ToString());
    }

    [Test]
    public void TestInvserseNullToBoolConverter()
    {
      var converter = new InverseNullToBoolConverter();
      var convertedValue = converter.Convert(null, null, null, CultureInfo.CurrentCulture);
      Assert.That(convertedValue is bool);
      ClassicAssert.AreEqual(false, convertedValue);
    }

    [Test]
    public void TestLogLineLimitConverter()
    {
      var converter = new LogLineLimitConverter();
      var convertedLogLineLimit = converter.Convert(20, typeof(int), null, CultureInfo.CurrentCulture);
      ClassicAssert.That(convertedLogLineLimit is int);
      ClassicAssert.AreEqual(20, convertedLogLineLimit);

      convertedLogLineLimit = converter.Convert("test", typeof(int), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual(EnvironmentContainer.UnlimitedLogLineValue, convertedLogLineLimit);

      convertedLogLineLimit = converter.ConvertBack(-1, typeof(int), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual(-1, convertedLogLineLimit);
    }

    [Test]
    public void TestStringToIntConverter()
    {
      var converter = new StringToIntConverter();
      var convertedInt = converter.Convert("test", typeof(int), null, CultureInfo.CurrentCulture);
      ClassicAssert.IsNull(convertedInt);

      convertedInt = converter.Convert(-1, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.That(convertedInt is int);
      ClassicAssert.AreEqual(0, convertedInt);

      convertedInt = converter.Convert(1080, typeof(int), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual(1080, convertedInt);
    }

    [Test]
    public void TestLogLineLimitToLabelConverter()
    {
      var converter = new LogLineLimitToLabelConverter();
      var labelText = converter.Convert(40000D, typeof(string), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual("40.000 lines", labelText);

      labelText = converter.Convert(-1D, typeof(string), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual("Unlimited", labelText);
    }

    [Test]
    public void TestStringToWindowMediaBrushConverter()
    {
      var converter = new StringToWindowMediaBrushConverter();
      var convertedBrush = converter.Convert("#FFFFFF", typeof(Brush), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual(Brushes.White.ToString(), convertedBrush?.ToString());
    }

    [Test]
    public void TestBoolToUpdateHintConverter()
    {
      var converter = new BoolToUpdateHintConverter();
      var hint = converter.Convert(true, typeof(bool), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual("There is an update available", hint);

      hint = converter.Convert(false, typeof(bool), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual("No update necessary", hint);

      hint = converter.Convert("blabla", typeof(string), null, CultureInfo.CurrentCulture);
      ClassicAssert.AreEqual(string.Empty, hint);
    }
  }
}
