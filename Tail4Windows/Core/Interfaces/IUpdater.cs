using System;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Update interface
  /// </summary>
  public interface IUpdater
  {
    /// <summary>
    /// Current Web version
    /// </summary>
    Version WebVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Current Application version
    /// </summary>
    Version AppVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Do check if main application needs to update
    /// </summary>
    /// <returns>Should update <c>True</c> otherwise <c>False</c></returns>
    Task<bool> UpdateNecessaryAsync();
  }
}
