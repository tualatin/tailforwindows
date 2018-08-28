using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.ViewModels
{
  /// <summary>
  /// About option view model
  /// </summary>
  public class AboutOptionViewModel : NotifyMaster, IAboutOptionViewModel
  {
    private CancellationTokenSource _cts;
    private CancellationTokenSource _thirdPartyCts;
    private readonly IThirdPartyController _thirdPartyController;
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

    /// <summary>
    /// ThirdPartyComponents view
    /// </summary>
    public ListCollectionView ThirdPartyComponentsView
    {
      get;
      set;
    }

    private ObservableCollection<ThirdPartyComponentData> _thirdPartyComponents;

    /// <summary>
    /// Third party components
    /// </summary>
    public ObservableCollection<ThirdPartyComponentData> ThirdPartyComponents
    {
      get => _thirdPartyComponents;
      private set
      {
        _thirdPartyComponents = value;
        OnPropertyChanged();
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
      _thirdPartyController = new ThirdPartyController();
      NotifyTaskCompletion notifyTaskCompletion = NotifyTaskCompletion.Create(GetThirdPartyComponentsAsync);
      notifyTaskCompletion.PropertyChanged += OnGetThirdPartyComponentsPropertyChanged;

      Assembly assembly = Assembly.GetExecutingAssembly();
      Author = $"M. Zoennchen, Copyright 2013 - {DateTime.Now.Year}";
      BuildDate = Core.Utils.BuildDate.GetBuildDateTime(assembly).ToString(SettingsHelperController.CurrentSettings.CurrentCultureInfo);
      Version = $"{assembly.GetName().Version} - {build} ({channel})";
    }

    #region Commands

    private ICommand _requestNavigateCommand;

    /// <summary>
    /// Request navigate command
    /// </summary>
    public ICommand RequestNavigateCommand => _requestNavigateCommand ?? (_requestNavigateCommand = new RelayCommand(EnvironmentContainer.Instance.ExecuteRequestNavigateCommand));

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
      var url = new Uri(CoreEnvironment.ApplicationDonateWebUrl);
      EnvironmentContainer.Instance.ExecuteRequestNavigateCommand(url);
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource();

      while ( !_cts.IsCancellationRequested )
      {
        TimeSpan uptime = DateTime.Now.Subtract(EnvironmentContainer.Instance.UpTime);
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

    private void ExecuteUnloadedCommand()
    {
      _cts?.Cancel();
      _thirdPartyCts?.Cancel();
    }

    #endregion

    private async Task GetThirdPartyComponentsAsync()
    {
      MouseService.SetBusyState();

      _thirdPartyCts?.Dispose();
      _thirdPartyCts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

      _thirdPartyComponents = await _thirdPartyController.GetThirdPartyComponentsAsync(_thirdPartyCts.Token).ConfigureAwait(false);
    }

    private void OnGetThirdPartyComponentsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      ThirdPartyComponentsView = (ListCollectionView) new CollectionViewSource { Source = ThirdPartyComponents }.View;
      ThirdPartyCollectionViewHolder.Cv = ThirdPartyComponentsView;

      OnPropertyChanged(nameof(ThirdPartyComponents));
      OnPropertyChanged(nameof(ThirdPartyComponentsView));
    }
  }
}
