using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// XML FileManager interface
  /// </summary>
  [Obsolete("Please use IFileManager instead")]
  public interface IXmlFileManager
  {
    /// <summary>
    /// Gets current XML config file
    /// </summary>
    string XmlFileName
    {
      get;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>List of tail settings from XML file</returns>
    Task<ObservableCollection<TailData>> ReadXmlFileAsync(CancellationToken token);
  }
}
