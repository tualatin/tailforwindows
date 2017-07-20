using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;
using NUnit.Framework;
using Org.Vs.TailForWin.Converters;
using Org.Vs.TailForWin.Converters.MultiConverters;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Utils;
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

    [Test]
    public void Test_IntToBool_Converter()
    {
      var converter = new IntToBoolConverter();
      var convertedBool = converter.Convert(1, typeof(bool), null, CultureInfo.CurrentCulture);
      Assert.That(convertedBool is bool);
      Assert.IsTrue((bool) convertedBool);

      convertedBool = converter.Convert(0, typeof(bool), null, CultureInfo.CurrentCulture);
      Assert.IsFalse(convertedBool != null && (bool) convertedBool);
    }

    [Test]
    public void Test_GroupByCategory_Converter()
    {
      var converter = new GroupByCategoryContentConverter();
      var convertedGroup = converter.Convert("test", typeof(string), null, CultureInfo.CurrentCulture);
      Assert.That(convertedGroup is string);
      Assert.AreNotEqual("Ungroup", convertedGroup);

      convertedGroup = converter.Convert(true, typeof(string), null, CultureInfo.CurrentCulture);
      Assert.AreEqual("Ungroup", convertedGroup);
    }

    [Test]
    public void Test_LogLineLimit_Converter()
    {
      var converter = new LogLineLimitConverter();
      var convertedLogLineLimit = converter.Convert(20, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.That(convertedLogLineLimit is int);
      Assert.AreEqual(20, convertedLogLineLimit);

      convertedLogLineLimit = converter.Convert("test", typeof(int), null, CultureInfo.CurrentCulture);
      Assert.AreEqual(CentralManager.UNLIMITED_LOG_LINE_VALUE, convertedLogLineLimit);

      convertedLogLineLimit = converter.ConvertBack(-1, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.AreEqual(-1, convertedLogLineLimit);
    }

    [Test]
    public void Test_SmartWatchModeToBool_Converter()
    {
      var converter = new SmartWatchModeToBoolConverter();
      var convertedBool = converter.Convert("test", typeof(bool), null, CultureInfo.CurrentCulture);
      Assert.That(convertedBool is bool);
      Assert.IsFalse((bool) convertedBool);

      convertedBool = converter.Convert(ESmartWatchMode.Auto, typeof(bool), null, CultureInfo.CurrentCulture);
      Assert.IsTrue(convertedBool != null && (bool) convertedBool);
    }

    [Test]
    public void Test_StringToInt_Converter()
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

    [Test]
    public void Test_SmtpPort_Converter()
    {
      var converter = new SmtpPortConverter();
      var convertedPort = converter.Convert("test", typeof(int), null, CultureInfo.CurrentCulture);
      Assert.IsNull(convertedPort);

      convertedPort = converter.Convert(0, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.That(convertedPort is int);
      Assert.AreEqual(25, convertedPort);

      convertedPort = converter.Convert(587, typeof(int), null, CultureInfo.CurrentCulture);
      Assert.AreEqual(587, convertedPort);
    }

    [Test]
    public void Test_EnableUsePatternCheckBox_MultiConverter()
    {
      var converter = new EnableUsePatternCheckBoxMultiConverter();
      FileManagerData data = new FileManagerData
      {
        ID = new Guid(),
        Category = "for testing"
      };
      var objects = new object[] { data, @"tailforwindows_\d{4}-\d{2}-\d{2}.log" };
      var convertedBool = converter.Convert(objects, typeof(bool), null, CultureInfo.CurrentCulture);
      Assert.That(convertedBool is bool);
      Assert.IsTrue((bool) convertedBool);


      objects = new object[] { data, string.Empty };
      convertedBool = converter.Convert(objects, typeof(bool), null, CultureInfo.CurrentCulture);
      Assert.IsFalse((bool) convertedBool);
    }
  }
}