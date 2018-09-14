using System;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;


namespace Org.Vs.TailForWin.Business.StatisticEngine.Interfaces
{
  /// <summary>
  /// Statistics controller interface
  /// </summary>
  public interface IStatisticController
  {
    #region Properties

    /// <summary>
    /// Statistics is busy
    /// </summary>
    bool IsBusy
    {
      get;
    }

    /// <summary>
    /// Current Session ID
    /// </summary>
    Guid SessionId
    {
      get;
    }

    #endregion

    /// <summary>
    /// Starts statistics
    /// </summary>
    void Start();

    /// <summary>
    /// Stops statistics
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Starts analysis
    /// </summary>
    /// <returns><see cref="StatisticData"/></returns>
    Task<StatisticData> StartAnalysisAsync();
  }
}
