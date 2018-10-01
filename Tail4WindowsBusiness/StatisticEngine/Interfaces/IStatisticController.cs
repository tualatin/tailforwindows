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
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="fileName">Name of file with path</param>
    /// <param name="isWindowsEvent">Is Windows event</param>
    void AddFileToCurrentSession(Guid logReaderId, int index, string fileName, bool isWindowsEvent = false);

    /// <summary>
    /// Adds file to current queue
    /// </summary>
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="elapsedTime">Elapsed time</param>
    /// <param name="fileName">Name of file with path</param>
    /// <param name="isWindowsEvent">Is Windows event</param>
    void AddFileToQueue(Guid logReaderId, int index, TimeSpan? elapsedTime, string fileName, bool isWindowsEvent = false);

    /// <summary>
    /// Saves file to current session
    /// </summary>
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="elapsedTime">Elapsed time</param>
    /// <param name="fileName">Name of file with path</param>
    void SaveFileToCurrentSession(Guid logReaderId, int index, TimeSpan elapsedTime, string fileName);

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
