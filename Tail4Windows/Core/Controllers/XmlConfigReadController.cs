using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Data;
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
  }
}
