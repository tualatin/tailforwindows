﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.ViewModels
{
  /// <summary>
  /// Update option view model
  /// </summary>
  public class UpdateOptionViewModel : NotifyMaster, IUpdateOptionViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(UpdateOptionViewModel));

    private readonly IUpdater _updateController;
    private CancellationTokenSource _cts;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public UpdateOptionViewModel() => _updateController = new UpdateController(new WebDataController());

    #region Commands

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new NotImplementedException();

    private IAsyncCommand _checkUpdateCommand;

    /// <summary>
    /// Check update command
    /// </summary>
    public IAsyncCommand CheckUpdateCommand => _checkUpdateCommand ?? (_checkUpdateCommand = AsyncCommand.Create(ExecuteCheckUpdateCommandAsync));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteUnloadedCommand()));

    private ICommand _visitWebsiteCommand;

    /// <summary>
    /// Visivit website command
    /// </summary>
    public ICommand VisitWebsiteCommand => _visitWebsiteCommand ?? (_visitWebsiteCommand = new RelayCommand(p => ExecuteVisitWebsiteCommand()));

    #endregion

    #region Command functions

    private async Task<UpdateData> ExecuteCheckUpdateCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

      MouseService.SetBusyState();

      var result = new UpdateData();

      try
      {
        result = await _updateController.UpdateNecessaryAsync(_cts.Token, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      return result;
    }

    private void ExecuteUnloadedCommand() => _cts?.Cancel();

    private void ExecuteVisitWebsiteCommand()
    {
      try
      {
        var url = new Uri(EnvironmentContainer.ApplicationReleaseWebUrl);
        Process.Start(new ProcessStartInfo(url.AbsoluteUri));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }

    #endregion
  }
}
