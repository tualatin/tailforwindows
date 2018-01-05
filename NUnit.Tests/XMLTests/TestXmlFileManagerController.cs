using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers.XMLCore;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces.XMLCore;


namespace Org.Vs.NUnit.Tests.XMLTests
{
  [TestFixture]
  public class TestXmlFileManagerController
  {
    private IXmlFileManager _xmlReader;
    private TestContext _currenTestContext;
    private TailData _tailData;
    private string _path;
    private string _tempPath;

    [SetUp]
    protected void SetUp()
    {
      _currenTestContext = TestContext.CurrentContext;
      _tempPath = _currenTestContext.TestDirectory + @"\Files\FileManager.xml";
      _path = _currenTestContext.TestDirectory + @"\FileManager.xml";
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
            Id = Guid.Parse("e8378c20-c1cc-457e-872f-c4139539dec9"),
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
      IXmlFileManager xmlReader = new XmlFileManagerController(@"C:\blabla\Test.xml");
      var reader = xmlReader;
      Assert.That(() => reader.ReadXmlFileAsync(), Throws.InstanceOf<FileNotFoundException>());

      InitXmlReader();

      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      Assert.NotNull(files);
      Assert.AreEqual(2, files.Count);
      Assert.IsInstanceOf<TailData>(files.First());

      var xmlTailData = files.SingleOrDefault(p => p.Id.Equals(_tailData.Id));
      Assert.IsNotNull(xmlTailData);
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

      foreach ( FilterData t in _tailData.ListOfFilter )
      {
        var filter = xmlTailData.ListOfFilter.SingleOrDefault(p => p.Id == t.Id);
        Assert.IsNotNull(filter);

        Assert.AreEqual(t.Id, filter.Id);
        Assert.AreEqual(t.Description, filter.Description);
        Assert.AreEqual(t.FilterFontType, filter.FilterFontType);
        Assert.AreEqual(t.Filter, filter.Filter);
      }
    }

    private void InitXmlReader()
    {
      if ( File.Exists(_path) )
        File.Delete(_path);

      File.Copy(_tempPath, _path);
      _xmlReader = new XmlFileManagerController(_path);
    }

    [Test]
    public async Task TestGetCategories()
    {
      InitXmlReader();

      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      var categories = await _xmlReader.GetCategoriesFromXmlFileAsync(files).ConfigureAwait(false);
      Assert.NotNull(categories);
      Assert.AreEqual(2, categories.Count);
      Assert.IsNotNull(categories.SingleOrDefault(p => p.Equals("T4F")));
      Assert.IsNotNull(categories.SingleOrDefault(p => p.Equals("MS Setup")));
      Assert.That(() => _xmlReader.GetCategoriesFromXmlFileAsync(null), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestGetTailDataById()
    {
      InitXmlReader();

      var id = Guid.Parse("8a0c7206-7d0e-4d81-a25c-1d4accca09b7");

      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      var tailData = await _xmlReader.GetTailDataByIdAsync(files, id).ConfigureAwait(false);
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

      foreach ( FilterData t in _tailData.ListOfFilter )
      {
        var filter = tailData.ListOfFilter.SingleOrDefault(p => p.Id == t.Id);
        Assert.IsNotNull(filter);

        Assert.AreEqual(t.Id, filter.Id);
        Assert.AreEqual(t.Description, filter.Description);
        Assert.AreEqual(t.FilterFontType, filter.FilterFontType);
        Assert.AreEqual(t.Filter, filter.Filter);
      }

      Assert.That(() => _xmlReader.GetTailDataByIdAsync(null, id), Throws.InstanceOf<ArgumentException>());
      Assert.That(() => _xmlReader.GetTailDataByIdAsync(files, Guid.Empty), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestAddTailDataToXmlFile()
    {
      var tailData = new TailData
      {
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
            Description = "Test filter",
            Filter = "test",
            FilterFontType = new Font("Tahoma", 12f, FontStyle.Italic)
          }
        }
      };

      InitXmlReader();

      await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      await _xmlReader.AddTailDataToXmlFileAsync(tailData).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
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
      InitXmlReader();

      Assert.That(() => _xmlReader.AddTailDataToXmlFileAsync(null), Throws.InstanceOf<ArgumentException>());

      var path = _currenTestContext.TestDirectory + @"\FileManager.xml";

      if ( File.Exists(path) )
        File.Delete(path);

      var tailData = new TailData
      {
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
            Description = "Test filter",
            Filter = "test",
            FilterFontType = new Font("Tahoma", 12f, FontStyle.Italic)
          }
        }
      };

      await _xmlReader.AddTailDataToXmlFileAsync(tailData).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
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
      InitXmlReader();

      _tailData.Description = "Windows";
      _tailData.Category = "For testing";
      _tailData.FontType = new Font("Tahoma", 10f, FontStyle.Bold);
      _tailData.FileName = @"C:\Test\Test.log";
      _tailData.ListOfFilter.Clear();

      Assert.That(() => _xmlReader.UpdateTailDataInXmlFileAsync(null), Throws.InstanceOf<ArgumentException>());

      await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      await _xmlReader.UpdateTailDataInXmlFileAsync(_tailData).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      var xmlData = files.SingleOrDefault(p => p.Id.Equals(_tailData.Id));

      Assert.IsNotNull(xmlData);
      Assert.AreEqual(_tailData.Description, xmlData.Description);
      Assert.AreEqual(_tailData.Category, xmlData.Category);
      Assert.AreEqual(_tailData.FontType, xmlData.FontType);
      Assert.AreEqual(_tailData.ListOfFilter.Count, xmlData.ListOfFilter.Count);

      _tailData.ListOfFilter.Add(new FilterData
      {
        Description = "Test Filter",
        Filter = "filter",
        FilterColor = System.Windows.Media.Brushes.Bisque,
        FilterFontType = new Font("Tahoma", 9f, FontStyle.Italic)
      });

      await _xmlReader.UpdateTailDataInXmlFileAsync(_tailData).ConfigureAwait(false);
      files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      xmlData = files.SingleOrDefault(p => p.Id.Equals(_tailData.Id));

      Assert.IsNotNull(xmlData);
      Assert.AreEqual(_tailData.ListOfFilter.Count, xmlData.ListOfFilter.Count);

      var filter = xmlData.ListOfFilter.First();
      Assert.IsNotNull(filter);
      Assert.AreEqual(_tailData.ListOfFilter.First().Description, filter.Description);
      Assert.AreEqual(_tailData.ListOfFilter.First().Filter, filter.Filter);
      Assert.AreEqual(_tailData.ListOfFilter.First().FilterColor.ToString(), filter.FilterColor.ToString());
      Assert.AreEqual(_tailData.ListOfFilter.First().FilterFontType, filter.FilterFontType);
    }

    [Test]
    public async Task TestRemoveTailDataById()
    {
      InitXmlReader();

      var id = "8a0c7206-7d0e-4d81-a25c-1d4accca09b7";
      await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      await _xmlReader.DeleteTailDataByIdFromXmlFileAsync(id).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);

      Assert.AreEqual(1, files.Count);
      Assert.That(() => _xmlReader.DeleteTailDataByIdFromXmlFileAsync(null), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public async Task TestRemoveFilterById()
    {
      InitXmlReader();

      var id = "8a0c7206-7d0e-4d81-a25c-1d4accca09b7";
      var idFilter = "e8378c20-c0cc-457e-872f-c4139539dec9";

      await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);
      await _xmlReader.DeleteFilterByIdByTailDataIdFromXmlFileAsync(id, idFilter).ConfigureAwait(false);
      var files = await _xmlReader.ReadXmlFileAsync().ConfigureAwait(false);

      var xmlData = files.SingleOrDefault(p => p.Id == Guid.Parse(id));
      Assert.IsNotNull(xmlData);
      Assert.AreEqual(1, xmlData.ListOfFilter.Count);
      Assert.IsNull(xmlData.ListOfFilter.SingleOrDefault(p => p.Id == Guid.Parse(idFilter)));
      Assert.That(() => _xmlReader.DeleteFilterByIdByTailDataIdFromXmlFileAsync(null, null), Throws.InstanceOf<ArgumentException>());
    }
  }
}
