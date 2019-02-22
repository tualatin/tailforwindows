using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// XML search history interface
  /// </summary>
  /// <typeparam name="T">Type of interface</typeparam>
  public interface IXmlSearchHistory<T>
  {
    /// <summary>
    /// Wrap at the end of search
    /// </summary>
    bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    Task<T> ReadXmlFileAsync();
  }
}
