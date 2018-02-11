using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption;
using Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// Options view model
  /// </summary>
  public class OptionsViewModel : NotifyMaster
  {
    private readonly NotifyTaskCompletion _notifyTaskCompletion;
    private EnvironmentSettings.MementoEnvironmentSettings _mementoSettings;
    private ObservableCollection<IOptionPage> _options;

    #region Properties

    private string _title;

    /// <summary>
    /// Title
    /// </summary>
    public string Title
    {
      get => _title;
      set
      {
        _title = value;
        OnPropertyChanged(nameof(Title));
      }
    }

    private IOptionPage _currentViewModel;

    /// <summary>
    /// Current option view model
    /// </summary>
    public IOptionPage CurrentViewModel
    {
      get => _currentViewModel;
      set
      {
        _currentViewModel = value;
        OnPropertyChanged(nameof(CurrentViewModel));
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public OptionsViewModel()
    {
      _mementoSettings = SettingsHelperController.CurrentSettings.SaveToMemento();
      _notifyTaskCompletion = NotifyTaskCompletion.Create(OptionsViewModelAsync);
      _notifyTaskCompletion.PropertyChanged += TaskCompletionPropertyChanged;
    }

    private void TaskCompletionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if ( !(sender is NotifyTaskCompletion) || !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      CurrentViewModel = _options.First();
      Title = string.Format(Application.Current.TryFindResource("OptionsPageTitle").ToString(), CurrentViewModel.PageTitle);

      _notifyTaskCompletion.PropertyChanged -= TaskCompletionPropertyChanged;
    }

    private async Task OptionsViewModelAsync()
    {
      await Task.Run(() =>
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          _options = new ObservableCollection<IOptionPage>
          {
            new AboutOptionPage(),
            new EnvironmentOptionPage(),
            new AlertOptionPage()

          };
        });
      }).ConfigureAwait(false);
    }

    #region Commands

    private IAsyncCommand _saveOptionsCommand;

    /// <summary>
    /// Save options command
    /// </summary>
    public IAsyncCommand SaveOptionsCommand => _saveOptionsCommand ?? (_saveOptionsCommand = AsyncCommand.Create((param, token) => ExecuteSaveOptionsCommandAsync(param as Window)));

    private ICommand _closeOptionsCommand;

    /// <summary>
    /// Close options dialog command
    /// </summary>
    public ICommand CloseOptionsCommand => _closeOptionsCommand ?? (_closeOptionsCommand = new RelayCommand(p => ExecuteCloseOptionsCommand((Window) p)));

    #endregion

    #region Command functions

    private void ExecuteCloseOptionsCommand(Window window)
    {
      if ( _mementoSettings != null )
        SettingsHelperController.CurrentSettings.RestoreFromMemento(_mementoSettings);

      window?.Close();
    }

    private async Task ExecuteSaveOptionsCommandAsync(Window window)
    {
      await EnvironmentContainer.Instance.SaveSettingsAsync().ConfigureAwait(false);

      _mementoSettings = null;
      window?.Dispatcher.Invoke(window.Close);
    }

    #endregion
  }
}
