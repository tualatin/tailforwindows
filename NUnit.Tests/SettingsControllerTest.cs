using NUnit.Framework;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Interfaces;


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
      Assert.AreEqual(-1, SettingsHelper.TailSettings.LogLineLimit);
      Assert.AreEqual(1274, SettingsHelper.TailSettings.WndWidth);
      Assert.AreEqual(1080, SettingsHelper.TailSettings.WndHeight);
      Assert.AreEqual(2566, SettingsHelper.TailSettings.WndXPos);
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

      Assert.AreEqual(-1, SettingsHelper.TailSettings.SearchWndXPos);
      Assert.AreEqual(-1, SettingsHelper.TailSettings.SearchWndYPos);

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

      Assert.AreNotEqual("user", SettingsHelper.TailSettings.ProxySettings.UserName);

      Assert.AreNotEqual(10, SettingsHelper.TailSettings.LinesRead);
    }

    [Test]
    public void Test_SettingsController_Save_SearchWindowPosition()
    {
    }
  }
}
