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
    /// Starts to dequeue the file queue
    /// </summary>
    void StartFileQueue();

    /// <summary>
    /// Adds file to current session
    /// </summary>
    /// <param name="data">Data as <see cref="StatisticData"/></param>
    void AddFileToCurrentSession(StatisticData data);

    /// <summary>
    /// Adds file to current queue
    /// </summary>
    /// <param name="data">Data as <see cref="StatisticData"/></param>
    void AddFileToQueue(StatisticData data);

    /// <summary>
    /// Saves file to current session
    /// </summary>
    /// <param name="data">Data as <see cref="StatisticData"/></param>
    void SaveFileToCurrentSession(StatisticData data);

    /// <summary>
    /// Stops statistics
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Starts analysis
    /// </summary>
    /// <returns><see cref="StatisticData"/></returns>
    Task<StatisticAnalysisData> StartAnalysisAsync();
  }
}
