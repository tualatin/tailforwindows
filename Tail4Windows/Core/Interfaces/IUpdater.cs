using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Update interface
  /// </summary>
  public interface IUpdater
  {
    /// <summary>
    /// Do check if main application needs to update
    /// </summary>
    /// <returns>Should update <c>True</c> otherwise <c>False</c></returns>
    Task<UpdateData> UpdateNecessaryAsync();
  }
}
