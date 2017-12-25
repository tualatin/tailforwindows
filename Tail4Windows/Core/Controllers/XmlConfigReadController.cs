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
          XDocument xmlDocument = XDocument.Load(_fileManagerFile);

          if ( xmlDocument.Root == null )
            return;

          var xmlVersion = xmlDocument.Root.Element(XmlStructure.XmlVersion)?.Value.ConvertToDecimal();
          decimal version;

          if ( xmlVersion.HasValue )
            version = xmlVersion.Value == decimal.MinValue ? XmlStructure.CurrentXmlVersion : xmlVersion.Value;
          else
            version = XmlStructure.CurrentXmlVersion;

          var files = xmlDocument.Root.Descendants(XmlStructure.File).Select(p => new TailData
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
    /// <exception cref="ArgumentException">If tailData is null</exception>
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

    public async Task WriteXmlFile()
    {
      throw new NotImplementedException();
    }

    public async Task UpdateXmlFile()
    {
      throw new NotImplementedException();
    }

    public async Task DeleteXmlElement()
    {
      throw new NotImplementedException();
    }

    public async Task<TailData> GetNodeById(string id)
    {
      throw new NotImplementedException();
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
        return new Font("Tahoma", 11f, FontStyle.Regular);

      var name = xmlFont.Element(XmlStructure.Name)?.Value;
      var size = (xmlFont.Element(XmlStructure.Size)?.Value).ConvertToFloat();
      var bold = (xmlFont.Element(XmlStructure.Bold)?.Value).ConvertToBool();
      var italic = (xmlFont.Element(XmlStructure.Italic)?.Value).ConvertToBool();

      FontStyle fs = FontStyle.Regular;
      fs |= bold ? FontStyle.Bold : FontStyle.Regular;
      fs |= italic ? FontStyle.Italic : FontStyle.Regular;

      return new Font(name, size, fs);
    }

    private static Color GetColor(string sColor)
    {
      return Color.Black;
    }

    #endregion
  }
}
