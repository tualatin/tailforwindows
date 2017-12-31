using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestSettingsHelper
  {
    private ISettingsHelper _currentSettings;

    [SetUp]
    protected void SetUp()
    {
      _currentSettings = new SettingsHelperController();
    }

    [Test]
    public async Task TestSettingsHelperReadSettings()
    {
      await _currentSettings.ReadSettingsAsync().ConfigureAwait(false);

      //Assert.IsTrue(SettingsHelper.CurrentSettings.SmartWatch);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.AlwaysScrollToEnd);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.AutoUpdate);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.DeleteLogFiles);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.GroupByCategory);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.ShowLineNumbers);

      Assert.IsFalse(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.ExitWithEscape);

      //Assert.AreEqual(25, SettingsHelper.CurrentSettings.LinesRead);
      //Assert.LessOrEqual(-1, SettingsHelper.CurrentSettings.LogLineLimit);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowWidth, -1);
      Assert.AreEqual(1274, SettingsHelperController.CurrentSettings.WindowWidth);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowHeight, -1);
      Assert.AreEqual(1080, SettingsHelperController.CurrentSettings.WindowHeight);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowPositionX, -1);
      Assert.AreEqual(2566, SettingsHelperController.CurrentSettings.WindowPositionX);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowPositionY, -1);
      Assert.AreEqual(0, SettingsHelperController.CurrentSettings.WindowPositionY);

      //Assert.AreEqual(ETailRefreshRate.Normal, SettingsHelper.CurrentSettings.DefaultRefreshRate);
      //Assert.AreEqual(System.Threading.ThreadPriority.Normal, SettingsHelper.CurrentSettings.DefaultThreadPriority);
      Assert.AreEqual(System.Windows.WindowState.Normal, SettingsHelperController.CurrentSettings.CurrentWindowState);
      //Assert.AreEqual(EWindowStyle.ModernBlueWindowStyle, SettingsHelper.CurrentSettings.CurrentWindowStyle);

      //Assert.AreEqual(ETimeFormat.HHMMSSD, SettingsHelper.CurrentSettings.DefaultTimeFormat);
      //Assert.AreEqual(EDateFormat.DDMMYYYY, SettingsHelper.CurrentSettings.DefaultDateFormat);

      //Assert.AreEqual("#FFFFFFFF", SettingsHelper.CurrentSettings.DefaultForegroundColor);
      //Assert.AreEqual("#FF001825", SettingsHelper.CurrentSettings.DefaultBackgroundColor);
      //Assert.AreEqual("#000000", SettingsHelper.CurrentSettings.DefaultInactiveForegroundColor);
      //Assert.AreEqual("#FFFCFAF5", SettingsHelper.CurrentSettings.DefaultInactiveBackgroundColor);
      //Assert.AreEqual("#000000", SettingsHelper.CurrentSettings.DefaultHighlightForegroundColor);
      //Assert.AreEqual("#FFCC00", SettingsHelper.CurrentSettings.DefaultHighlightBackgroundColor);
      //Assert.AreEqual("#FF41A1FF", SettingsHelper.CurrentSettings.DefaultLineNumbersColor);
      //Assert.AreEqual("#FF3B7FFE", SettingsHelper.CurrentSettings.DefaultHighlightColor);

      //Assert.LessOrEqual(SettingsHelper.CurrentSettings.SearchWndXPos, -1);
      //Assert.LessOrEqual(SettingsHelper.CurrentSettings.SearchWndYPos, -1);

      //Assert.AreEqual(EFileSort.FileCreationTime, SettingsHelper.CurrentSettings.DefaultFileSort);

      //Assert.IsNotEmpty(SettingsHelper.CurrentSettings.ProxySettings.UserName);
      //Assert.AreEqual("testuser", SettingsHelper.CurrentSettings.ProxySettings.UserName);
      //Assert.NotNull(SettingsHelper.CurrentSettings.ProxySettings.Password);
      //Assert.IsNotEmpty(SettingsHelper.CurrentSettings.ProxySettings.Password);
      //Assert.IsFalse(SettingsHelper.CurrentSettings.ProxySettings.UseProxy);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.ProxySettings.UseSystemSettings);
      //Assert.AreEqual(0, SettingsHelper.CurrentSettings.ProxySettings.ProxyPort);
      //Assert.IsEmpty(SettingsHelper.CurrentSettings.ProxySettings.ProxyUrl);

      //Assert.IsFalse(SettingsHelper.CurrentSettings.AlertSettings.BringToFront);
      //Assert.IsFalse(SettingsHelper.CurrentSettings.AlertSettings.PlaySoundFile);
      //Assert.IsFalse(SettingsHelper.CurrentSettings.AlertSettings.SendEMail);
      //Assert.AreEqual("NoFile", SettingsHelper.CurrentSettings.AlertSettings.SoundFileName);
      //Assert.AreEqual("NoMail", SettingsHelper.CurrentSettings.AlertSettings.EMailAddress);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.AlertSettings.PopupWnd);

      //Assert.IsEmpty(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.SmtpServerName);
      //Assert.AreEqual(-1, SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.SmtpPort);
      //Assert.IsEmpty(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.LoginName);
      //Assert.IsEmpty(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.Password);
      //Assert.IsEmpty(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.FromAddress);
      //Assert.IsEmpty(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.Subject);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.SSL);
      //Assert.IsFalse(SettingsHelper.CurrentSettings.AlertSettings.SmtpSettings.TLS);

      //Assert.IsTrue(SettingsHelper.CurrentSettings.SmartWatchData.FilterByExtension);
      //Assert.IsFalse(SettingsHelper.CurrentSettings.SmartWatchData.NewTab);
      //Assert.AreEqual(ESmartWatchMode.Auto, SettingsHelper.CurrentSettings.SmartWatchData.Mode);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.SmartWatchData.AutoRun);
    }

    [Test]
    public async Task TestSettingsHelperSaveSettings()
    {
      SettingsHelperController.CurrentSettings.WindowPositionX = 200;
      SettingsHelperController.CurrentSettings.WindowPositionY = 150;
      SettingsHelperController.CurrentSettings.AlwaysOnTop = true;
      SettingsHelperController.CurrentSettings.ExitWithEscape = true;
      SettingsHelperController.CurrentSettings.RestoreWindowSize = true;
      SettingsHelperController.CurrentSettings.CurrentWindowState = WindowState.Maximized;
      SettingsHelperController.CurrentSettings.SaveWindowPosition = true;

      await _currentSettings.SaveSettingsAsync().ConfigureAwait(false);

      SettingsHelperController.CurrentSettings.WindowPositionX = -1;
      SettingsHelperController.CurrentSettings.WindowPositionY = -1;
      SettingsHelperController.CurrentSettings.AlwaysOnTop = false;
      SettingsHelperController.CurrentSettings.ExitWithEscape = false;
      SettingsHelperController.CurrentSettings.RestoreWindowSize = false;
      SettingsHelperController.CurrentSettings.CurrentWindowState = default(WindowState);
      SettingsHelperController.CurrentSettings.SaveWindowPosition = false;

      await _currentSettings.ReloadCurrentSettingsAsync().ConfigureAwait(false);
      await _currentSettings.ReadSettingsAsync().ConfigureAwait(false);

      Assert.AreEqual(200, SettingsHelperController.CurrentSettings.WindowPositionX);
      Assert.AreEqual(150, SettingsHelperController.CurrentSettings.WindowPositionY);
      Assert.AreEqual(WindowState.Maximized, SettingsHelperController.CurrentSettings.CurrentWindowState);

      Assert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ExitWithEscape);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
    }
  }
}
