using NUnit.Framework;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class SettingsControllerTest
  {
    private ISettingsHelper settings;

    [SetUp]
    protected void SetUp()
    {
      settings = new SettingsHelper();
    }

    [Test]
    public void Test_SettingsController_ReadSettings_Positive()
    {
      settings.ReadSettings();

      Assert.IsTrue(SettingsHelper.TailSettings.SmartWatch);
      Assert.IsTrue(SettingsHelper.TailSettings.AlwaysScrollToEnd);
      Assert.IsTrue(SettingsHelper.TailSettings.AutoUpdate);
      Assert.IsTrue(SettingsHelper.TailSettings.RestoreWindowSize);
      Assert.IsTrue(SettingsHelper.TailSettings.SaveWindowPosition);
      Assert.IsTrue(SettingsHelper.TailSettings.DeleteLogFiles);
      Assert.IsTrue(SettingsHelper.TailSettings.GroupByCategory);
      Assert.IsTrue(SettingsHelper.TailSettings.ShowLineNumbers);

      Assert.IsFalse(SettingsHelper.TailSettings.AlwaysOnTop);
      Assert.IsFalse(SettingsHelper.TailSettings.ExitWithEscape);

      Assert.AreEqual(25, SettingsHelper.TailSettings.LinesRead);
      Assert.LessOrEqual(-1, SettingsHelper.TailSettings.LogLineLimit);
      Assert.Greater(SettingsHelper.TailSettings.WndWidth, -1);
      Assert.AreEqual(1274, SettingsHelper.TailSettings.WndWidth);
      Assert.Greater(SettingsHelper.TailSettings.WndHeight, -1);
      Assert.AreEqual(1080, SettingsHelper.TailSettings.WndHeight);
      Assert.Greater(SettingsHelper.TailSettings.WndXPos, -1);
      Assert.AreEqual(2566, SettingsHelper.TailSettings.WndXPos);
      Assert.Greater(SettingsHelper.TailSettings.WndYPos, -1);
      Assert.AreEqual(0, SettingsHelper.TailSettings.WndYPos);

      Assert.AreEqual(ETailRefreshRate.Normal, SettingsHelper.TailSettings.DefaultRefreshRate);
      Assert.AreEqual(System.Threading.ThreadPriority.Normal, SettingsHelper.TailSettings.DefaultThreadPriority);
      Assert.AreEqual(System.Windows.WindowState.Normal, SettingsHelper.TailSettings.CurrentWindowState);
      Assert.AreEqual(EWindowStyle.ModernBlueWindowStyle, SettingsHelper.TailSettings.CurrentWindowStyle);

      Assert.AreEqual(ETimeFormat.HHMMSSD, SettingsHelper.TailSettings.DefaultTimeFormat);
      Assert.AreEqual(EDateFormat.DDMMYYYY, SettingsHelper.TailSettings.DefaultDateFormat);

      Assert.AreEqual("#FFFFFFFF", SettingsHelper.TailSettings.DefaultForegroundColor);
      Assert.AreEqual("#FF001825", SettingsHelper.TailSettings.DefaultBackgroundColor);
      Assert.AreEqual("#000000", SettingsHelper.TailSettings.DefaultInactiveForegroundColor);
      Assert.AreEqual("#FFFCFAF5", SettingsHelper.TailSettings.DefaultInactiveBackgroundColor);
      Assert.AreEqual("#000000", SettingsHelper.TailSettings.DefaultHighlightForegroundColor);
      Assert.AreEqual("#FFCC00", SettingsHelper.TailSettings.DefaultHighlightBackgroundColor);
      Assert.AreEqual("#FF41A1FF", SettingsHelper.TailSettings.DefaultLineNumbersColor);
      Assert.AreEqual("#FF3B7FFE", SettingsHelper.TailSettings.DefaultHighlightColor);

      Assert.LessOrEqual(SettingsHelper.TailSettings.SearchWndXPos, -1);
      Assert.LessOrEqual(SettingsHelper.TailSettings.SearchWndYPos, -1);

      Assert.AreEqual(EFileSort.FileCreationTime, SettingsHelper.TailSettings.DefaultFileSort);

      Assert.IsNotEmpty(SettingsHelper.TailSettings.ProxySettings.UserName);
      Assert.AreEqual("testuser", SettingsHelper.TailSettings.ProxySettings.UserName);
      Assert.NotNull(SettingsHelper.TailSettings.ProxySettings.Password);
      Assert.IsNotEmpty(SettingsHelper.TailSettings.ProxySettings.Password);
      Assert.IsFalse(SettingsHelper.TailSettings.ProxySettings.UseProxy);
      Assert.IsTrue(SettingsHelper.TailSettings.ProxySettings.UseSystemSettings);
      Assert.AreEqual(0, SettingsHelper.TailSettings.ProxySettings.ProxyPort);
      Assert.IsEmpty(SettingsHelper.TailSettings.ProxySettings.ProxyUrl);

      Assert.IsFalse(SettingsHelper.TailSettings.AlertSettings.BringToFront);
      Assert.IsFalse(SettingsHelper.TailSettings.AlertSettings.PlaySoundFile);
      Assert.IsFalse(SettingsHelper.TailSettings.AlertSettings.SendEMail);
      Assert.AreEqual("NoFile", SettingsHelper.TailSettings.AlertSettings.SoundFileName);
      Assert.AreEqual("NoMail", SettingsHelper.TailSettings.AlertSettings.EMailAddress);
      Assert.IsTrue(SettingsHelper.TailSettings.AlertSettings.PopupWnd);

      Assert.IsEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpServerName);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SmtpPort);
      Assert.IsEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.LoginName);
      Assert.IsEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Password);
      Assert.IsEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.FromAddress);
      Assert.IsEmpty(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.Subject);
      Assert.IsTrue(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.SSL);
      Assert.IsFalse(SettingsHelper.TailSettings.AlertSettings.SmtpSettings.TLS);

      Assert.IsTrue(SettingsHelper.TailSettings.SmartWatchData.FilterByExtension);
      Assert.IsFalse(SettingsHelper.TailSettings.SmartWatchData.NewTab);
      Assert.AreEqual(ESmartWatchMode.Auto, SettingsHelper.TailSettings.SmartWatchData.Mode);
      Assert.IsTrue(SettingsHelper.TailSettings.SmartWatchData.AutoRun);
    }

    [Test]
    public void Test_SettingsController_ReadSettings_Negative()
    {
      settings.ReadSettings();

      Assert.AreNotEqual(10, SettingsHelper.TailSettings.LinesRead);
      Assert.AreNotEqual(System.Windows.WindowState.Maximized, SettingsHelper.TailSettings.CurrentWindowState);
      Assert.AreNotEqual(CentralManager.DEFAULT_FOREGROUND_COLOR, SettingsHelper.TailSettings.DefaultForegroundColor);
      Assert.AreNotEqual(CentralManager.DEFAULT_BACKGROUND_COLOR, SettingsHelper.TailSettings.DefaultBackgroundColor);
      Assert.AreNotEqual("user", SettingsHelper.TailSettings.ProxySettings.UserName);
    }

    [Test]
    public void Test_SettingsController_Save_SearchWindowPosition()
    {
      SettingsHelper.TailSettings.SearchWndYPos = 200;
      SettingsHelper.TailSettings.SearchWndXPos = 300;

      settings.SaveSearchWindowPosition();

      SettingsHelper.TailSettings.SearchWndXPos = -1;
      SettingsHelper.TailSettings.SearchWndYPos = -1;

      settings.ReloadSettings();
      settings.ReadSettings();

      Assert.AreEqual(200, SettingsHelper.TailSettings.SearchWndYPos);
      Assert.AreEqual(300, SettingsHelper.TailSettings.SearchWndXPos);
    }

    [Test]
    public void Test_SettingsController_ResetToDefault()
    {
      settings.SetToDefault();
      settings.ReadSettings();

      Assert.IsFalse(SettingsHelper.TailSettings.AlwaysOnTop);
      Assert.IsFalse(SettingsHelper.TailSettings.RestoreWindowSize);
      Assert.IsFalse(SettingsHelper.TailSettings.SaveWindowPosition);
      Assert.IsFalse(SettingsHelper.TailSettings.AutoUpdate);
      Assert.IsFalse(SettingsHelper.TailSettings.SmartWatch);
      Assert.IsFalse(SettingsHelper.TailSettings.ShowLineNumbers);
      Assert.IsFalse(SettingsHelper.TailSettings.ExitWithEscape);

      Assert.IsTrue(SettingsHelper.TailSettings.AlwaysScrollToEnd);
      Assert.IsTrue(SettingsHelper.TailSettings.ShowNLineAtStart);
      Assert.IsTrue(SettingsHelper.TailSettings.GroupByCategory);
      Assert.IsTrue(SettingsHelper.TailSettings.DeleteLogFiles);

      Assert.AreEqual(10, SettingsHelper.TailSettings.LinesRead);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.WndHeight);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.WndWidth);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.WndXPos);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.WndYPos);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.SearchWndXPos);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.SearchWndYPos);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.LogLineLimit);

      Assert.AreEqual(System.Threading.ThreadPriority.Normal, SettingsHelper.TailSettings.DefaultThreadPriority);
      Assert.AreEqual(ETailRefreshRate.Normal, SettingsHelper.TailSettings.DefaultRefreshRate);
      Assert.AreEqual(ETimeFormat.HHMMD, SettingsHelper.TailSettings.DefaultTimeFormat);
      Assert.AreEqual(EDateFormat.DDMMYYYY, SettingsHelper.TailSettings.DefaultDateFormat);
      Assert.AreEqual(EFileSort.Nothing, SettingsHelper.TailSettings.DefaultFileSort);
      Assert.AreEqual(System.Windows.WindowState.Normal, SettingsHelper.TailSettings.CurrentWindowState);
      Assert.AreEqual(EWindowStyle.ModernBlueWindowStyle, SettingsHelper.TailSettings.CurrentWindowStyle);

      Assert.AreEqual(CentralManager.DEFAULT_BACKGROUND_COLOR, SettingsHelper.TailSettings.DefaultBackgroundColor);
      Assert.AreEqual(CentralManager.DEFAULT_FOREGROUND_COLOR, SettingsHelper.TailSettings.DefaultForegroundColor);
      Assert.AreEqual(CentralManager.DEFAULT_INACTIVE_BACKGROUND_COLOR, SettingsHelper.TailSettings.DefaultInactiveBackgroundColor);
      Assert.AreEqual(CentralManager.DEFAULT_INACTIVE_FOREGROUND_COLOR, SettingsHelper.TailSettings.DefaultInactiveForegroundColor);
      Assert.AreEqual(CentralManager.DEFAULT_FIND_HIGHLIGHT_BACKGROUND_COLOR, SettingsHelper.TailSettings.DefaultHighlightBackgroundColor);
      Assert.AreEqual(CentralManager.DEFAULT_FIND_HIGHLIGHT_FOREGROUND_COLOR, SettingsHelper.TailSettings.DefaultHighlightForegroundColor);
      Assert.AreEqual(CentralManager.DEFAULT_LINE_NUMBERS_COLOR, SettingsHelper.TailSettings.DefaultLineNumbersColor);
      Assert.AreEqual(CentralManager.DEFAULT_HIGHLIGHT_COLOR, SettingsHelper.TailSettings.DefaultHighlightColor);
    }
  }
}
