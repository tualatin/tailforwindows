using System;
using System.Collections.ObjectModel;
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
    public Task<ObservableCollection<TailData>> ReadXmlFile()
    {
      throw new NotImplementedException();
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
