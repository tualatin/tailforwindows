using System;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Utils
{
  /// <summary>
  /// Prevent message flood util
  /// </summary>
  public class PreventMessageFlood : IPreventMessageFlood
  {
    /// <summary>
    /// IsBusy indicator
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    /// <summary>
    /// Updates busy state
    /// </summary>
    public void UpdateBusyState()
    {
      IsBusy = true;
      NotifyTaskCompletion.Create(PreventFloodingAsync);
    }

    private async Task PreventFloodingAsync()
    {
      await Task.Delay(TimeSpan.FromMinutes(5)).ConfigureAwait(false);
      IsBusy = false;
    }
  }
}
