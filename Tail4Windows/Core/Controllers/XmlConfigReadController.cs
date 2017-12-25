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
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// XML config read controller
  /// </summary>
  public class XmlConfigReadController : IXmlReader
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlConfigReadController));

    private readonly string _fileManagerFile;
    private XDocument _xmlDocument;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlConfigReadController()
    {
      _fileManagerFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\FileManager.xml";
    }

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlConfigReadController(string path)
    {
      _fileManagerFile = path;
    }

    /// <summary>
    /// Read XML config file
    /// </summary>
    /// <returns>List of tail settings from XML file</returns>
    /// <exception cref="FileNotFoundException">If XML file does not exists</exception>
    /// <exception cref="XmlException">If an error occurred while reading XML file</exception>
    public async Task<ObservableCollection<TailData>> ReadXmlFile()
    {
      if ( !File.Exists(_fileManagerFile) )
        throw new FileNotFoundException();

      LOG.Trace("Read XML T4W config file");

      try
      {
        ObservableCollection<TailData> result = new ObservableCollection<TailData>();
        await Task.Run(() =>
        {
          _xmlDocument = XDocument.Load(_fileManagerFile);

          if ( _xmlDocument.Root == null )
            return;

          var xmlVersion = _xmlDocument.Root.Element(XmlStructure.XmlVersion)?.Value.ConvertToDecimal();
          decimal version;

          if ( xmlVersion.HasValue )
            version = xmlVersion.Value == decimal.MinValue ? XmlStructure.CurrentXmlVersion : xmlVersion.Value;
          else
            version = XmlStructure.CurrentXmlVersion;

          var files = _xmlDocument.Root.Descendants(XmlStructure.File).Select(p => new TailData
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
            ThreadPriority = SettingsHelper.GetThreadPriority(p.Element(XmlStructure.ThreadPriority)?.Value),
            RefreshRate = SettingsHelper.GetRefreshRate(p.Element(XmlStructure.RefreshRate)?.Value),
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
              FilterColor = GetColor(x.Element(XmlStructure.FilterColor)?.Value)
            }).ToList() ?? throw new InvalidOperationException())
          }).ToList();

          result = new ObservableCollection<TailData>(files);
        });
        return result;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        throw;
      }
    }

    /// <summary>
    /// Get list of categories
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <returns>List of all categories</returns>
    /// <exception cref="ArgumentException">If <c>tailData</c> is null</exception>
    public async Task<ObservableCollection<string>> GetCategories(ObservableCollection<TailData> tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));

      LOG.Trace("Get all categories from XML T4W config file");

      ObservableCollection<string> result = new ObservableCollection<string>();

      try
      {
        await Task.Run(() =>
        {
          var categories = tailData.Select(p => p.Category).ToList();
          result = new ObservableCollection<string>(categories);
        });
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return result;
    }

    /// <summary>
    /// Write XML config file
    /// </summary>
    /// <returns>Task</returns>
    public async Task WriteXmlFile()
    {
      await Task.Run(() =>
      {
        _xmlDocument.Save(_fileManagerFile, SaveOptions.None);
      });
    }

    /// <summary>
    /// Update XML config file
    /// </summary>
    /// <param name="tailData">TailData</param>
    /// <returns>Task</returns>
    public async Task UpdateXmlFile(TailData tailData)
    {
      Arg.NotNull(tailData, nameof(tailData));

      await Task.Run(() =>
      {
        if ( !File.Exists(_fileManagerFile) )
          _xmlDocument = new XDocument(new XElement(XmlStructure.XmlRoot));

        if ( tailData.FileEncoding == null )
        {
          // TODO encoding
        }

        try
        {
          if ( tailData.FontType == null )
            tailData.FontType = CreateDefaultFont();

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

          foreach ( var filter in tailData.ListOfFilter )
          {
            filters.Add(AddFilterToDoc(filter));
          }

          node.Add(filters);
          _xmlDocument.Add(node);
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      });
      await WriteXmlFile();
    }

    /// <summary>
    /// Delete XML node from config file
    /// </summary>
    /// <param name="id">Id to remove from XML scheme</param>
    /// <returns>Task</returns>
    /// <exception cref="ArgumentException">If <c>XML document</c> is null or <c>id</c> is empty</exception>
    public async Task DeleteXmlElement(string id)
    {
      Arg.NotNull(id, nameof(id));
      Arg.NotNull(_xmlDocument, nameof(_xmlDocument));

      await Task.Run(() =>
      {
        if ( _xmlDocument.Root == null )
          return;

        _xmlDocument.Root.Descendants(XmlStructure.File).Where(p => p.Element(XmlStructure.Id)?.Value == id).Remove();
      });
      await WriteXmlFile();
    }

    /// <summary>
    /// Delete a filter element from XML config file
    /// </summary>
    /// <param name="id">Id of parent XML element</param>
    /// <param name="filterId">Id of filter to remove</param>
    /// <returns>Task</returns>
    public async Task DeleteFilterElement(string id, string filterId)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Get XML node by certain Id
    /// </summary>
    /// <param name="tailData">List of TailData</param>
    /// <param name="id">Id</param>
    /// <returns><c>TailData</c>, otherwise <c>Null</c></returns>
    /// <exception cref="ArgumentException">If <c>tailData</c> or <c>id</c> is empty</exception>
    public async Task<TailData> GetNodeById(ObservableCollection<TailData> tailData, Guid id)
    {
      Arg.NotNull(tailData, nameof(tailData));

      if ( id == Guid.Empty )
        throw new ArgumentException();

      LOG.Trace("Get XML node by '{0}", id);

      TailData result = new TailData();

      try
      {
        await Task.Run(() =>
        {
          result = tailData.SingleOrDefault(p => p.Id.Equals(id));
        });
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
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

      foreach ( Encoding encode in EnvironmentlContainer.Instance.FileEncoding )
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
        return CreateDefaultFont();

      var name = xmlFont.Element(XmlStructure.Name)?.Value;
      var size = (xmlFont.Element(XmlStructure.Size)?.Value).ConvertToFloat();
      var bold = (xmlFont.Element(XmlStructure.Bold)?.Value).ConvertToBool();
      var italic = (xmlFont.Element(XmlStructure.Italic)?.Value).ConvertToBool();

      FontStyle fs = FontStyle.Regular;
      fs |= bold ? FontStyle.Bold : FontStyle.Regular;
      fs |= italic ? FontStyle.Italic : FontStyle.Regular;

      return new Font(name, size, fs);
    }

    private static Font CreateDefaultFont()
    {
      return new Font("Segoe UI", 11f, FontStyle.Regular);
    }

    private static Color GetColor(string sColor)
    {
      return Color.Black;
    }

    private static XElement AddFilterToDoc(FilterData filter)
    {
      if ( filter.FilterFontType == null )
        filter.FilterFontType = CreateDefaultFont();

      XElement newFilterElement = new XElement(XmlStructure.Filter,
        new XElement(XmlStructure.Id, filter.Id),
        new XElement(XmlStructure.FilterName, filter.Description),
        new XElement(XmlStructure.FilterPattern, filter.Filter),
        new XElement(XmlStructure.FilterColor, filter.FilterColor),
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
