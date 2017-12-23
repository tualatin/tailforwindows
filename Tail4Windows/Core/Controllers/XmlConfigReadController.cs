using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// XML config read controller
  /// </summary>
  public class XmlConfigReadController : IXmlReader
  {
    private readonly string _fileManagerFile;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlConfigReadController()
    {
      _fileManagerFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\FileManager.xml";
    }

    /// <summary>
    /// Read XML config file
    /// </summary>
    /// <returns>List of tail settings from XML file</returns>
    public Task<ObservableCollection<TailData>> ReadXmlFile()
    {
      if ( !File.Exists(_fileManagerFile) )
        throw new FileNotFoundException();

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
