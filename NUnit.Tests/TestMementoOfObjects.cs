using NUnit.Framework;
using NUnit.Framework.Legacy;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestMementoOfObjects
  {
    private EnvironmentSettings _settings;

    [SetUp]
    protected void SetUp() =>
      _settings = new EnvironmentSettings
      {
        RestoreWindowSize = true,
        SaveWindowPosition = true,
        ShowNumberLineAtStart = true,
        LinesRead = 20,
        ExitWithEscape = true
      };

    [Test]
    public void TestMementoProxySettings()
    {
      var proxy = new ProxySetting
      {
        UseSystemSettings = false,
        UserName = "tail4windows",
        Password = "blablabla",
        ProxyPort = 8888,
        ProxyUrl = "myhostname.com"
      };

      _settings.ProxySettings = proxy;

      var memento = _settings.SaveToMemento();
      ClassicAssert.IsInstanceOf<EnvironmentSettings.MementoEnvironmentSettings>(memento);
      ClassicAssert.AreEqual(proxy.UseSystemSettings, memento.ProxySettings.UseSystemSettings);
      ClassicAssert.AreEqual(proxy.UserName, memento.ProxySettings.UserName);
      ClassicAssert.AreEqual(proxy.Password, memento.ProxySettings.Password);
      ClassicAssert.AreEqual(proxy.ProxyPort, memento.ProxySettings.ProxyPort);
      ClassicAssert.AreEqual(proxy.ProxyUrl, memento.ProxySettings.ProxyUrl);

      proxy.UseSystemSettings = null;
      proxy.ProxyPort = 4560;
      _settings.RestoreWindowSize = false;
      _settings.LinesRead = 40;
      _settings.ExitWithEscape = false;

      _settings.RestoreFromMemento(memento);
      ClassicAssert.AreEqual(memento.ProxySettings.UseSystemSettings, _settings.ProxySettings.UseSystemSettings);
      ClassicAssert.AreEqual(memento.ProxySettings.ProxyPort, _settings.ProxySettings.ProxyPort);
      ClassicAssert.AreEqual(memento.RestoreWindowSize, _settings.RestoreWindowSize);
      ClassicAssert.AreEqual(memento.LinesRead, _settings.LinesRead);
      ClassicAssert.AreEqual(memento.ExitWithEscape, _settings.ExitWithEscape);
    }

    [Test]
    public void TestMementoSmartWatchSettings()
    {
      var smartWatch = new SmartWatchSetting
      {
        AutoRun = true,
        NewTab = true,
        FilterByExtension = true,
        Mode = ESmartWatchMode.Manual
      };

      _settings.SmartWatchSettings = smartWatch;

      var memento = _settings.SaveToMemento();
      ClassicAssert.IsInstanceOf<EnvironmentSettings.MementoEnvironmentSettings>(memento);
      ClassicAssert.AreEqual(_settings.SmartWatchSettings.Mode, memento.SmartWatchSettings.Mode);
      ClassicAssert.AreEqual(_settings.SmartWatchSettings.AutoRun, memento.SmartWatchSettings.AutoRun);
      ClassicAssert.AreEqual(_settings.SmartWatchSettings.NewTab, memento.SmartWatchSettings.NewTab);
      ClassicAssert.AreEqual(_settings.SmartWatchSettings.FilterByExtension, memento.SmartWatchSettings.FilterByExtension);

      smartWatch.AutoRun = false;
      smartWatch.Mode = ESmartWatchMode.Auto;

      _settings.RestoreFromMemento(memento);
      ClassicAssert.AreEqual(memento.SmartWatchSettings.AutoRun, _settings.SmartWatchSettings.AutoRun);
      ClassicAssert.AreEqual(memento.SmartWatchSettings.Mode, _settings.SmartWatchSettings.Mode);
    }

    [Test]
    public void TestMementoAlertSettings()
    {
      var smtp = new SmtpSetting
      {
        FromAddress = "blabla@test.org",
        Tls = true,
        Subject = "Alert!",
        SmtpServerName = "hostname.test.local",
        LoginName = "testname",
        Password = "test",
        SmtpPort = 25
      };

      var alert = new AlertSetting
      {
        PopupWnd = true,
        MailAddress = "blablaTo@test.org",
        BringToFront = true,
        SendMail = true
      };

      _settings.AlertSettings = alert;
      _settings.SmtpSettings = smtp;

      var memento = _settings.SaveToMemento();
      ClassicAssert.IsInstanceOf<EnvironmentSettings.MementoEnvironmentSettings>(memento);
      ClassicAssert.AreEqual(_settings.AlertSettings.BringToFront, memento.AlertSettings.BringToFront);
      ClassicAssert.AreEqual(_settings.AlertSettings.MailAddress, memento.AlertSettings.MailAddress);
      ClassicAssert.AreEqual(_settings.AlertSettings.PopupWnd, memento.AlertSettings.PopupWnd);
      ClassicAssert.AreEqual(_settings.SmtpSettings.FromAddress, memento.SmtpSettings.FromAddress);
      ClassicAssert.AreEqual(_settings.SmtpSettings.LoginName, memento.SmtpSettings.LoginName);
      ClassicAssert.AreEqual(_settings.SmtpSettings.Tls, memento.SmtpSettings.Tls);
      ClassicAssert.AreEqual(_settings.SmtpSettings.Subject, memento.SmtpSettings.Subject);
      ClassicAssert.AreEqual(_settings.SmtpSettings.Password, memento.SmtpSettings.Password);

      _settings.SmtpSettings.FromAddress = "phew@123.org";
      _settings.SmtpSettings.Tls = false;
      _settings.SmtpSettings.SmtpServerName = "hostname1234.test.local";

      _settings.RestoreFromMemento(memento);
      ClassicAssert.AreEqual(memento.SmtpSettings.FromAddress, _settings.SmtpSettings.FromAddress);
      ClassicAssert.AreEqual(memento.SmtpSettings.Tls, _settings.SmtpSettings.Tls);
      ClassicAssert.AreEqual(memento.SmtpSettings.SmtpServerName, _settings.SmtpSettings.SmtpServerName);
    }
  }
}
