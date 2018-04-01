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
    protected void SetUp() => _currentSettings = new SettingsHelperController();

    [Test]
    public async Task TestSettingsHelperReadSettingsAsync()
    {
      await _currentSettings.ReadSettingsAsync().ConfigureAwait(false);

      Assert.AreEqual(EUiLanguage.English, SettingsHelperController.CurrentSettings.Language);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.SmartWatch);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysScrollToEnd);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AutoUpdate);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.DeleteLogFiles);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.GroupByCategory);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.ShowLineNumbers);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ShowNumberLineAtStart);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ActivateDragDropWindow);

      Assert.IsFalse(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.ExitWithEscape);

      Assert.AreEqual(25, SettingsHelperController.CurrentSettings.LinesRead);
      Assert.LessOrEqual(-1, SettingsHelperController.CurrentSettings.LogLineLimit);
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

      Assert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex);
      Assert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex);
      Assert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarTailBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex);

      Assert.AreEqual(ETimeFormat.HHMMD, SettingsHelperController.CurrentSettings.DefaultTimeFormat);
      Assert.AreEqual(EDateFormat.DDMMYYYY, SettingsHelperController.CurrentSettings.DefaultDateFormat);

      Assert.AreEqual("#FFFFFFFF", SettingsHelperController.CurrentSettings.ColorSettings.ForegroundColorHex);
      Assert.AreEqual("#FF001825", SettingsHelperController.CurrentSettings.ColorSettings.BackgroundColorHex);
      Assert.AreEqual("#000000", SettingsHelperController.CurrentSettings.ColorSettings.InactiveForegroundColorHex);
      Assert.AreEqual("#FFFCFAF5", SettingsHelperController.CurrentSettings.ColorSettings.InactiveBackgroundColorHex);
      Assert.AreEqual("#000000", SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightForegroundColorHex);
      Assert.AreEqual("#FFCC00", SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex);
      Assert.AreEqual("#FF41A1FF", SettingsHelperController.CurrentSettings.ColorSettings.LineNumberColorHex);
      Assert.AreEqual("#FF3B7FFE", SettingsHelperController.CurrentSettings.ColorSettings.LineNumberHighlightColorHex);

      Assert.AreEqual(EFileSort.FileCreationTime, SettingsHelperController.CurrentSettings.DefaultFileSort);

      Assert.IsNotEmpty(SettingsHelperController.CurrentSettings.ProxySettings.UserName);
      Assert.AreEqual("testuser", SettingsHelperController.CurrentSettings.ProxySettings.UserName);
      Assert.NotNull(SettingsHelperController.CurrentSettings.ProxySettings.Password);
      Assert.IsNotEmpty(SettingsHelperController.CurrentSettings.ProxySettings.Password);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings);
      Assert.AreEqual(0, SettingsHelperController.CurrentSettings.ProxySettings.ProxyPort);
      Assert.IsEmpty(SettingsHelperController.CurrentSettings.ProxySettings.ProxyUrl);

      Assert.IsFalse(SettingsHelperController.CurrentSettings.AlertSettings.BringToFront);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.AlertSettings.PlaySoundFile);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.AlertSettings.SendMail);
      Assert.AreEqual("NoFile", SettingsHelperController.CurrentSettings.AlertSettings.SoundFileName);
      Assert.AreEqual("NoMail", SettingsHelperController.CurrentSettings.AlertSettings.MailAddress);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.AlertSettings.PopupWnd);

      Assert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.SmtpServerName);
      Assert.AreEqual(-1, SettingsHelperController.CurrentSettings.SmtpSettings.SmtpPort);
      Assert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.LoginName);
      Assert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.Password);
      Assert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.FromAddress);
      Assert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.Subject);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SmtpSettings.Ssl);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.SmtpSettings.Tls);

      Assert.IsTrue(SettingsHelperController.CurrentSettings.SmartWatchSettings.FilterByExtension);
      Assert.IsFalse(SettingsHelperController.CurrentSettings.SmartWatchSettings.NewTab);
      Assert.AreEqual(ESmartWatchMode.Auto, SettingsHelperController.CurrentSettings.SmartWatchSettings.Mode);
      Assert.IsTrue(SettingsHelperController.CurrentSettings.SmartWatchSettings.AutoRun);
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
      SettingsHelperController.CurrentSettings.ActivateDragDropWindow = false;

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
      SettingsHelperController.CurrentSettings.ActivateDragDropWindow = DefaultEnvironmentSettings.ActivateDragDropWindow;

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
      Assert.IsFalse(SettingsHelperController.CurrentSettings.ActivateDragDropWindow);
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
