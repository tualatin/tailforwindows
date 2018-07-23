using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Controllers;
using Org.Vs.TailForWin.Business.SmartWatchEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule
{
  /// <summary>
  /// XML FileManager controller
  /// </summary>
  public class XmlFileManagerController : IXmlFileManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlFileManagerController));

    private readonly string _fileManagerFile;
    private readonly ISmartWatchController _smartWatchController;
    private XDocument _xmlDocument;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlFileManagerController()
    {
      _fileManagerFile = EnvironmentContainer.UserSettingsPath + @"\FileManager.xml";
      _smartWatchController = new SmartWatchController();
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlFileManagerController(string path)
    {
      _fileManagerFile = path;
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
      if ( !File.Exists(_fileManagerFile) )
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
        _xmlDocument = XDocument.Load(_fileManagerFile);
        var xmlVersion = _xmlDocument.Root?.Element(XmlNames.XmlVersion)?.Value.ConvertToDecimal();
        decimal version;

        if ( xmlVersion.HasValue )
          version = xmlVersion.Value == decimal.MinValue ? XmlNames.CurrentXmlVersion : xmlVersion.Value;
        else
          version = XmlNames.CurrentXmlVersion;

        var files = _xmlDocument.Root?.Descendants(XmlNames.File).AsParallel().Select(p => new TailData
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
          ListOfFilter = new ObservableCollection<FilterData>(p.Element(XmlNames.Filters)?.Descendants(XmlNames.Filter).Select(x => new FilterData
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
            IsEnabled = (x.Element(XmlNames.IsEnabled)?.Value).ConvertToBool(true)
          }).ToList() ?? new List<FilterData>())
        }).ToList();

        if ( files != null )
          result = new ObservableCollection<TailData>(files);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
      return result;
    }

    private async Task ModifyFileNameBySmartWatchAsync(ObservableCollection<TailData> result)
    {
      foreach ( TailData item in result.Where(p => p.SmartWatch && p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameByPatternAsync(item, item.PatternString).ConfigureAwait(false);
      }

      foreach ( TailData item in result.Where(p => p.SmartWatch && !p.UsePattern).ToList() )
      {
        item.OriginalFileName = item.FileName;
        item.FileName = await _smartWatchController.GetFileNameBySmartWatchAsync(item).ConfigureAwait(false);
      }
    }

    private async Task<ObservableCollection<TailData>> RemoveDuplicateItemsAsync(ObservableCollection<TailData> items, CancellationToken token)
    {
      LOG.Trace("Try to remove duplicate items");

      ObservableCollection<TailData> result = null;

      await Task.Run(() =>
      {
        var list = new List<TailData>();
        list.AddRange(items.Where(p => !p.IsWindowsEvent).GroupBy(p => p.FileName.ToLower()).Select(p => p.FirstOrDefault()).ToList());
        list.ForEach(w =>
        {
          w.ListOfFilter = new ObservableCollection<FilterData>(w.ListOfFilter.GroupBy(p => p.Filter.ToLower()).Select(p => p.FirstOrDefault()).ToList());
        });

        list.AddRange(items.Where(p => p.IsWindowsEvent).GroupBy(p => p.File.ToLower()).Select(p => p.FirstOrDefault()).ToList());
        list.ForEach(w =>
        {
          w.ListOfFilter = new ObservableCollection<FilterData>(w.ListOfFilter.GroupBy(p => p.Filter.ToLower()).Select(p => p.FirstOrDefault()).ToList());
        });

        result = new ObservableCollection<TailData>(list);
      }, token).ConfigureAwait(false);

      return result;
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

    /// <summary>
    /// Get list of categories from XML file
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <returns>List of all categories</returns>
    /// <exception cref="ArgumentException">If <c>tailData</c> is null</exception>
    public async Task<ObservableCollection<string>> GetCategoriesFromXmlFileAsync(ObservableCollection<TailData> tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));
      LOG.Trace("Get all categories from XML");

      return await Task.Run(() => GetCategoriesFromXmlFile(tailData)).ConfigureAwait(false);
    }

    private ObservableCollection<string> GetCategoriesFromXmlFile(ObservableCollection<TailData> tailData)
    {
      var result = new ObservableCollection<string>();

      try
      {
        var categories = tailData.Select(p => p.Category).Distinct().ToList();
        result = new ObservableCollection<string>(categories);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
      return result;
    }

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    public async Task WriteXmlFileAsync(CancellationToken token)
    {
      LOG.Trace("Writing XML file");
      await Task.Run(() => WriteXmlFile(), token).ConfigureAwait(false);
    }

    private void WriteXmlFile()
    {
      _xmlDocument.Save(_fileManagerFile, SaveOptions.None);
    }

    /// <summary>
    /// Add new tailData to XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">TailData to add</param>
    /// <returns>Task</returns>
    public async Task AddTailDataToXmlFileAsync(CancellationToken token, TailData tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));
      LOG.Trace("Add TailData to XML");

      await Task.Run(async () =>
      {
        try
        {
          if ( !File.Exists(_fileManagerFile) )
          {
            // Adds XML version
            _xmlDocument = new XDocument(new XElement(XmlNames.FileManagerXmlRoot));
            _xmlDocument.Root?.Add(new XElement(XmlNames.XmlVersion, XmlNames.CurrentXmlVersion));
          }
          else
          {
            // Updates XML version
            _xmlDocument.Root?.Element(XmlNames.XmlVersion)?.SetValue(XmlNames.CurrentXmlVersion);
          }

          if ( tailData.FileEncoding == null && !tailData.IsWindowsEvent )
          {
            try
            {
              tailData.FileEncoding = await EncodingDetector.GetEncodingAsync(tailData.FileName).ConfigureAwait(false);
            }
            catch
            {
              InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("FileNotFound").ToString());
              return;
            }
          }

          if ( tailData.FontType == null )
            tailData.FontType = new FontType();

          var node = new XElement(XmlNames.File,
            new XElement(XmlNames.Id, tailData.Id),
            new XElement(XmlNames.FileName, tailData.IsWindowsEvent ? string.Empty : tailData.FileName),
            new XElement(XmlNames.Description, tailData.Description?.Trim()),
            new XElement(XmlNames.Category, tailData.Category?.Trim()),
            new XElement(XmlNames.ThreadPriority, tailData.ThreadPriority),
            new XElement(XmlNames.NewWindow, tailData.NewWindow),
            new XElement(XmlNames.RefreshRate, tailData.RefreshRate),
            new XElement(XmlNames.TimeStamp, tailData.Timestamp),
            new XElement(XmlNames.RemoveSpace, tailData.RemoveSpace),
            new XElement(XmlNames.LineWrap, tailData.Wrap),
            new XElement(XmlNames.FileEncoding, tailData.IsWindowsEvent ? string.Empty : tailData.FileEncoding?.HeaderName),
            new XElement(XmlNames.UseFilters, tailData.FilterState),
            new XElement(XmlNames.UsePattern, !tailData.IsWindowsEvent && tailData.UsePattern),
            new XElement(XmlNames.UseSmartWatch, !tailData.IsWindowsEvent && tailData.SmartWatch),
            new XElement(XmlNames.TabItemBackgroundColor, tailData.TabItemBackgroundColorStringHex),
            new XElement(XmlNames.IsWindowsEvent, tailData.IsWindowsEvent),
            new XElement(XmlNames.WindowsEvent,
              new XElement(XmlNames.WindowsEventName, tailData.IsWindowsEvent ? tailData.WindowsEvent.Name : string.Empty),
              new XElement(XmlNames.WindowsEventMachineName, tailData.IsWindowsEvent ? tailData.WindowsEvent.Machine : string.Empty),
              new XElement(XmlNames.WindowsEventUserName, tailData.IsWindowsEvent ? tailData.WindowsEvent.UserName : string.Empty),
              new XElement(XmlNames.WindowsEventCategory, tailData.IsWindowsEvent ? tailData.WindowsEvent.Category : string.Empty)),
            new XElement(XmlNames.Font,
              new XElement(XmlBaseStructure.Name, tailData.FontType.FontFamily.Source),
              new XElement(XmlNames.Size, tailData.FontType.FontSize),
              new XElement(XmlNames.Weight, tailData.FontType.FontWeight),
              new XElement(XmlNames.Style, tailData.FontType.FontStyle)),
            new XElement(XmlNames.SearchPattern,
              new XElement(XmlBaseStructure.IsRegex, tailData.FindSettings.UseRegex),
              new XElement(XmlBaseStructure.UseWildcard, tailData.FindSettings.UseWildcard),
              new XElement(XmlBaseStructure.PatternString, tailData.PatternString)));

          var filters = new XElement(XmlNames.Filters);

          Parallel.ForEach(tailData.ListOfFilter, filter =>
          {
            filters.Add(AddFilterToDoc(filter));
          });

          node.Add(filters);
          _xmlDocument.Root?.Add(node);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          InteractionService.ShowErrorMessageBox(ex.Message);
        }
      }, token).ConfigureAwait(false);

      await WriteXmlFileAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData"><c>TailData</c> to update</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If tailData is null</exception>
    public async Task UpdateTailDataInXmlFileAsync(CancellationToken token, TailData tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));
      LOG.Trace("Update TailData");

      await Task.Run(() => UpdateTailDataInXmlFile(tailData), token).ConfigureAwait(false);
      await WriteXmlFileAsync(token).ConfigureAwait(false);
    }

    private void UpdateTailDataInXmlFile(TailData tailData)
    {
      try
      {
        XElement updateNode = _xmlDocument.Root?.Descendants(XmlNames.File)
          .SingleOrDefault(p => p.Element(XmlNames.Id)?.Value == tailData.Id.ToString());

        if ( updateNode == null )
          return;

        updateNode.Element(XmlNames.FileName)?.SetValue(tailData.IsWindowsEvent ? string.Empty : tailData.FileName);
        updateNode.Element(XmlNames.Description)?.SetValue(tailData.Description);
        updateNode.Element(XmlNames.Category)?.SetValue(tailData.Category ?? string.Empty);
        updateNode.Element(XmlNames.ThreadPriority)?.SetValue(tailData.ThreadPriority);
        updateNode.Element(XmlNames.NewWindow)?.SetValue(tailData.NewWindow);
        updateNode.Element(XmlNames.RefreshRate)?.SetValue(tailData.RefreshRate);
        updateNode.Element(XmlNames.TimeStamp)?.SetValue(tailData.Timestamp);
        updateNode.Element(XmlNames.RemoveSpace)?.SetValue(tailData.RemoveSpace);
        updateNode.Element(XmlNames.LineWrap)?.SetValue(tailData.Wrap);
        updateNode.Element(XmlNames.FileEncoding)?.SetValue(tailData.IsWindowsEvent ? string.Empty : tailData.FileEncoding?.HeaderName ?? string.Empty);
        updateNode.Element(XmlNames.UseFilters)?.SetValue(tailData.FilterState);
        updateNode.Element(XmlNames.UsePattern)?.SetValue(!tailData.IsWindowsEvent && tailData.UsePattern);
        updateNode.Element(XmlNames.IsWindowsEvent)?.SetValue(tailData.IsWindowsEvent);

        updateNode.Element(XmlNames.WindowsEvent)?.Element(XmlNames.WindowsEventName)?.SetValue(tailData.IsWindowsEvent ? tailData.WindowsEvent.Name : string.Empty);
        updateNode.Element(XmlNames.WindowsEvent)?.Element(XmlNames.WindowsEventMachineName)?.SetValue(tailData.IsWindowsEvent ? tailData.WindowsEvent.Machine : string.Empty);
        updateNode.Element(XmlNames.WindowsEvent)?.Element(XmlNames.WindowsEventUserName)?.SetValue(tailData.IsWindowsEvent ? tailData.WindowsEvent.UserName : string.Empty);
        updateNode.Element(XmlNames.WindowsEvent)?.Element(XmlNames.WindowsEventCategory)?.SetValue(tailData.IsWindowsEvent ? tailData.WindowsEvent.Category : string.Empty);

        updateNode.Element(XmlNames.TabItemBackgroundColor)?.SetValue(tailData.TabItemBackgroundColorStringHex ?? string.Empty);
        updateNode.Element(XmlNames.UseSmartWatch)?.SetValue(!tailData.IsWindowsEvent && tailData.SmartWatch);

        updateNode.Element(XmlNames.Font)?.Element(XmlBaseStructure.Name)?.SetValue(tailData.FontType.FontFamily.Source);
        updateNode.Element(XmlNames.Font)?.Element(XmlNames.Size)?.SetValue(tailData.FontType.FontSize);
        updateNode.Element(XmlNames.Font)?.Element(XmlNames.Weight)?.SetValue(tailData.FontType.FontWeight);
        updateNode.Element(XmlNames.Font)?.Element(XmlNames.Style)?.SetValue(tailData.FontType.FontStyle);

        updateNode.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.IsRegex)?.SetValue(tailData.FindSettings.UseRegex);
        updateNode.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.UseWildcard)?.SetValue(tailData.FindSettings.UseWildcard);
        updateNode.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.PatternString)?.SetValue(tailData.PatternString ?? string.Empty);

        // Remove all filters from document
        updateNode.Element(XmlNames.Filters)?.RemoveAll();

        XElement filters = updateNode.Element(XmlNames.Filters);

        Parallel.ForEach(
          tailData.ListOfFilter,
          filter =>
          {
            bool error = filter["Description"] != null || filter["Filter"] != null || filter["FilterSource"] != null || filter["IsHighlight"] != null;

            if ( error )
              return;

            filters?.Add(AddFilterToDoc(filter));
          });
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
    }

    /// <summary>
    /// Delete <c>TailData</c> from XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="id">Id to remove from XML scheme</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If <c>XML document</c> is null or <c>id</c> is empty</exception>
    public async Task DeleteTailDataByIdFromXmlFileAsync(CancellationToken token, string id)
    {
      Arg.NotNull(id, nameof(id));
      Arg.NotNull(_xmlDocument, nameof(_xmlDocument));
      LOG.Trace("Delete TailData by '{0}'", id);

      await Task.Run(() => DeleteTailDataByIdFromXmlFile(id), token).ConfigureAwait(false);
      await WriteXmlFileAsync(token).ConfigureAwait(false);
    }

    private void DeleteTailDataByIdFromXmlFile(string id)
    {
      try
      {
        _xmlDocument.Root?.Descendants(XmlNames.File).Where(p => p.Element(XmlNames.Id)?.Value == id).Remove();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
    }

    /// <summary>
    /// Delete a filter element from XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="id">Id of parent XML element</param>
    /// <param name="filterId">Id of filter to remove</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If <c>id</c> or <c>filterId</c> is null or empty</exception>
    public async Task DeleteFilterByIdByTailDataIdFromXmlFileAsync(CancellationToken token, string id, string filterId)
    {
      Arg.NotNull(id, nameof(id));
      Arg.NotNull(filterId, nameof(filterId));
      Arg.NotNull(_xmlDocument, nameof(_xmlDocument));
      LOG.Trace("Delete filter from XML id '{0}'", id);

      await Task.Run(() => DeleteFilterByIdByTailDataIdFromXmlFile(id, filterId), token).ConfigureAwait(false);
      await WriteXmlFileAsync(token).ConfigureAwait(false);
    }

    private void DeleteFilterByIdByTailDataIdFromXmlFile(string id, string filterId)
    {
      try
      {
        var updateNode = _xmlDocument.Root?.Descendants(XmlNames.File).SingleOrDefault(p => p.Element(XmlNames.Id)?.Value == id);

        if ( updateNode == null )
          return;

        updateNode.Element(XmlNames.Filters)?.Descendants(XmlNames.Filter).Where(p => p.Element(XmlNames.Id)?.Value == filterId).Remove();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
    }

    /// <summary>
    /// Get <c>TailData</c> by certain Id
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <param name="id">Id</param>
    /// <returns><c>TailData</c>, otherwise <c>Null</c></returns>
    /// <exception cref="ArgumentException">If <c>tailData</c> or <c>id</c> is empty</exception>
    public async Task<TailData> GetTailDataByIdAsync(ObservableCollection<TailData> tailData, Guid id)
    {
      Arg.NotNull(tailData, nameof(tailData));

      if ( id == Guid.Empty )
        throw new ArgumentException();

      LOG.Trace("Get TailData by '{0}", id);

      return await Task.Run(() => GetTailDataById(tailData, id), new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);
    }

    private TailData GetTailDataById(ObservableCollection<TailData> tailData, Guid id)
    {
      var result = new TailData();

      try
      {
        result = tailData.SingleOrDefault(p => p.Id.Equals(id));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        InteractionService.ShowErrorMessageBox(ex.Message);
      }
      return result;
    }

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

    private static XElement AddFilterToDoc(FilterData filter)
    {
      if ( filter.FontType == null )
        filter.FontType = new FontType();

      var newFilterElement = new XElement(XmlNames.Filter,
        new XElement(XmlNames.Id, filter.Id),
        new XElement(XmlNames.FilterName, filter.Description),
        new XElement(XmlNames.FilterPattern, filter.Filter),
        new XElement(XmlNames.FilterColor, filter.FilterColorHex),
        new XElement(XmlNames.FilterMatchCase, filter.FindSettingsData.CaseSensitive),
        new XElement(XmlNames.FilterMatchWholeWord, filter.FindSettingsData.WholeWord),
        new XElement(XmlNames.FilterRegex, filter.FindSettingsData.UseRegex),
        new XElement(XmlNames.FilterUseWildcard, filter.FindSettingsData.UseWildcard),
        new XElement(XmlNames.FilterIsHighlight, filter.IsHighlight),
        new XElement(XmlNames.FilterNotification, filter.UseNotification),
        new XElement(XmlNames.FilterSource, filter.FilterSource),
        new XElement(XmlNames.IsEnabled, filter.IsEnabled),
        new XElement(XmlNames.Font,
          new XElement(XmlBaseStructure.Name, filter.FontType.FontFamily.Source),
          new XElement(XmlNames.Size, filter.FontType.FontSize),
          new XElement(XmlNames.Weight, filter.FontType.FontWeight),
          new XElement(XmlNames.Style, filter.FontType.FontStyle)));

      return newFilterElement;
    }

    #endregion
  }
}
