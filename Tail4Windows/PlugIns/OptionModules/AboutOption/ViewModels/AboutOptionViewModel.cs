using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.ViewModels
{
  /// <summary>
  /// About option view model
  /// </summary>
  public class AboutOptionViewModel : NotifyMaster, IAboutOptionViewModel
  {
    private CancellationTokenSource _cts;
    private readonly string _hours = Application.Current.TryFindResource("AboutUptimeHours").ToString();
    private readonly string _days = Application.Current.TryFindResource("AboutUptimeDays").ToString();

    #region Properties

    private string _version;

    /// <summary>
    /// Current version
    /// </summary>
    public string Version
    {
      get => _version;
      set
      {
        _version = value;
        OnPropertyChanged(nameof(_version));
      }
    }

    private string _buildDate;

    /// <summary>
    /// Build date
    /// </summary>
    public string BuildDate
    {
      get => _buildDate;
      set
      {
        _buildDate = value;
        OnPropertyChanged(nameof(BuildDate));
      }
    }

    private string _author;

    /// <summary>
    /// Author
    /// </summary>
    public string Author
    {
      get => _author;
      set
      {
        _author = value;
        OnPropertyChanged(nameof(Author));
      }
    }

    private string _upTime;

    /// <summary>
    /// Uptime
    /// </summary>
    public string UpTime
    {
      get => _upTime;
      set
      {
        _upTime = value;
        OnPropertyChanged(nameof(UpTime));
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public AboutOptionViewModel()
    {
#if BUILD64
      string build = "64-Bit";
#elif BUILD32
      string build = "32-Bit";
#endif

#if DEBUG
      string channel = "debug";
#elif RELEASE
      string channel = "release";
#endif

      var assembly = Assembly.GetExecutingAssembly();

      Author = $"M. Zoennchen, Copyright 2013 - {DateTime.Now.Year}";
      BuildDate = Core.Utils.BuildDate.GetBuildDateTime(assembly).ToString(SettingsHelperController.CurrentSettings.CurrentCultureInfo);
      Version = $"{assembly.GetName().Version} - {build} ({channel})";
    }

    #region Commands

    private ICommand _requestNavigateCommand;

    /// <summary>
    /// Request navigate command
    /// </summary>
    public ICommand RequestNavigateCommand => _requestNavigateCommand ?? (_requestNavigateCommand = new RelayCommand(ExecuteRequestNavigateCommand));

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create((p, t) => ExecuteLoadedCommandAsync()));

    private ICommand _unloadedCommand;

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(p => ExecuteUnloadedCommand()));

    private ICommand _donateCommand;

    /// <summary>
    /// Donate command
    /// </summary>
    public ICommand DonateCommand => _donateCommand ?? (_donateCommand = new RelayCommand(p => ExecuteDonateCommand()));

    #endregion

    #region Command functions

    private void ExecuteDonateCommand()
    {
      var url = new Uri(EnvironmentContainer.ApplicationDonateWebUrl);
      ExecuteRequestNavigateCommand(url);
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      while ( !_cts.IsCancellationRequested )
      {
        var uptime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);
        UpTime = $"{uptime.Days} {_days}, {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00} {_hours}";

        try
        {
          await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token).ConfigureAwait(false);
        }
        catch
        {
          // Nothing
        }
      }
    }
    private void ExecuteUnloadedCommand() => _cts?.Cancel();

    private void ExecuteRequestNavigateCommand(object parameter)
    {
      if ( !(parameter is Uri url) )
        return;

      Process.Start(new ProcessStartInfo(url.AbsoluteUri));
    }

    #endregion
  }
}
