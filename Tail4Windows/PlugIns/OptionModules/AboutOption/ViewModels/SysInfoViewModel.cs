using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Controllers;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.ViewModels
{
  /// <summary>
  /// System information view model
  /// </summary>
  public class SysInfoViewModel : NotifyMaster, ISysInfoViewModel
  {
    private CancellationTokenSource _cts;

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// SystemInformation loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(GetSystemInformationsAsync));

    private ICommand _unloadedCommand;

    /// <summary>
    /// SystemInformation unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteSystemInfoUnloaded()));

    #endregion

    #region Command functions

    private void ExecuteSystemInfoUnloaded() => _cts?.Cancel();

    private async Task<ObservableCollection<KeyValuePair<string, string>>> GetSystemInformationsAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

      MouseService.SetBusyState();

      var systemInfoController = new SystemInformationController();
      var result = await systemInfoController.GetAllSystemInformationsAsync(_cts.Token).ConfigureAwait(false);
      var systemInformations = new ObservableCollection<KeyValuePair<string, string>>();

      await Task.Run(() =>
      {
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoApplicationName").ToString(), result.ApplicationName));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoApplicationVersion").ToString(), result.ApplicationVersion));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoApplicationBuildDate").ToString(), result.BuildDateTime));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoMachineName").ToString(), result.MachineName));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("Os").ToString(), $"{result.OsName} ({result.OsReleaseId}) / {result.OsType}"));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoVersion").ToString(), result.OsVersion));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoCpuName").ToString(), result.CpuInfo.Name));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoCpuManufacturer").ToString(), result.CpuInfo.Manufacturer));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoCpuSpeed").ToString(), $"{result.CpuInfo.ClockSpeed} MHz"));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoCpuPhysCores").ToString(), result.CpuInfo.NumberOfProcessors));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoCpuLogicCores").ToString(), result.CpuInfo.LogicalNumberOfProcessors));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoIpAddress").ToString(),
          $"{result.HostIpAddress.Ipv4} / {result.HostIpAddress.Ipv6}"));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoPhysMem").ToString(), result.GuiTotalPhys));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoPhysAvailable").ToString(), result.GuiAvailPhys));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoVirtMemTotal").ToString(), result.GuiTotalVirtual));
        systemInformations.Add(new KeyValuePair<string, string>(Application.Current.TryFindResource("SystemInfoVirtMemAvailable").ToString(), result.GuiAvailVirtual));
      }, _cts.Token).ConfigureAwait(false);

      return systemInformations;
    }

    #endregion
  }
}
