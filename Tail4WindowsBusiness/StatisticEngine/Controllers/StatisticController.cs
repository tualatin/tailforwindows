using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;


namespace Org.Vs.TailForWin.Business.StatisticEngine.Controllers
{
  /// <summary>
  /// Statistics controller
  /// </summary>
  public class StatisticController : IStatisticController
  {
    #region Properties

    /// <summary>
    /// Statistics is busy
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    #endregion

    /// <summary>
    /// Starts statistics
    /// </summary>
    public void Start()
    {
      IsBusy = true;
    }

    /// <summary>
    /// Stops statistics
    /// </summary>
    public async Task StopAsync()
    {
      IsBusy = false;
    }

    /// <summary>
    /// Starts analysis
    /// </summary>
    /// <returns><see cref="StatisticData"/></returns>
    public async Task<StatisticData> StartAnalysisAsync()
    {
      var result = new StatisticData();

      return result;
    }
  }
}
