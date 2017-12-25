using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces;


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
    public Task<ObservableCollection<TailData>> ReadXmlFile()
    {
      if ( !File.Exists(_fileManagerFile) )
        throw new FileNotFoundException();

      try
      {
        XDocument xmlDocument = XDocument.Load(_fileManagerFile);

        if ( xmlDocument.Root == null )
          throw new XmlException(Application.Current.TryFindResource("XmlExceptionConfigFileCorrupt").ToString());

        var version = xmlDocument.Root.Element(XmlStructure.XmlVersion);
        var files = xmlDocument.Root.Descendants(XmlStructure.File).Select(p => new TailData
        {
          Id = GetIdByElement(p.Element(XmlStructure.Id)?.Value),
          Description = p.Element(XmlStructure.Description)?.Value,
          File = p.Element(XmlStructure.FileName)?.Value,
          Category = p.Element(XmlStructure.Category)?.Value,
          Wrap = (p.Element(XmlStructure.LineWrap)?.Value).ConvertToBool(),
          RemoveSpace = (p.Element(XmlStructure.RemoveSpace)?.Value).ConvertToBool(),
          Timestamp = (p.Element(XmlStructure.TimeStamp)?.Value).ConvertToBool(),
          NewWindow = (p.Element(XmlStructure.NewWindow)?.Value).ConvertToBool(),
          SmartWatch = (p.Element(XmlStructure.UseSmartWatch)?.Value).ConvertToBool(),
          UsePattern = (p.Element(XmlStructure.UsePattern)?.Value).ConvertToBool()
        });


      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        throw;
      }
      return null;
    }

    public Task WriteXmlFile()
    {
      throw new NotImplementedException();
    }

    public Task UpdateXmlFile()
    {
      throw new NotImplementedException();
    }

    public Task DeleteXmlElement()
    {
      throw new NotImplementedException();
    }

    public Task<TailData> GetNodeById(string id)
    {
      throw new NotImplementedException();
    }

    #region HelperFunctions

    private Guid GetIdByElement(string id)
    {
      if ( !Guid.TryParse(id, out var result) )
        result = Guid.NewGuid();

      return result;
    }

    #endregion
  }
}
