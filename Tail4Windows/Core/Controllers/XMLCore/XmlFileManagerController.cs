using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces.XMLCore;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers.XMLCore
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
      _fileManagerFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\FileManager.xml";
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
        var xmlVersion = _xmlDocument.Root?.Element(XmlStructure.XmlVersion)?.Value.ConvertToDecimal();
        decimal version;

        if ( xmlVersion.HasValue )
          version = xmlVersion.Value == decimal.MinValue ? XmlStructure.CurrentXmlVersion : xmlVersion.Value;
        else
          version = XmlStructure.CurrentXmlVersion;

        var files = _xmlDocument.Root?.Descendants(XmlStructure.File).AsParallel().Select(p => new TailData
        {
          Version = version,
          Id = GetIdByElement(p.Element(XmlStructure.Id)?.Value),
          Description = p.Element(XmlStructure.Description)?.Value,
          FileName = p.Element(XmlStructure.FileName)?.Value,
          OriginalFileName = p.Element(XmlStructure.FileName)?.Value,
          Category = p.Element(XmlStructure.Category)?.Value,
          Wrap = (p.Element(XmlStructure.LineWrap)?.Value).ConvertToBool(),
          RemoveSpace = (p.Element(XmlStructure.RemoveSpace)?.Value).ConvertToBool(),
          Timestamp = (p.Element(XmlStructure.TimeStamp)?.Value).ConvertToBool(),
          NewWindow = (p.Element(XmlStructure.NewWindow)?.Value).ConvertToBool(),
          SmartWatch = (p.Element(XmlStructure.UseSmartWatch)?.Value).ConvertToBool(),
          UsePattern = (p.Element(XmlStructure.UsePattern)?.Value).ConvertToBool(),
          ThreadPriority = EnvironmentContainer.GetThreadPriority(p.Element(XmlStructure.ThreadPriority)?.Value),
          RefreshRate = EnvironmentContainer.GetRefreshRate(p.Element(XmlStructure.RefreshRate)?.Value),
          FileEncoding = GetEncoding(p.Element(XmlStructure.FileEncoding)?.Value),
          FilterState = (p.Element(XmlStructure.UseFilters)?.Value).ConvertToBool(),
          FontType = GetFont(p.Element(XmlStructure.Font)),
          IsRegex = (p.Element(XmlStructure.SearchPattern)?.Element(XmlStructure.IsRegex)?.Value).ConvertToBool(),
          PatternString = p.Element(XmlStructure.SearchPattern)?.Element(XmlStructure.PatternString)?.Value,
          ListOfFilter = new ObservableCollection<FilterData>(p.Element(XmlStructure.Filters)?.Descendants(XmlStructure.Filter).Select(x => new FilterData
          {
            Id = GetIdByElement(x.Element(XmlStructure.Id)?.Value),
            Description = x.Element(XmlStructure.FilterName)?.Value,
            Filter = x.Element(XmlStructure.FilterPattern)?.Value,
            FilterFontType = GetFont(x.Element(XmlStructure.Font)),
            FilterColor = EnvironmentContainer.ConvertHexStringToBrush(x.Element(XmlStructure.FilterColor)?.Value)
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

      await Task.Run(() => AddTailDataToXmlFile(tailData)).ConfigureAwait(false);
      await WriteXmlFileAsync().ConfigureAwait(false);
    }

    private void AddTailDataToXmlFile(TailData tailData)
    {
      try
      {
        if ( !File.Exists(_fileManagerFile) )
        {
          _xmlDocument = new XDocument(new XElement(XmlStructure.XmlRoot));
          _xmlDocument.Root?.Add(new XElement(XmlStructure.XmlVersion, XmlStructure.CurrentXmlVersion));
        }

        if ( tailData.FileEncoding == null )
        {
          tailData.FileEncoding = Encoding.UTF8;
          // TODO encoding
        }

        if ( tailData.FontType == null )
          tailData.FontType = EnvironmentContainer.CreateDefaultFont();

        XElement node = new XElement(XmlStructure.File,
          new XElement(XmlStructure.Id, tailData.Id),
          new XElement(XmlStructure.FileName, tailData.FileName),
          new XElement(XmlStructure.Description, tailData.Description),
          new XElement(XmlStructure.Category, tailData.Category),
          new XElement(XmlStructure.ThreadPriority, tailData.ThreadPriority),
          new XElement(XmlStructure.NewWindow, tailData.NewWindow),
          new XElement(XmlStructure.RefreshRate, tailData.RefreshRate),
          new XElement(XmlStructure.TimeStamp, tailData.Timestamp),
          new XElement(XmlStructure.RemoveSpace, tailData.RemoveSpace),
          new XElement(XmlStructure.LineWrap, tailData.Wrap),
          new XElement(XmlStructure.FileEncoding, tailData.FileEncoding?.HeaderName),
          new XElement(XmlStructure.UseFilters, tailData.FilterState),
          new XElement(XmlStructure.UsePattern, tailData.UsePattern),
          new XElement(XmlStructure.UseSmartWatch, tailData.SmartWatch),
          new XElement(XmlStructure.Font,
            new XElement(XmlStructure.Name, tailData.FontType.Name),
            new XElement(XmlStructure.Size, tailData.FontType.Size),
            new XElement(XmlStructure.Bold, tailData.FontType.Bold),
            new XElement(XmlStructure.Italic, tailData.FontType.Italic)),
          new XElement(XmlStructure.SearchPattern,
            new XElement(XmlStructure.IsRegex, tailData.IsRegex),
            new XElement(XmlStructure.PatternString, tailData.PatternString)));

        var filters = new XElement(XmlStructure.Filters);

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
        var updateNode = _xmlDocument.Root?.Descendants(XmlStructure.File)
          .SingleOrDefault(p => p.Element(XmlStructure.Id)?.Value == tailData.Id.ToString());

        if ( updateNode == null )
          return;

        updateNode.Element(XmlStructure.FileName)?.SetValue(tailData.FileName);
        updateNode.Element(XmlStructure.Description)?.SetValue(tailData.Description);
        updateNode.Element(XmlStructure.Category)?.SetValue(tailData.Category);
        updateNode.Element(XmlStructure.ThreadPriority)?.SetValue(tailData.ThreadPriority);
        updateNode.Element(XmlStructure.NewWindow)?.SetValue(tailData.NewWindow);
        updateNode.Element(XmlStructure.RefreshRate)?.SetValue(tailData.RefreshRate);
        updateNode.Element(XmlStructure.TimeStamp)?.SetValue(tailData.Timestamp);
        updateNode.Element(XmlStructure.RemoveSpace)?.SetValue(tailData.RemoveSpace);
        updateNode.Element(XmlStructure.LineWrap)?.SetValue(tailData.Wrap);
        updateNode.Element(XmlStructure.FileEncoding)?.SetValue(tailData.FileEncoding?.HeaderName ?? string.Empty);
        updateNode.Element(XmlStructure.UseFilters)?.SetValue(tailData.FilterState);
        updateNode.Element(XmlStructure.UsePattern)?.SetValue(tailData.UsePattern);
        updateNode.Element(XmlStructure.UseSmartWatch)?.SetValue(tailData.SmartWatch);
        updateNode.Element(XmlStructure.Font)?.Element(XmlStructure.Name)?.SetValue(tailData.FontType.Name);
        updateNode.Element(XmlStructure.Font)?.Element(XmlStructure.Size)?.SetValue(tailData.FontType.Size);
        updateNode.Element(XmlStructure.Font)?.Element(XmlStructure.Bold)?.SetValue(tailData.FontType.Bold);
        updateNode.Element(XmlStructure.Font)?.Element(XmlStructure.Italic)?.SetValue(tailData.FontType.Italic);
        updateNode.Element(XmlStructure.SearchPattern)?.Element(XmlStructure.IsRegex)?.SetValue(tailData.IsRegex);
        updateNode.Element(XmlStructure.SearchPattern)?.Element(XmlStructure.PatternString)?.SetValue(tailData.PatternString);

        // Remove all filters from document
        updateNode.Element(XmlStructure.Filters)?.RemoveAll();

        var filters = updateNode.Element(XmlStructure.Filters);

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
        _xmlDocument.Root?.Descendants(XmlStructure.File).Where(p => p.Element(XmlStructure.Id)?.Value == id).Remove();
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
        var updateNode = _xmlDocument.Root?.Descendants(XmlStructure.File).SingleOrDefault(p => p.Element(XmlStructure.Id)?.Value == id);

        if ( updateNode == null )
          return;

        updateNode.Element(XmlStructure.Filters)?.Descendants(XmlStructure.Filter).Where(p => p.Element(XmlStructure.Id)?.Value == filterId).Remove();
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
        Name = xmlFont.Element(XmlStructure.Name)?.Value,
        Size = (xmlFont.Element(XmlStructure.Size)?.Value).ConvertToFloat(),
        Bold = (xmlFont.Element(XmlStructure.Bold)?.Value).ConvertToBool(),
        Iitalic = (xmlFont.Element(XmlStructure.Italic)?.Value).ConvertToBool()
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

      XElement newFilterElement = new XElement(XmlStructure.Filter,
        new XElement(XmlStructure.Id, filter.Id),
        new XElement(XmlStructure.FilterName, filter.Description),
        new XElement(XmlStructure.FilterPattern, filter.Filter),
        new XElement(XmlStructure.FilterColor, filter.FilterColor.ToString()),
        new XElement(XmlStructure.Font,
          new XElement(XmlStructure.Name, filter.FilterFontType.Name),
          new XElement(XmlStructure.Size, filter.FilterFontType.Size),
          new XElement(XmlStructure.Bold, filter.FilterFontType.Bold),
          new XElement(XmlStructure.Italic, filter.FilterFontType.Italic)));

      return newFilterElement;
    }

    #endregion
  }
}
