using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestMementoOfObjects
  {
    private EnvironmentSettings _settings;

    [SetUp]
    protected void SetUp()
    {
      _settings = new EnvironmentSettings
      {
        RestoreWindowSize = true,
        SaveWindowPosition = true,
        ShowNumberLineAtStart = true,
        LinesRead = 20,
        ExitWithEscape = true
      };
    }

    [Test]
    public void TestMementoFilterDate()
    {
      FilterData filter = new FilterData
      {
        Id = Guid.NewGuid(),
        Description = "Test Filter",
        Filter = @"^\d[A...Z]",
        FilterColor = System.Windows.Media.Brushes.Beige,
        FilterFontType = new Font("Tahoma", 11f, FontStyle.Italic)
      };

      var memento = filter.SaveToMemento();
      Assert.IsInstanceOf<FilterData.MementoFilterData>(memento);
      Assert.AreEqual(filter.Id, memento.Id);
      Assert.AreEqual(filter.Description, memento.Description);
      Assert.AreEqual(filter.Filter, memento.Filter);
      Assert.AreEqual(filter.FilterColor, memento.FilterColor);
      Assert.AreEqual(filter.FilterFontType, memento.FilterFontType);

      filter.Description = "Test Filter 2";
      filter.Filter = "....";
      filter.FilterColor = System.Windows.Media.Brushes.Black;
      filter.FilterFontType = new Font("Curier", 8f, FontStyle.Regular);

      filter.RestoreFromMemento(memento);
      Assert.IsInstanceOf<FilterData>(filter);
      Assert.AreEqual(memento.Description, filter.Description);
    }

    [Test]
    public void TestMementoTailData()
    {
      FilterData filter = new FilterData
      {
        Id = Guid.NewGuid(),
        Description = "Test Filter",
        Filter = @"^\d[A...Z]",
        FilterColor = System.Windows.Media.Brushes.Beige,
        FilterFontType = new Font("Tahoma", 11f, FontStyle.Italic)
      };

      TailData tailData = new TailData
      {
        Id = Guid.NewGuid(),
        AutoRun = false,
        Category = "Test category",
        Description = "Test Description",
        FontType = new Font("Tahoma", 11f, FontStyle.Underline),
        IsRegex = true,
        NewWindow = false,
        PatternString = "???_??.???",
        RefreshRate = ETailRefreshRate.Highest,
        RemoveSpace = true,
        ThreadPriority = ThreadPriority.Normal,
        SmartWatch = true,
        UsePattern = true,
        Wrap = false,
        Timestamp = true,
        FileName = @"C:\TestFile",
        FileEncoding = Encoding.UTF8,
        FilterState = true,
        LastRefreshTime = DateTime.Now,
        OpenFromFileManager = true,
        OpenFromSmartWatch = true,
        Version = 2.1m,
        ListOfFilter = new ObservableCollection<FilterData> { filter }
      };

      var memento = tailData.SaveToMemento();
      Assert.IsInstanceOf<TailData.MementoTailData>(memento);
      Assert.AreEqual(tailData.Id, memento.Id);
      Assert.AreEqual(tailData.AutoRun, memento.AutoRun);
      Assert.AreEqual(tailData.Category, memento.Category);
      Assert.AreEqual(tailData.Description, memento.Description);
      Assert.AreEqual(tailData.FontType, memento.FontType);
      Assert.AreEqual(tailData.IsRegex, memento.IsRegex);
      Assert.AreEqual(tailData.NewWindow, memento.NewWindow);
      Assert.AreEqual(tailData.PatternString, memento.PatternString);
      Assert.AreEqual(tailData.RefreshRate, memento.RefreshRate);
      Assert.AreEqual(tailData.RemoveSpace, memento.RemoveSpace);
      Assert.AreEqual(tailData.ThreadPriority, memento.ThreadPriority);
      Assert.AreEqual(tailData.SmartWatch, memento.SmartWatch);
      Assert.AreEqual(tailData.UsePattern, memento.UsePattern);
      Assert.AreEqual(tailData.Wrap, memento.Wrap);
      Assert.AreEqual(tailData.Timestamp, memento.TimeStamp);
      Assert.AreEqual(tailData.FileName, memento.FileName);
      Assert.AreEqual(tailData.FileEncoding, memento.FileEncoding);
      Assert.AreEqual(tailData.FilterState, memento.FilterState);
      Assert.AreEqual(tailData.LastRefreshTime, memento.LastRefreshTime);
      Assert.AreEqual(tailData.OpenFromSmartWatch, memento.OpenFromSmartWatch);
      Assert.IsTrue(tailData.ListOfFilter.CompareGenericObservableCollections(memento.ListOfFilter));

      filter = new FilterData
      {
        Id = Guid.NewGuid(),
        Description = "Test Filter 2",
        Filter = "Error",
        FilterColor = System.Windows.Media.Brushes.Beige
      };

      tailData.ListOfFilter.Add(filter);
      tailData.Description = "Test 2";
      tailData.NewWindow = true;
      tailData.FileName = @"C:\Test3";
      tailData.FileEncoding = Encoding.ASCII;

      tailData.RestoreFromMemento(memento);
      Assert.IsInstanceOf<TailData>(tailData);
      Assert.AreEqual(memento.Description, tailData.Description);
      Assert.AreEqual(memento.NewWindow, tailData.NewWindow);
      Assert.AreEqual(memento.FileName, tailData.FileName);
      Assert.AreEqual(memento.FileEncoding, tailData.FileEncoding);
      Assert.IsTrue(memento.ListOfFilter.CompareGenericObservableCollections(tailData.ListOfFilter));
    }

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
      Assert.IsInstanceOf<EnvironmentSettings.MementoEnvironmentSettings>(memento);
      Assert.AreEqual(proxy.UseSystemSettings, memento.ProxySettings.UseSystemSettings);
      Assert.AreEqual(proxy.UserName, memento.ProxySettings.UserName);
      Assert.AreEqual(proxy.Password, memento.ProxySettings.Password);
      Assert.AreEqual(proxy.ProxyPort, memento.ProxySettings.ProxyPort);
      Assert.AreEqual(proxy.ProxyUrl, memento.ProxySettings.ProxyUrl);

      proxy.UseSystemSettings = null;
      proxy.ProxyPort = 4560;
      _settings.RestoreWindowSize = false;
      _settings.LinesRead = 40;
      _settings.ExitWithEscape = false;

      _settings.RestoreFromMemento(memento);
      Assert.AreEqual(memento.ProxySettings.UseSystemSettings, _settings.ProxySettings.UseSystemSettings);
      Assert.AreEqual(memento.ProxySettings.ProxyPort, _settings.ProxySettings.ProxyPort);
      Assert.AreEqual(memento.RestoreWindowSize, _settings.RestoreWindowSize);
      Assert.AreEqual(memento.LinesRead, _settings.LinesRead);
      Assert.AreEqual(memento.ExitWithEscape, _settings.ExitWithEscape);
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
      Assert.IsInstanceOf<EnvironmentSettings.MementoEnvironmentSettings>(memento);
      Assert.AreEqual(_settings.SmartWatchSettings.Mode, memento.SmartWatchSettings.Mode);
      Assert.AreEqual(_settings.SmartWatchSettings.AutoRun, memento.SmartWatchSettings.AutoRun);
      Assert.AreEqual(_settings.SmartWatchSettings.NewTab, memento.SmartWatchSettings.NewTab);
      Assert.AreEqual(_settings.SmartWatchSettings.FilterByExtension, memento.SmartWatchSettings.FilterByExtension);

      smartWatch.AutoRun = false;
      smartWatch.Mode = ESmartWatchMode.Auto;

      _settings.RestoreFromMemento(memento);
      Assert.AreEqual(memento.SmartWatchSettings.AutoRun, _settings.SmartWatchSettings.AutoRun);
      Assert.AreEqual(memento.SmartWatchSettings.Mode, _settings.SmartWatchSettings.Mode);
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
      Assert.IsInstanceOf<EnvironmentSettings.MementoEnvironmentSettings>(memento);
      Assert.AreEqual(_settings.AlertSettings.BringToFront, memento.AlertSettings.BringToFront);
      Assert.AreEqual(_settings.AlertSettings.MailAddress, memento.AlertSettings.MailAddress);
      Assert.AreEqual(_settings.AlertSettings.PopupWnd, memento.AlertSettings.PopupWnd);
      Assert.AreEqual(_settings.SmtpSettings.FromAddress, memento.SmtpSettings.FromAddress);
      Assert.AreEqual(_settings.SmtpSettings.LoginName, memento.SmtpSettings.LoginName);
      Assert.AreEqual(_settings.SmtpSettings.Tls, memento.SmtpSettings.Tls);
      Assert.AreEqual(_settings.SmtpSettings.Subject, memento.SmtpSettings.Subject);
      Assert.AreEqual(_settings.SmtpSettings.Password, memento.SmtpSettings.Password);

      _settings.SmtpSettings.FromAddress = "phew@123.org";
      _settings.SmtpSettings.Tls = false;
      _settings.SmtpSettings.SmtpServerName = "hostname1234.test.local";

      _settings.RestoreFromMemento(memento);
      Assert.AreEqual(memento.SmtpSettings.FromAddress, _settings.SmtpSettings.FromAddress);
      Assert.AreEqual(memento.SmtpSettings.Tls, _settings.SmtpSettings.Tls);
      Assert.AreEqual(memento.SmtpSettings.SmtpServerName, _settings.SmtpSettings.SmtpServerName);
    }
  }
}
