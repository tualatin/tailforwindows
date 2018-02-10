using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.ViewModels;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// Options view model
  /// </summary>
  public class OptionsViewModel : NotifyMaster
  {
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
      CurrentViewModel = new AboutOptionViewModel();
      Title = string.Format(Application.Current.TryFindResource("OptionsPageTitle").ToString(), CurrentViewModel.PageTitle);
    }

    #region Commands

    private IAsyncCommand _saveOptionsCommand;

    /// <summary>
    /// Save options command
    /// </summary>
    public IAsyncCommand SaveOptionsCommand => _saveOptionsCommand ?? (_saveOptionsCommand = AsyncCommand.Create((param, token) => ExecuteSaveOptionsCommandAsync(param as Window, token)));

    private ICommand _closeOptionsCommand;

    /// <summary>
    /// Close options dialog command
    /// </summary>
    public ICommand CloseOptionsCommand => _closeOptionsCommand ?? (_closeOptionsCommand = new RelayCommand(p => ExecuteCloseOptionsCommand((Window) p)));

    #endregion

    #region Command functions

    private void ExecuteCloseOptionsCommand(Window window) => window?.Close();

    private async Task ExecuteSaveOptionsCommandAsync(Window window, object _)
    {
      await EnvironmentContainer.Instance.SaveSettingsAsync().ConfigureAwait(false);
      window?.Dispatcher.Invoke(window.Close);
    }

    #endregion
  }
}
