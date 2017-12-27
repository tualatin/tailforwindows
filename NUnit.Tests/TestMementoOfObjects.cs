using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestMementoOfObjects
  {
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
  }
}
