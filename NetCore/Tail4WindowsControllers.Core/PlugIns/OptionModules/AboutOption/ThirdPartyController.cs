using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.AboutOption.Data;
using Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.AboutOption.Interfaces;
using Org.Vs.Tail4Win.Core.Utils;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.AboutOption
{
  /// <summary>
  /// ThirdParty controller
  /// </summary>
  public class ThirdPartyController : IThirdPartyController
  {
    /// <summary>
    /// Get a <see cref="ObservableCollection{T}"/> of <see cref="ThirdPartyComponentData"/> of ThirdParty
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>A list of <see cref="ObservableCollection{T}"/> of <see cref="ThirdPartyComponentData"/></returns>
    public async Task<ObservableCollection<ThirdPartyComponentData>> GetThirdPartyComponentsAsync(CancellationToken token)
    {
      var result = new ObservableCollection<ThirdPartyComponentData>();

      await Task.Run(() =>
      {
        var di = new DirectoryInfo(CoreEnvironment.ApplicationPath);

        if ( !di.Exists )
          return;

        var dlls = di.GetFiles("*.dll");

        if ( dlls.Length == 0 )
          return;

        dlls = dlls.Where(p => !p.Name.StartsWith("T4W") && !p.Name.StartsWith("Microsoft")).OrderBy(p => p.Name).ToArray();

        foreach ( FileInfo file in dlls )
        {
          try
          {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(file.Name);

            result.Add(new ThirdPartyComponentData
            {
              ComponentName = file.Name,
              ComponentVersion = fileVersionInfo.FileVersion,
              Description = fileVersionInfo.FileDescription
            });
          }
          catch
          {
            // Nothing
          }
        }
      }, token);

      return result;
    }
  }
}
