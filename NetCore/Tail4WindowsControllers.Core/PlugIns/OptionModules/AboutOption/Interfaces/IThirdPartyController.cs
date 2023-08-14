using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Data;

namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Interfaces
{
  /// <summary>
  /// ThirdParty controller interface
  /// </summary>
  public interface IThirdPartyController
  {
    /// <summary>
    /// Get a <see cref="ObservableCollection{T}"/> of <see cref="ThirdPartyComponentData"/> of ThirdParty
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>A list of <see cref="ObservableCollection{T}"/> of <see cref="ThirdPartyComponentData"/></returns>
    Task<ObservableCollection<ThirdPartyComponentData>> GetThirdPartyComponentsAsync(CancellationToken token);
  }
}
