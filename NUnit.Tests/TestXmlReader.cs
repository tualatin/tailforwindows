using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestXmlReader
  {
    private IXmlReader _xmlReader;
    private TestContext _currenTestContext;
    private TailData _tailData;

    [SetUp]
    protected void SetUp()
    {
      _currenTestContext = TestContext.CurrentContext;
      var path = _currenTestContext.TestDirectory + @"\FileManager.xml";
      _xmlReader = new XmlConfigReadController(path);
      _tailData = new TailData
      {
        Id = Guid.Parse("8a0c7206-7d0e-4d81-a25c-1d4accca09b7"),
        FileName = @"D:\Tools\TailForWindows\logs\tailforwindows_2017-03-06.log",
        Description = "Tail4Windows",
        Category = "T4F",
        ThreadPriority = System.Threading.ThreadPriority.Lowest,
        NewWindow = false,
        RefreshRate = ETailRefreshRate.Highest,
        Timestamp = false,
        RemoveSpace = false,
        Wrap = false,
        FileEncoding = Encoding.ASCII,
        FilterState = false,
        UsePattern = true,
        FontType = new Font("Segoe UI", 12f, FontStyle.Regular),
        SmartWatch = false,
        IsRegex = false,
        PatternString = "tailforwindows_????-??-??.log",
        ListOfFilter = new ObservableCollection<FilterData>
        {
          new FilterData
          {
            Id = Guid.Parse("e8378c20-c0cc-457e-872f-c4139539dec9"),
            Description = "Error filter",
            Filter = "error",
            FilterFontType = new Font("Tahoma", 12f, FontStyle.Regular)
          },
          new FilterData
          {
            Id = Guid.Parse("e8378c20-c0cc-457e-872f-c4139539dec9"),
            Description = "Debug filter",
            Filter = "debug",
            FilterFontType = new Font("Tahoma", 12f, FontStyle.Regular)
          }
        }
      };
    }

    [Test]
    public async Task TestReadXmlConfigFile()
    {
      IXmlReader xmlReader = new XmlConfigReadController(@"C:\blabla\Test.xml");
      var reader = xmlReader;
      Assert.That(() => reader.ReadXmlFile(), Throws.InstanceOf<FileNotFoundException>());

      var path = _currenTestContext.TestDirectory + @"\FileManager_Root.xml";
      xmlReader = new XmlConfigReadController(path);
      Assert.That(() => xmlReader.ReadXmlFile(), Throws.InstanceOf<XmlException>());

      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      Assert.NotNull(files);
      Assert.AreEqual(2, files.Count);
      Assert.IsInstanceOf<TailData>(files.First());

      var xmlTailData = files.First();
      Assert.AreEqual(_tailData.Id, xmlTailData.Id);
      Assert.AreEqual(_tailData.FileName, xmlTailData.FileName);
      Assert.AreEqual(_tailData.Description, xmlTailData.Description);
      Assert.AreEqual(_tailData.Category, xmlTailData.Category);
      Assert.AreEqual(_tailData.ThreadPriority, xmlTailData.ThreadPriority);
      Assert.AreEqual(_tailData.NewWindow, xmlTailData.NewWindow);
      Assert.AreEqual(_tailData.RefreshRate, xmlTailData.RefreshRate);
      Assert.AreEqual(_tailData.Timestamp, xmlTailData.Timestamp);
      Assert.AreEqual(_tailData.RemoveSpace, xmlTailData.RemoveSpace);
      Assert.AreEqual(_tailData.Wrap, xmlTailData.Wrap);
      Assert.AreEqual(_tailData.FileEncoding, xmlTailData.FileEncoding);
      Assert.AreEqual(_tailData.FilterState, xmlTailData.FilterState);
      Assert.AreEqual(_tailData.UsePattern, xmlTailData.UsePattern);
      Assert.AreEqual(_tailData.FontType, xmlTailData.FontType);
      Assert.AreEqual(_tailData.SmartWatch, xmlTailData.SmartWatch);
      Assert.AreEqual(_tailData.IsRegex, xmlTailData.IsRegex);
      Assert.AreEqual(_tailData.PatternString, xmlTailData.PatternString);
      Assert.AreEqual(_tailData.ListOfFilter.Count, xmlTailData.ListOfFilter.Count);

      for ( int i = 0; i < _tailData.ListOfFilter.Count; i++ )
      {
        Assert.AreEqual(_tailData.ListOfFilter[i].Id, xmlTailData.ListOfFilter[i].Id);
        Assert.AreEqual(_tailData.ListOfFilter[i].Description, xmlTailData.ListOfFilter[i].Description);
        Assert.AreEqual(_tailData.ListOfFilter[i].FilterFontType, xmlTailData.ListOfFilter[i].FilterFontType);
        Assert.AreEqual(_tailData.ListOfFilter[i].Filter, xmlTailData.ListOfFilter[i].Filter);
      }
    }

    [Test]
    public async Task TestGetCategories()
    {
      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      var categories = await _xmlReader.GetCategoriesFromXmlFile(files).ConfigureAwait(false);
      Assert.NotNull(categories);
      Assert.AreEqual(2, categories.Count);
      Assert.AreEqual("T4F", categories.First());
      Assert.AreEqual("MS Setup", categories.Last());
      Assert.That(() => _xmlReader.GetCategoriesFromXmlFile(null), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestGetTailDataById()
    {
      var id = Guid.Parse("8a0c7206-7d0e-4d81-a25c-1d4accca09b7");

      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      var tailData = await _xmlReader.GetTailDataById(files, id).ConfigureAwait(false);
      Assert.NotNull(tailData);

      Assert.AreEqual(_tailData.Id, tailData.Id);
      Assert.AreEqual(_tailData.FileName, tailData.FileName);
      Assert.AreEqual(_tailData.Description, tailData.Description);
      Assert.AreEqual(_tailData.Category, tailData.Category);
      Assert.AreEqual(_tailData.ThreadPriority, tailData.ThreadPriority);
      Assert.AreEqual(_tailData.NewWindow, tailData.NewWindow);
      Assert.AreEqual(_tailData.RefreshRate, tailData.RefreshRate);
      Assert.AreEqual(_tailData.Timestamp, tailData.Timestamp);
      Assert.AreEqual(_tailData.RemoveSpace, tailData.RemoveSpace);
      Assert.AreEqual(_tailData.Wrap, tailData.Wrap);
      Assert.AreEqual(_tailData.FileEncoding, tailData.FileEncoding);
      Assert.AreEqual(_tailData.FilterState, tailData.FilterState);
      Assert.AreEqual(_tailData.UsePattern, tailData.UsePattern);
      Assert.AreEqual(_tailData.FontType, tailData.FontType);
      Assert.AreEqual(_tailData.SmartWatch, tailData.SmartWatch);
      Assert.AreEqual(_tailData.IsRegex, tailData.IsRegex);
      Assert.AreEqual(_tailData.PatternString, tailData.PatternString);
      Assert.AreEqual(_tailData.ListOfFilter.Count, tailData.ListOfFilter.Count);

      for ( int i = 0; i < _tailData.ListOfFilter.Count; i++ )
      {
        Assert.AreEqual(_tailData.ListOfFilter[i].Id, tailData.ListOfFilter[i].Id);
        Assert.AreEqual(_tailData.ListOfFilter[i].Description, tailData.ListOfFilter[i].Description);
        Assert.AreEqual(_tailData.ListOfFilter[i].FilterFontType, tailData.ListOfFilter[i].FilterFontType);
        Assert.AreEqual(_tailData.ListOfFilter[i].Filter, tailData.ListOfFilter[i].Filter);
      }

      Assert.That(() => _xmlReader.GetTailDataById(null, id), Throws.InstanceOf<ArgumentException>());
      Assert.That(() => _xmlReader.GetTailDataById(files, Guid.Empty), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestAddTailDataToXmlFile()
    {
      var tailData = new TailData
      {
        Id = Guid.NewGuid(),
        FileName = @"D:\Tools\TailForWindows\logs\testLogFile.log",
        Description = "Test item",
        Category = "Testing",
        ThreadPriority = System.Threading.ThreadPriority.Lowest,
        NewWindow = false,
        RefreshRate = ETailRefreshRate.Normal,
        Timestamp = true,
        RemoveSpace = true,
        Wrap = true,
        PatternString = string.Empty,
        FileEncoding = Encoding.UTF8,
        FilterState = true,
        UsePattern = true,
        FontType = new Font("Segoe UI", 12f, FontStyle.Bold),
        SmartWatch = true,
        IsRegex = false,
        ListOfFilter = new ObservableCollection<FilterData>
        {
          new FilterData
          {
            Id = Guid.NewGuid(),
            Description = "Test filter",
            Filter = "test",
            FilterFontType = new Font("Tahoma", 12f, FontStyle.Italic)
          }
        }
      };

      await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      await _xmlReader.AddTailDataToXmlFile(tailData).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      Assert.AreEqual(3, files.Count);

      var xmlTailData = files.SingleOrDefault(p => p.Id.Equals(tailData.Id));
      Assert.IsNotNull(xmlTailData);
      Assert.AreEqual(tailData.Id, xmlTailData.Id);
      Assert.AreEqual(tailData.FileName, xmlTailData.FileName);
      Assert.AreEqual(tailData.Description, xmlTailData.Description);
      Assert.AreEqual(tailData.Category, xmlTailData.Category);
      Assert.AreEqual(tailData.ThreadPriority, xmlTailData.ThreadPriority);
      Assert.AreEqual(tailData.NewWindow, xmlTailData.NewWindow);
      Assert.AreEqual(tailData.RefreshRate, xmlTailData.RefreshRate);
      Assert.AreEqual(tailData.Timestamp, xmlTailData.Timestamp);
      Assert.AreEqual(tailData.RemoveSpace, xmlTailData.RemoveSpace);
      Assert.AreEqual(tailData.Wrap, xmlTailData.Wrap);
      Assert.AreEqual(tailData.FileEncoding, xmlTailData.FileEncoding);
      Assert.AreEqual(tailData.FilterState, xmlTailData.FilterState);
      Assert.AreEqual(tailData.UsePattern, xmlTailData.UsePattern);
      Assert.AreEqual(tailData.FontType, xmlTailData.FontType);
      Assert.AreEqual(tailData.SmartWatch, xmlTailData.SmartWatch);
      Assert.AreEqual(tailData.IsRegex, xmlTailData.IsRegex);
      Assert.AreEqual(tailData.PatternString, xmlTailData.PatternString);
      Assert.AreEqual(tailData.ListOfFilter.Count, xmlTailData.ListOfFilter.Count);

      for ( int i = 0; i < tailData.ListOfFilter.Count; i++ )
      {
        Assert.AreEqual(tailData.ListOfFilter[i].Id, xmlTailData.ListOfFilter[i].Id);
        Assert.AreEqual(tailData.ListOfFilter[i].Description, xmlTailData.ListOfFilter[i].Description);
        Assert.AreEqual(tailData.ListOfFilter[i].FilterFontType, xmlTailData.ListOfFilter[i].FilterFontType);
        Assert.AreEqual(tailData.ListOfFilter[i].Filter, xmlTailData.ListOfFilter[i].Filter);
      }
    }

    [Test]
    public async Task TestAddTailDataWhenNotExistsXmlFile()
    {
      var path = _currenTestContext.TestDirectory + @"\FileManager.xml";

      if ( File.Exists(path) )
        File.Delete(path);

      var tailData = new TailData
      {
        Id = Guid.NewGuid(),
        FileName = @"D:\Tools\TailForWindows\logs\testLogFile.log",
        Description = "Test item",
        Category = "Testing",
        ThreadPriority = System.Threading.ThreadPriority.Lowest,
        NewWindow = false,
        RefreshRate = ETailRefreshRate.Normal,
        Timestamp = true,
        RemoveSpace = true,
        Wrap = true,
        PatternString = string.Empty,
        FileEncoding = Encoding.UTF8,
        FilterState = true,
        UsePattern = true,
        FontType = new Font("Segoe UI", 12f, FontStyle.Bold),
        SmartWatch = true,
        IsRegex = false,
        ListOfFilter = new ObservableCollection<FilterData>
        {
          new FilterData
          {
            Id = Guid.NewGuid(),
            Description = "Test filter",
            Filter = "test",
            FilterFontType = new Font("Tahoma", 12f, FontStyle.Italic)
          }
        }
      };

      await _xmlReader.AddTailDataToXmlFile(tailData).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      Assert.AreEqual(1, files.Count);

      var xmlTailData = files.SingleOrDefault(p => p.Id.Equals(tailData.Id));
      Assert.IsNotNull(xmlTailData);
      Assert.AreEqual(tailData.Id, xmlTailData.Id);
      Assert.AreEqual(tailData.FileName, xmlTailData.FileName);
      Assert.AreEqual(tailData.Description, xmlTailData.Description);
      Assert.AreEqual(tailData.Category, xmlTailData.Category);
      Assert.AreEqual(tailData.ThreadPriority, xmlTailData.ThreadPriority);
      Assert.AreEqual(tailData.NewWindow, xmlTailData.NewWindow);
      Assert.AreEqual(tailData.RefreshRate, xmlTailData.RefreshRate);
      Assert.AreEqual(tailData.Timestamp, xmlTailData.Timestamp);
      Assert.AreEqual(tailData.RemoveSpace, xmlTailData.RemoveSpace);
      Assert.AreEqual(tailData.Wrap, xmlTailData.Wrap);
      Assert.AreEqual(tailData.FileEncoding, xmlTailData.FileEncoding);
      Assert.AreEqual(tailData.FilterState, xmlTailData.FilterState);
      Assert.AreEqual(tailData.UsePattern, xmlTailData.UsePattern);
      Assert.AreEqual(tailData.FontType, xmlTailData.FontType);
      Assert.AreEqual(tailData.SmartWatch, xmlTailData.SmartWatch);
      Assert.AreEqual(tailData.IsRegex, xmlTailData.IsRegex);
      Assert.AreEqual(tailData.PatternString, xmlTailData.PatternString);
      Assert.AreEqual(tailData.ListOfFilter.Count, xmlTailData.ListOfFilter.Count);

      for ( int i = 0; i < tailData.ListOfFilter.Count; i++ )
      {
        Assert.AreEqual(tailData.ListOfFilter[i].Id, xmlTailData.ListOfFilter[i].Id);
        Assert.AreEqual(tailData.ListOfFilter[i].Description, xmlTailData.ListOfFilter[i].Description);
        Assert.AreEqual(tailData.ListOfFilter[i].FilterFontType, xmlTailData.ListOfFilter[i].FilterFontType);
        Assert.AreEqual(tailData.ListOfFilter[i].Filter, xmlTailData.ListOfFilter[i].Filter);
      }
    }

    [Test]
    public async Task TestUpdateXmlConfigFile()
    {
      _tailData.Description = "Windows";
      _tailData.Category = "For testing";
      _tailData.ListOfFilter.Clear();

      await _xmlReader.UpdateTailDataInXmlFile(_tailData).ConfigureAwait(false);
    }

    [Test]
    public async Task TestRemoveTailDataById()
    {
      var id = "8a0c7206-7d0e-4d81-a25c-1d4accca09b7";
      await _xmlReader.ReadXmlFile().ConfigureAwait(false);
      await _xmlReader.DeleteTailDataByIdFromXmlFile(id).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFile().ConfigureAwait(false);

      Assert.AreEqual(1, files.Count);
      Assert.That(() => _xmlReader.DeleteTailDataByIdFromXmlFile(null), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestRemoveFilterById()
    {
      var id = "8a0c7206-7d0e-4d81-a25c-1d4accca09b7";
      var idFilter = "e8378c20-c0cc-457e-872f-c4139539dec9";
      await _xmlReader.DeleteFilterByIdByTailDataIdFromXmlFile(id, idFilter).ConfigureAwait(false);
    }
  }
}
