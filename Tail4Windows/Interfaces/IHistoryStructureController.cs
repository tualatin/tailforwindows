using System.Xml.Linq;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// History structure controller interface
  /// </summary>
  public interface IHistoryStructureController
  {
    /// <summary>
    /// Wrap around in search dialogue
    /// </summary>
    bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Read find history section in XML file
    /// </summary>
    /// <param name="words">History words</param>
    void ReadFindHistory(ref ObservableDictionary<string, string> words);

    /// <summary>
    /// Save find history attribute name
    /// </summary>
    /// <param name="searchWord">Find what word</param>
    void SaveFindHistoryName(string searchWord);

    /// <summary>
    /// Save find history attribute wrap
    /// </summary>
    XElement SaveFindHistoryWrap();
  }
}