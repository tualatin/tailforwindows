using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;


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

      ClassicAssert.AreEqual(EUiLanguage.English, SettingsHelperController.CurrentSettings.Language);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.SmartWatch);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysScrollToEnd);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.AutoUpdate);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.DeleteLogFiles);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.GroupByCategory);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.ShowLineNumbers);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.ShowNumberLineAtStart);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.ActivateDragDropWindow);

      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.ExitWithEscape);

      ClassicAssert.AreEqual(25, SettingsHelperController.CurrentSettings.LinesRead);
      ClassicAssert.LessOrEqual(-1, SettingsHelperController.CurrentSettings.LogLineLimit);
      ClassicAssert.Greater(SettingsHelperController.CurrentSettings.WindowWidth, -1);
      ClassicAssert.AreEqual(1274, SettingsHelperController.CurrentSettings.WindowWidth);
      ClassicAssert.Greater(SettingsHelperController.CurrentSettings.WindowHeight, -1);
      ClassicAssert.AreEqual(1080, SettingsHelperController.CurrentSettings.WindowHeight);
      ClassicAssert.Greater(SettingsHelperController.CurrentSettings.WindowPositionX, -1);
      ClassicAssert.AreEqual(2566, SettingsHelperController.CurrentSettings.WindowPositionX);
      ClassicAssert.Greater(SettingsHelperController.CurrentSettings.WindowPositionY, -1);
      ClassicAssert.AreEqual(0, SettingsHelperController.CurrentSettings.WindowPositionY);

      ClassicAssert.AreEqual(ETailRefreshRate.Normal, SettingsHelperController.CurrentSettings.DefaultRefreshRate);
      ClassicAssert.AreEqual(ThreadPriority.Normal, SettingsHelperController.CurrentSettings.DefaultThreadPriority);
      ClassicAssert.AreEqual(WindowState.Normal, SettingsHelperController.CurrentSettings.CurrentWindowState);
      ClassicAssert.AreEqual(EWindowStyle.ModernBlueWindowStyle, SettingsHelperController.CurrentSettings.CurrentWindowStyle);

      ClassicAssert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarInactiveBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex);
      ClassicAssert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarFileLoadedBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex);
      ClassicAssert.AreEqual(EnvironmentContainer.ConvertHexStringToBrush(DefaultEnvironmentSettings.StatusBarTailBackgroundColor).ToString(), SettingsHelperController.CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex);

      ClassicAssert.AreEqual(ETimeFormat.HHMMD, SettingsHelperController.CurrentSettings.DefaultTimeFormat);
      ClassicAssert.AreEqual(EDateFormat.DDMMYYYY, SettingsHelperController.CurrentSettings.DefaultDateFormat);

      ClassicAssert.AreEqual("#FFFFFFFF", SettingsHelperController.CurrentSettings.ColorSettings.ForegroundColorHex);
      ClassicAssert.AreEqual("#FF001825", SettingsHelperController.CurrentSettings.ColorSettings.BackgroundColorHex);
      ClassicAssert.AreEqual("#000000", SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightForegroundColorHex);
      ClassicAssert.AreEqual("#FFCC00", SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex);
      ClassicAssert.AreEqual("#FF41A1FF", SettingsHelperController.CurrentSettings.ColorSettings.LineNumberColorHex);
      ClassicAssert.AreEqual("#FF3B7FFE", SettingsHelperController.CurrentSettings.ColorSettings.LineNumberHighlightColorHex);

      ClassicAssert.AreEqual(EFileSort.FileCreationTime, SettingsHelperController.CurrentSettings.DefaultFileSort);

      ClassicAssert.IsNotEmpty(SettingsHelperController.CurrentSettings.ProxySettings.UserName);
      ClassicAssert.AreEqual("testuser", SettingsHelperController.CurrentSettings.ProxySettings.UserName);
      ClassicAssert.NotNull(SettingsHelperController.CurrentSettings.ProxySettings.Password);
      ClassicAssert.IsNotEmpty(SettingsHelperController.CurrentSettings.ProxySettings.Password);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.ProxySettings.UseSystemSettings);
      ClassicAssert.AreEqual(0, SettingsHelperController.CurrentSettings.ProxySettings.ProxyPort);
      ClassicAssert.IsEmpty(SettingsHelperController.CurrentSettings.ProxySettings.ProxyUrl);

      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.AlertSettings.BringToFront);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.AlertSettings.PlaySoundFile);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.AlertSettings.SendMail);
      ClassicAssert.AreEqual("NoFile", SettingsHelperController.CurrentSettings.AlertSettings.SoundFileName);
      ClassicAssert.AreEqual("NoMail", SettingsHelperController.CurrentSettings.AlertSettings.MailAddress);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.AlertSettings.PopupWnd);

      ClassicAssert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.SmtpServerName);
      ClassicAssert.AreEqual(-1, SettingsHelperController.CurrentSettings.SmtpSettings.SmtpPort);
      ClassicAssert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.LoginName);
      ClassicAssert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.Password);
      ClassicAssert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.FromAddress);
      ClassicAssert.IsEmpty(SettingsHelperController.CurrentSettings.SmtpSettings.Subject);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.SmtpSettings.Ssl);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.SmtpSettings.Tls);

      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.SmartWatchSettings.FilterByExtension);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.SmartWatchSettings.NewTab);
      ClassicAssert.AreEqual(ESmartWatchMode.Auto, SettingsHelperController.CurrentSettings.SmartWatchSettings.Mode);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.SmartWatchSettings.AutoRun);
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

      ClassicAssert.AreEqual(200, SettingsHelperController.CurrentSettings.WindowPositionX);
      ClassicAssert.AreEqual(150, SettingsHelperController.CurrentSettings.WindowPositionY);
      ClassicAssert.AreEqual(WindowState.Maximized, SettingsHelperController.CurrentSettings.CurrentWindowState);

      ClassicAssert.AreEqual(100, SettingsHelperController.CurrentSettings.LinesRead);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysOnTop);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.ExitWithEscape);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.RestoreWindowSize);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.SaveWindowPosition);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.AlwaysScrollToEnd);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.ShowLineNumbers);
      ClassicAssert.IsTrue(SettingsHelperController.CurrentSettings.ShowNumberLineAtStart);
      ClassicAssert.AreEqual(EUiLanguage.German, SettingsHelperController.CurrentSettings.Language);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.GroupByCategory);
      ClassicAssert.IsFalse(SettingsHelperController.CurrentSettings.ActivateDragDropWindow);
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

      ClassicAssert.IsTrue(config.AppSettings.Settings.AllKeys.Contains("TestKey"));
      ClassicAssert.IsFalse(config.AppSettings.Settings.AllKeys.Contains("blablabla"));

      await _currentSettings.AddNewPropertyAsync(newSetting, new CancellationTokenSource(TimeSpan.FromMinutes(1))).ConfigureAwait(false);
      await _currentSettings.ReadSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(1))).ConfigureAwait(false);

      config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

      var count = config.AppSettings.Settings.AllKeys.Where(p => p.Equals("TestKey")).ToList();
      ClassicAssert.AreEqual(1, count.Count);
    }
  }
}
