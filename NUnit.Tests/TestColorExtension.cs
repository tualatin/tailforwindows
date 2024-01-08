using System.Windows.Media;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Extensions;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestColorExtension
  {
    [Test]
    public void TestColorToHex()
    {
      var background = new SolidColorBrush(Color.FromRgb(104, 33, 122));
      var color = System.Drawing.Color.FromArgb(background.Color.A, background.Color.R, background.Color.G, background.Color.B);
      string hexString = color.ToHexString();

      ClassicAssert.AreEqual(DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor, hexString);
    }
  }
}
