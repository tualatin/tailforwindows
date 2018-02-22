using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


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
    public async Task TestSettingsHelperReadSettingsAsync()
    {
      await _currentSettings.ReadSettingsAsync().ConfigureAwait(false);

      Assert.AreEqual(EUiLanguage.English, SettingsHelperController.CurrentSettings.Language);
      //Assert.IsTrue(SettingsHelper.CurrentSettings.SmartWatch);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysScrollToEnd);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AutoUpdate);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.DeleteLogFiles);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.GroupByCategory);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.ShowLineNumbers);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ShowNumberLineAtStart);

      Assert.IsFalse(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.ExitWithEscape);

      Assert.AreEqual(25, SettingsHelperController.CurrentSettings.LinesRead);
      //Assert.LessOrEqual(-1, SettingsHelper.CurrentSettings.LogLineLimit);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowWidth, -1);
      Assert.AreEqual(1274, SettingsHelperController.CurrentSettings.WindowWidth);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowHeight, -1);
      Assert.AreEqual(1080, SettingsHelperController.CurrentSettings.WindowHeight);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowPositionX, -1);
      Assert.AreEqual(2566, SettingsHelperController.CurrentSettings.WindowPositionX);
      Assert.Greater(SettingsHelperController.CurrentSettings.WindowPositionY, -1);
      Assert.AreEqual(0, SettingsHelperController.CurrentSettings.WindowPositionY);

      Assert.AreEqual(ETailRefreshRate.Normal, SettingsHelperController.CurrentSettings.DefaultRefreshRate);
      Assert.AreEqual(ThreadPriority.Normal, SettingsHelperController.CurrentSettings.DefaultThreadPriority);
      Assert.AreEqual(WindowState.Normal, SettingsHelperController.CurrentSettings.CurrentWindowState);
      Assert.AreEqual(EWindowStyle.ModernBlueWindowStyle, SettingsHelperController.CurrentSettings.CurrentWindowStyle);

      Assert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.StatusBarInactiveBackgroundColor.ToString());
      Assert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.StatusBarFileLoadedBackgroundColor.ToString());
      Assert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarTailBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.StatusBarTailBackgroundColor.ToString());

      Assert.AreEqual(ETimeFormat.HHMMD, SettingsHelperController.CurrentSettings.DefaultTimeFormat);
      Assert.AreEqual(EDateFormat.DDMMYYYY, SettingsHelperController.CurrentSettings.DefaultDateFormat);

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

      Assert.IsNotEmpty(SettingsHelperController.CurrentSettings.ProxySettings.UserName);
      Assert.AreEqual("testuser", SettingsHelperController.CurrentSettings.ProxySettings.UserName);
      Assert.NotNull(SettingsHelperController.CurrentSettings.ProxySettings.Password);
      Assert.IsNotEmpty(SettingsHelperController.CurrentSettings.ProxySettings.Password);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings);
      Assert.AreEqual(0, SettingsHelperController.CurrentSettings.ProxySettings.ProxyPort);
      Assert.IsEmpty(SettingsHelperController.CurrentSettings.ProxySettings.ProxyUrl);

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
    public async Task TestSettingsHelperSaveSettingsAsync()
    {
      SettingsHelperController.CurrentSettings.WindowPositionX = 200;
      SettingsHelperController.CurrentSettings.WindowPositionY = 150;
      SettingsHelperController.CurrentSettings.AlwaysOnTop = true;
      SettingsHelperController.CurrentSettings.ExitWithEscape = true;
      SettingsHelperController.CurrentSettings.RestoreWindowSize = true;
      SettingsHelperController.CurrentSettings.CurrentWindowState = WindowState.Maximized;
      SettingsHelperController.CurrentSettings.SaveWindowPosition = true;
      SettingsHelperController.CurrentSettings.Language = EUiLanguage.German;
      SettingsHelperController.CurrentSettings.AlwaysScrollToEnd = true;
      SettingsHelperController.CurrentSettings.ShowLineNumbers = true;
      SettingsHelperController.CurrentSettings.ShowNumberLineAtStart = true;
      SettingsHelperController.CurrentSettings.LinesRead = 100;
      SettingsHelperController.CurrentSettings.GroupByCategory = false;

      await _currentSettings.SaveSettingsAsync().ConfigureAwait(false);

      SettingsHelperController.CurrentSettings.WindowPositionX = DefaultEnvironmentSettings.WindowPositionX;
      SettingsHelperController.CurrentSettings.WindowPositionY = DefaultEnvironmentSettings.WindowPositionY;
      SettingsHelperController.CurrentSettings.AlwaysOnTop = DefaultEnvironmentSettings.AlwaysOnTop;
      SettingsHelperController.CurrentSettings.ExitWithEscape = DefaultEnvironmentSettings.ExitWithEscape;
      SettingsHelperController.CurrentSettings.RestoreWindowSize = DefaultEnvironmentSettings.RestoreWindowSize;
      SettingsHelperController.CurrentSettings.CurrentWindowState = default(WindowState);
      SettingsHelperController.CurrentSettings.SaveWindowPosition = DefaultEnvironmentSettings.SaveWindowPosition;
      SettingsHelperController.CurrentSettings.Language = DefaultEnvironmentSettings.Language;
      SettingsHelperController.CurrentSettings.AlwaysScrollToEnd = DefaultEnvironmentSettings.AlwaysScrollToEnd;
      SettingsHelperController.CurrentSettings.ShowLineNumbers = DefaultEnvironmentSettings.ShowLineNumbers;
      SettingsHelperController.CurrentSettings.ShowNumberLineAtStart = DefaultEnvironmentSettings.ShowNumberLineAtStart;
      SettingsHelperController.CurrentSettings.LinesRead = DefaultEnvironmentSettings.LinesRead;
      SettingsHelperController.CurrentSettings.GroupByCategory = DefaultEnvironmentSettings.GroupByCategory;

      await _currentSettings.ReloadCurrentSettingsAsync(new CancellationTokenSource()).ConfigureAwait(false);
      await _currentSettings.ReadSettingsAsync().ConfigureAwait(false);

      Assert.AreEqual(200, SettingsHelperController.CurrentSettings.WindowPositionX);
      Assert.AreEqual(150, SettingsHelperController.CurrentSettings.WindowPositionY);
      Assert.AreEqual(WindowState.Maximized, SettingsHelperController.CurrentSettings.CurrentWindowState);

      Assert.AreEqual(100, SettingsHelperController.CurrentSettings.LinesRead);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ExitWithEscape);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysScrollToEnd);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ShowLineNumbers);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ShowNumberLineAtStart);
      Assert.AreEqual(EUiLanguage.German, SettingsHelperController.CurrentSettings.Language);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.GroupByCategory);
    }

    [Test]
    public async Task TestSettingsHelperAddSettingsToConfigAsync()
    {
      Dictionary<string, string> newSetting = new Dictionary<string, string>
      {
        { "TestKey", "TestValue" }
      };

      await _currentSettings.AddNewPropertyAsync(newSetting, new CancellationTokenSource(TimeSpan.FromMinutes(1))).ConfigureAwait(false);
      await _currentSettings.ReadSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(1))).ConfigureAwait(false);

      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

      Assert.IsTrue(config.AppSettings.Settings.AllKeys.Contains("TestKey"));
      Assert.IsFalse(config.AppSettings.Settings.AllKeys.Contains("blablabla"));

      await _currentSettings.AddNewPropertyAsync(newSetting, new CancellationTokenSource(TimeSpan.FromMinutes(1))).ConfigureAwait(false);
      await _currentSettings.ReadSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(1))).ConfigureAwait(false);

      config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

      var count = config.AppSettings.Settings.AllKeys.Where(p => p.Equals("TestKey")).ToList();
      Assert.AreEqual(1, count.Count);
    }
  }
}
