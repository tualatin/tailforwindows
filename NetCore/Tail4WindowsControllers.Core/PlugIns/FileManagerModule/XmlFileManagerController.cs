using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Controllers;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Collections;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Logging;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule
{
  /// <summary>
  /// XML FileManager controller
  /// </summary>
  [Obsolete("Please use FileManagerController instead")]
  public class XmlFileManagerController : IXmlFileManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlFileManagerController));

    private readonly ISmartWatchController _smartWatchController;
    private XDocument _xmlDocument;

    /// <summary>
    /// Gets current XML config file
    /// </summary>
    public string XmlFileName
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlFileManagerController()
    {
      XmlFileName = CoreEnvironment.UserSettingsPath + @"\FileManager.xml";
      _smartWatchController = new SmartWatchController();
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlFileManagerController(string path)
    {
      XmlFileName = path;
      _smartWatchController = new SmartWatchController();
    }


    /// <summary>
    /// Read XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>List of tail settings from XML file</returns>
    /// <exception cref="FileNotFoundException">If XML file does not exists</exception>
    /// <exception cref="XmlException">If an error occurred while reading XML file</exception>
    public async Task<ObservableCollection<TailData>> ReadXmlFileAsync(CancellationToken token)
    {
      if ( !File.Exists(XmlFileName) )
        return new ObservableCollection<TailData>();

      LOG.Trace("Read XML");

      var result = await Task.Run(() => ReadXmlFile(), token);

      if ( result != null && SettingsHelperController.CurrentSettings.SmartWatch )
        await ModifyFileNameBySmartWatchAsync(result);

      if ( result != null )
        result = await RemoveDuplicateItemsAsync(result, token);

      return result;
    }

    private ObservableCollection<TailData> ReadXmlFile()
    {
      var result = new ObservableCollection<TailData>();

      try
      {
        _xmlDocument = XDocument.Load(XmlFileName);
        var xmlVersion = _xmlDocument.Root?.Element(XmlNames.XmlVersion)?.Value.ConvertToDecimal();
        decimal version = xmlVersion.HasValue ? (xmlVersion.Value == decimal.MinValue ? XmlNames.CurrentXmlVersion : xmlVersion.Value) : XmlNames.CurrentXmlVersion;
        var files = new AsyncObservableCollection<TailData>();

        Parallel.ForEach(_xmlDocument.Root?.Descendants(XmlNames.File) ?? new List<XElement>(), p =>
        {
          var data = new TailData
          {
            Version = version,
            Id = GetIdByElement(p.Element(XmlNames.Id)?.Value),
            IsLoadedByXml = true,
            Description = p.Element(XmlNames.Description)?.Value,
            FileName = p.Element(XmlNames.FileName)?.Value,
            OriginalFileName = p.Element(XmlNames.FileName)?.Value,
            Category = p.Element(XmlNames.Category)?.Value,
            Wrap = (p.Element(XmlNames.LineWrap)?.Value).ConvertToBool(),
            RemoveSpace = (p.Element(XmlNames.RemoveSpace)?.Value).ConvertToBool(),
            Timestamp = (p.Element(XmlNames.TimeStamp)?.Value).ConvertToBool(),
            NewWindow = (p.Element(XmlNames.NewWindow)?.Value).ConvertToBool(),
            SmartWatch = (p.Element(XmlNames.UseSmartWatch)?.Value).ConvertToBool(),
            UsePattern = (p.Element(XmlNames.UsePattern)?.Value).ConvertToBool(),
            WindowsEvent = GetWindowsEventData(p.Element(XmlNames.WindowsEvent)),
            IsWindowsEvent = (p.Element(XmlNames.IsWindowsEvent)?.Value).ConvertToBool(),
            TabItemBackgroundColorStringHex = GetColorAsString(p.Element(XmlNames.TabItemBackgroundColor)?.Value),
            ThreadPriority = EnvironmentContainer.GetThreadPriority(p.Element(XmlNames.ThreadPriority)?.Value),
            RefreshRate = EnvironmentContainer.GetRefreshRate(p.Element(XmlNames.RefreshRate)?.Value),
            FileEncoding = GetEncoding(p.Element(XmlNames.FileEncoding)?.Value),
            FilterState = (p.Element(XmlNames.UseFilters)?.Value).ConvertToBool(),
            FontType = GetFont(p.Element(XmlNames.Font)),
            FindSettings = GetSearchPatternFindSettings(p.Element(XmlNames.SearchPattern)),
            PatternString = p.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.PatternString)?.Value,
          };

          var filters = p.Element(XmlNames.Filters)?.Descendants(XmlNames.Filter).Select(x => new FilterData
          {
            Id = GetIdByElement(x.Element(XmlNames.Id)?.Value),
            Description = x.Element(XmlNames.FilterName)?.Value,
            Filter = x.Element(XmlNames.FilterPattern)?.Value,
            FontType = GetFont(x.Element(XmlNames.Font)),
            FilterColorHex = x.Element(XmlNames.FilterColor)?.Value ?? DefaultEnvironmentSettings.FilterFontColor,
            FindSettingsData = GetFilterSettingsData(x),
            IsHighlight = (x.Element(XmlNames.FilterIsHighlight)?.Value).ConvertToBool(),
            UseNotification = (x.Element(XmlNames.FilterNotification)?.Value).ConvertToBool(),
            FilterSource = (x.Element(XmlNames.FilterSource)?.Value).ConvertToBool(true),
            IsAutoBookmark = (x.Element(XmlNames.FilterAutoBookmark)?.Value).ConvertToBool(),
            AutoBookmarkComment = (x.Element(XmlNames.FilterAutoBookmarkComment)?.Value),
            IsEnabled = (x.Element(XmlNames.IsEnabled)?.Value).ConvertToBool(true)
          }).ToList() ?? new List<FilterData>();

          filters.ForEach(f => data.ListOfFilter.Add(f));
          files.Add(data);
        });

        result = new ObservableCollection<TailData>(files);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
      return result;
    }

    private async Task ModifyFileNameBySmartWatchAsync(ObservableCollection<TailData> result)
    {
      foreach ( TailData item in result.Where(p => p != null && p.SmartWatch && p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameByPatternAsync(item, item.PatternString);
      }

      foreach ( TailData item in result.Where(p => p != null && p.SmartWatch && !p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameBySmartWatchAsync(item);
      }
    }

    private async Task<ObservableCollection<TailData>> RemoveDuplicateItemsAsync(ObservableCollection<TailData> items, CancellationToken token)
    {
      LOG.Trace("Try to remove duplicate items");

      ObservableCollection<TailData> result = null;

      await Task.Run(() =>
      {
        try
        {
          var list = new List<TailData>();

          // Group all items not IsWindowsEvent
          list.AddRange(items.Where(p => p != null && !p.IsWindowsEvent).GroupBy(p => p.FileName.ToLower()).Select(p => p.FirstOrDefault()).ToList());
          list.ForEach(InsertFilterData);

          // Group all item IsWindowsEvent
          list.AddRange(items.Where(p => p != null && p.IsWindowsEvent).GroupBy(p => p.File.ToLower()).Select(p => p.FirstOrDefault()).ToList());
          list.ForEach(InsertFilterData);

          result = new ObservableCollection<TailData>(list);
        }
        catch
        {
          result = items;
        }
      }, token).ConfigureAwait(false);

      return result;
    }

    private static void InsertFilterData(TailData w)
    {
      var grouped = w.ListOfFilter.GroupBy(p => p.Filter.ToLower()).Select(p => p.FirstOrDefault()).ToList();
      w.ListOfFilter.Clear();

      foreach ( FilterData item in grouped )
      {
        w.ListOfFilter.Add(item);
      }
    }

    private FindData GetSearchPatternFindSettings(XContainer settings)
    {
      var searchSettings = new FindData
      {
        UseRegex = settings.Element(XmlBaseStructure.IsRegex)?.Value.ConvertToBool() ?? false,
        UseWildcard = settings.Element(XmlBaseStructure.UseWildcard)?.Value.ConvertToBool() ?? false,
        WholeWord = true
      };
      return searchSettings;
    }

    private FindData GetFilterSettingsData(XContainer settings)
    {
      var findSettings = new FindData
      {
        CaseSensitive = settings.Element(XmlNames.FilterMatchCase)?.Value.ConvertToBool() ?? false,
        WholeWord = settings.Element(XmlNames.FilterMatchWholeWord)?.Value.ConvertToBool() ?? false,
        UseRegex = settings.Element(XmlNames.FilterRegex)?.Value.ConvertToBool() ?? false,
        UseWildcard = settings.Element(XmlNames.FilterUseWildcard)?.Value.ConvertToBool() ?? false
      };
      return findSettings;
    }

    private string GetColorAsString(string value) => string.IsNullOrWhiteSpace(value) ? DefaultEnvironmentSettings.TabItemHeaderBackgroundColor : value;

    #region HelperFunctions

    private static Guid GetIdByElement(string id)
    {
      if ( !Guid.TryParse(id, out var result) )
        result = Guid.NewGuid();

      return result;
    }

    private static Encoding GetEncoding(string sEncode)
    {
      Encoding encoding = null;

      foreach ( Encoding encode in EnvironmentContainer.Instance.FileEncoding )
      {
        if ( string.Compare(encode.HeaderName, sEncode, StringComparison.Ordinal) == 0 )
        {
          encoding = encode;
          break;
        }

        encoding = Encoding.UTF8;
      }
      return encoding;
    }

    private static WindowsEventData GetWindowsEventData(XContainer xmlWindowsEvent)
    {
      if ( xmlWindowsEvent == null )
        return new WindowsEventData();

      var windowsEvent = new
      {
        Category = xmlWindowsEvent.Element(XmlNames.WindowsEventCategory)?.Value,
        UserName = xmlWindowsEvent.Element(XmlNames.WindowsEventUserName)?.Value,
        MachineName = xmlWindowsEvent.Element(XmlNames.WindowsEventMachineName)?.Value,
        Name = xmlWindowsEvent.Element(XmlNames.WindowsEventName)?.Value
      };

      return new WindowsEventData
      {
        Category = windowsEvent.Category,
        Machine = windowsEvent.MachineName,
        Name = windowsEvent.Name,
        UserName = windowsEvent.UserName
      };
    }

    private static FontType GetFont(XContainer xmlFont)
    {
      if ( xmlFont == null )
        return new FontType();

      var font = new
      {
        Name = xmlFont.Element(XmlBaseStructure.Name)?.Value,
        Size = (xmlFont.Element(XmlNames.Size)?.Value).ConvertToFloat(),
        Weight = xmlFont.Element(XmlNames.Weight)?.Value,
        Style = xmlFont.Element(XmlNames.Style)?.Value
      };

      FontWeight fontWeight;
      FontStyle fontStyle;

      try
      {
        // ReSharper disable once PossibleNullReferenceException
        fontWeight = (FontWeight) new FontWeightConverter().ConvertFromString(font.Weight);
      }
      catch
      {
        fontWeight = FontWeights.Normal;
      }

      try
      {
        // ReSharper disable once PossibleNullReferenceException
        fontStyle = (FontStyle) new FontStyleConverter().ConvertFromString(font.Style);
      }
      catch
      {
        fontStyle = FontStyles.Normal;
      }

      return new FontType
      {
        FontFamily = new FontFamily(font.Name),
        FontSize = font.Size,
        FontStretch = FontStretches.Normal,
        FontWeight = fontWeight,
        FontStyle = fontStyle
      };
    }

    #endregion
  }
}
