using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;

using FontStyle = System.Drawing.FontStyle;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller
{
  /// <summary>
  /// XML FileManager controller
  /// </summary>
  public class XmlFileManagerController : IXmlFileManager
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlFileManagerController));

    private readonly string _fileManagerFile;
    private XDocument _xmlDocument;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlFileManagerController()
    {
      _fileManagerFile = EnvironmentContainer.ApplicationPath + @"\FileManager.xml";
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlFileManagerController(string path)
    {
      _fileManagerFile = path;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>List of tail settings from XML file</returns>
    /// <exception cref="FileNotFoundException">If XML file does not exists</exception>
    /// <exception cref="XmlException">If an error occurred while reading XML file</exception>
    public async Task<ObservableCollection<TailData>> ReadXmlFileAsync()
    {
      if ( !File.Exists(_fileManagerFile) )
        throw new FileNotFoundException();

      LOG.Trace("Read XML");

      return await Task.Run(() => ReadXmlFile()).ConfigureAwait(false);
    }

    private ObservableCollection<TailData> ReadXmlFile()
    {
      ObservableCollection<TailData> result = new ObservableCollection<TailData>();

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
          TabItemBackgroundColorStringHex = GetColorAsString(p.Element(XmlNames.TabItemBackgroundColor)?.Value),
          ThreadPriority = EnvironmentContainer.GetThreadPriority(p.Element(XmlNames.ThreadPriority)?.Value),
          RefreshRate = EnvironmentContainer.GetRefreshRate(p.Element(XmlNames.RefreshRate)?.Value),
          FileEncoding = GetEncoding(p.Element(XmlNames.FileEncoding)?.Value),
          FilterState = (p.Element(XmlNames.UseFilters)?.Value).ConvertToBool(),
          FontType = GetFont(p.Element(XmlNames.Font)),
          IsRegex = (p.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.IsRegex)?.Value).ConvertToBool(),
          PatternString = p.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.PatternString)?.Value,
          ListOfFilter = new ObservableCollection<FilterData>(p.Element(XmlNames.Filters)?.Descendants(XmlNames.Filter).Select(x => new FilterData
          {
            Id = GetIdByElement(x.Element(XmlNames.Id)?.Value),
            Description = x.Element(XmlNames.FilterName)?.Value,
            Filter = x.Element(XmlNames.FilterPattern)?.Value,
            FilterFontType = GetFont(x.Element(XmlNames.Font)),
            FilterColor = EnvironmentContainer.ConvertHexStringToBrush(x.Element(XmlNames.FilterColor)?.Value, System.Windows.Media.Brushes.Black)
          }).ToList() ?? throw new InvalidOperationException())
        }).ToList();

        if ( files != null )
          result = new ObservableCollection<TailData>(files);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
      }
      return result;
    }

    private string GetColorAsString(string value) => string.IsNullOrWhiteSpace(value) ? "#FFD6DBE9" : value;

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
      ObservableCollection<string> result = new ObservableCollection<string>();

      try
      {
        var categories = tailData.Select(p => p.Category).ToList();
        result = new ObservableCollection<string>(categories);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
      }
      return result;
    }

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <returns>Task</returns>
    public async Task WriteXmlFileAsync()
    {
      LOG.Trace("Writing XML file");
      await Task.Run(() => WriteXmlFile()).ConfigureAwait(false);
    }

    private void WriteXmlFile()
    {
      _xmlDocument.Save(_fileManagerFile, SaveOptions.None);
    }

    /// <summary>
    /// Add new tailData to XML file
    /// </summary>
    /// <param name="tailData">TailData to add</param>
    /// <returns>Task</returns>
    public async Task AddTailDataToXmlFileAsync(TailData tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));
      LOG.Trace("Add TailData to XML");

      await Task.Run(async () =>
      {
        try
        {
          if ( !File.Exists(_fileManagerFile) )
          {
            _xmlDocument = new XDocument(new XElement(XmlNames.FileManagerXmlRoot));
            _xmlDocument.Root?.Add(new XElement(XmlNames.XmlVersion, XmlNames.CurrentXmlVersion));
          }

          if ( tailData.FileEncoding == null )
          {
            try
            {
              tailData.FileEncoding = await EncodingDetector.GetEncodingAsync(tailData.FileName).ConfigureAwait(false);
            }
            catch
            {
              EnvironmentContainer.ShowErrorMessageBox(Application.Current.TryFindResource("FileNotFound").ToString());
              return;
            }
          }

          if ( tailData.FontType == null )
            tailData.FontType = EnvironmentContainer.CreateDefaultFont();

          XElement node = new XElement(XmlNames.File,
            new XElement(XmlNames.Id, tailData.Id),
            new XElement(XmlNames.FileName, tailData.FileName),
            new XElement(XmlNames.Description, tailData.Description),
            new XElement(XmlNames.Category, tailData.Category),
            new XElement(XmlNames.ThreadPriority, tailData.ThreadPriority),
            new XElement(XmlNames.NewWindow, tailData.NewWindow),
            new XElement(XmlNames.RefreshRate, tailData.RefreshRate),
            new XElement(XmlNames.TimeStamp, tailData.Timestamp),
            new XElement(XmlNames.RemoveSpace, tailData.RemoveSpace),
            new XElement(XmlNames.LineWrap, tailData.Wrap),
            new XElement(XmlNames.FileEncoding, tailData.FileEncoding?.HeaderName),
            new XElement(XmlNames.UseFilters, tailData.FilterState),
            new XElement(XmlNames.UsePattern, tailData.UsePattern),
            new XElement(XmlNames.UseSmartWatch, tailData.SmartWatch),
            new XElement(XmlNames.TabItemBackgroundColor, tailData.TabItemBackgroundColorStringHex),
            new XElement(XmlNames.Font,
              new XElement(XmlBaseStructure.Name, tailData.FontType.Name),
              new XElement(XmlNames.Size, tailData.FontType.Size),
              new XElement(XmlNames.Bold, tailData.FontType.Bold),
              new XElement(XmlNames.Italic, tailData.FontType.Italic)),
            new XElement(XmlNames.SearchPattern,
              new XElement(XmlBaseStructure.IsRegex, tailData.IsRegex),
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
          EnvironmentContainer.ShowErrorMessageBox(ex.Message);
        }
      }).ConfigureAwait(false);

      await WriteXmlFileAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <param name="tailData"><c>TailData</c> to update</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If tailData is null</exception>
    public async Task UpdateTailDataInXmlFileAsync(TailData tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));
      LOG.Trace("Update TailData");

      await Task.Run(() => UpdateTailDataInXmlFile(tailData)).ConfigureAwait(false);
      await WriteXmlFileAsync().ConfigureAwait(false);
    }

    private void UpdateTailDataInXmlFile(TailData tailData)
    {
      try
      {
        var updateNode = _xmlDocument.Root?.Descendants(XmlNames.File)
          .SingleOrDefault(p => p.Element(XmlNames.Id)?.Value == tailData.Id.ToString());

        if ( updateNode == null )
          return;

        updateNode.Element(XmlNames.FileName)?.SetValue(tailData.FileName);
        updateNode.Element(XmlNames.Description)?.SetValue(tailData.Description);
        updateNode.Element(XmlNames.Category)?.SetValue(tailData.Category);
        updateNode.Element(XmlNames.ThreadPriority)?.SetValue(tailData.ThreadPriority);
        updateNode.Element(XmlNames.NewWindow)?.SetValue(tailData.NewWindow);
        updateNode.Element(XmlNames.RefreshRate)?.SetValue(tailData.RefreshRate);
        updateNode.Element(XmlNames.TimeStamp)?.SetValue(tailData.Timestamp);
        updateNode.Element(XmlNames.RemoveSpace)?.SetValue(tailData.RemoveSpace);
        updateNode.Element(XmlNames.LineWrap)?.SetValue(tailData.Wrap);
        updateNode.Element(XmlNames.FileEncoding)?.SetValue(tailData.FileEncoding?.HeaderName ?? string.Empty);
        updateNode.Element(XmlNames.UseFilters)?.SetValue(tailData.FilterState);
        updateNode.Element(XmlNames.UsePattern)?.SetValue(tailData.UsePattern);
        updateNode.Element(XmlNames.TabItemBackgroundColor)?.SetValue(tailData.TabItemBackgroundColorStringHex);
        updateNode.Element(XmlNames.UseSmartWatch)?.SetValue(tailData.SmartWatch);
        updateNode.Element(XmlNames.Font)?.Element(XmlBaseStructure.Name)?.SetValue(tailData.FontType.Name);
        updateNode.Element(XmlNames.Font)?.Element(XmlNames.Size)?.SetValue(tailData.FontType.Size);
        updateNode.Element(XmlNames.Font)?.Element(XmlNames.Bold)?.SetValue(tailData.FontType.Bold);
        updateNode.Element(XmlNames.Font)?.Element(XmlNames.Italic)?.SetValue(tailData.FontType.Italic);
        updateNode.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.IsRegex)?.SetValue(tailData.IsRegex);
        updateNode.Element(XmlNames.SearchPattern)?.Element(XmlBaseStructure.PatternString)?.SetValue(tailData.PatternString);

        // Remove all filters from document
        updateNode.Element(XmlNames.Filters)?.RemoveAll();

        var filters = updateNode.Element(XmlNames.Filters);

        Parallel.ForEach(
          tailData.ListOfFilter,
          filter =>
          {
            filters?.Add(AddFilterToDoc(filter));
          });
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
      }
    }

    /// <summary>
    /// Delete <c>TailData</c> from XML file
    /// </summary>
    /// <param name="id">Id to remove from XML scheme</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If <c>XML document</c> is null or <c>id</c> is empty</exception>
    public async Task DeleteTailDataByIdFromXmlFileAsync(string id)
    {
      Arg.NotNull(id, nameof(id));
      Arg.NotNull(_xmlDocument, nameof(_xmlDocument));
      LOG.Trace("Delete TailData by '{0}'", id);

      await Task.Run(() => DeleteTailDataByIdFromXmlFile(id)).ConfigureAwait(false);
      await WriteXmlFileAsync().ConfigureAwait(false);
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
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
      }
    }

    /// <summary>
    /// Delete a filter element from XML file
    /// </summary>
    /// <param name="id">Id of parent XML element</param>
    /// <param name="filterId">Id of filter to remove</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If <c>id</c> or <c>filterId</c> is null or empty</exception>
    public async Task DeleteFilterByIdByTailDataIdFromXmlFileAsync(string id, string filterId)
    {
      Arg.NotNull(id, nameof(id));
      Arg.NotNull(filterId, nameof(filterId));
      Arg.NotNull(_xmlDocument, nameof(_xmlDocument));
      LOG.Trace("Delete filter from XML id '{0}'", id);

      await Task.Run(() => DeleteFilterByIdByTailDataIdFromXmlFile(id, filterId)).ConfigureAwait(false);
      await WriteXmlFileAsync().ConfigureAwait(false);
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
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
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

      return await Task.Run(() => GetTailDataById(tailData, id)).ConfigureAwait(false);
    }

    private TailData GetTailDataById(ObservableCollection<TailData> tailData, Guid id)
    {
      TailData result = new TailData();

      try
      {
        result = tailData.SingleOrDefault(p => p.Id.Equals(id));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        EnvironmentContainer.ShowErrorMessageBox(ex.Message);
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

    private static Font GetFont(XContainer xmlFont)
    {
      if ( xmlFont == null )
        return EnvironmentContainer.CreateDefaultFont();

      var font = new
      {
        Name = xmlFont.Element(XmlBaseStructure.Name)?.Value,
        Size = (xmlFont.Element(XmlNames.Size)?.Value).ConvertToFloat(),
        Bold = (xmlFont.Element(XmlNames.Bold)?.Value).ConvertToBool(),
        Iitalic = (xmlFont.Element(XmlNames.Italic)?.Value).ConvertToBool()
      };

      FontStyle fs = FontStyle.Regular;
      fs |= font.Bold ? FontStyle.Bold : FontStyle.Regular;
      fs |= font.Iitalic ? FontStyle.Italic : FontStyle.Regular;

      return new Font(font.Name, font.Size, fs);
    }

    private static XElement AddFilterToDoc(FilterData filter)
    {
      if ( filter.FilterFontType == null )
        filter.FilterFontType = EnvironmentContainer.CreateDefaultFont();

      XElement newFilterElement = new XElement(XmlNames.Filter,
        new XElement(XmlNames.Id, filter.Id),
        new XElement(XmlNames.FilterName, filter.Description),
        new XElement(XmlNames.FilterPattern, filter.Filter),
        new XElement(XmlNames.FilterColor, filter.FilterColor.ToString()),
        new XElement(XmlNames.Font,
          new XElement(XmlBaseStructure.Name, filter.FilterFontType.Name),
          new XElement(XmlNames.Size, filter.FilterFontType.Size),
          new XElement(XmlNames.Bold, filter.FilterFontType.Bold),
          new XElement(XmlNames.Italic, filter.FilterFontType.Italic)));

      return newFilterElement;
    }

    #endregion
  }
}
