﻿using System;
using System.Collections.ObjectModel;
using System.Threading;
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
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// Options view model
  /// </summary>
  public class OptionsViewModel : NotifyMaster
  {
    private EnvironmentSettings.MementoEnvironmentSettings _mementoSettings;
    private readonly CancellationTokenSource _cts;

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

        if ( _currentViewModel != null )
          Title = string.Format(Application.Current.TryFindResource("OptionsPageTitle").ToString(), _currentViewModel.PageTitle);

        OnPropertyChanged(nameof(CurrentViewModel));
      }
    }

    /// <summary>
    /// TreeView items
    /// </summary>
    public ObservableCollection<TreeNodeOptionViewModel> Root
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public OptionsViewModel()
    {
      _mementoSettings = SettingsHelperController.CurrentSettings.SaveToMemento();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

      var environment = new EnvironmentOptionPage();
      var optionPage1 = new TreeNodeOptionViewModel(environment, new[]
      {
        new TreeNodeOptionViewModel(environment)
      });

      var alert = new AlertOptionPage();
      var optionPage2 = new TreeNodeOptionViewModel(alert, new[]
      {
        new TreeNodeOptionViewModel(alert)
      });

      var about = new AboutOptionPage();
      var optionPage3 = new TreeNodeOptionViewModel(about, new[]
      {
        new TreeNodeOptionViewModel(about)
      });

      Root = new ObservableCollection<TreeNodeOptionViewModel>
      {
        optionPage1,
        optionPage2,
        optionPage3
      };
    }

    #region Commands

    private IAsyncCommand _saveOptionsCommand;

    /// <summary>
    /// Save options command
    /// </summary>
    public IAsyncCommand SaveOptionsCommand => _saveOptionsCommand ?? (_saveOptionsCommand = AsyncCommand.Create((p, t) => ExecuteSaveOptionsCommandAsync()));

    private ICommand _closeOptionsCommand;

    /// <summary>
    /// Close options dialog command
    /// </summary>
    public ICommand CloseOptionsCommand => _closeOptionsCommand ?? (_closeOptionsCommand = new RelayCommand(p => ExecuteCloseOptionsCommand((Window) p)));

    private ICommand _setSelectedItemCommand;

    /// <summary>
    /// Set selected item command
    /// </summary>
    public ICommand SetSelectedItemCommand => _setSelectedItemCommand ?? (_setSelectedItemCommand = new RelayCommand(ExecuteSelectedItemCommand));

    #endregion

    #region Command functions

    private void ExecuteSelectedItemCommand(object parameter)
    {
      if ( !(parameter is TreeNodeOptionViewModel node) )
        return;

      CurrentViewModel = node.OptionPage;
    }

    private void ExecuteCloseOptionsCommand(Window window)
    {
      MouseService.SetBusyState();

      if ( _mementoSettings != null )
        SettingsHelperController.CurrentSettings.RestoreFromMemento(_mementoSettings);

      _mementoSettings = null;

      _cts.Cancel();
      window?.Close();
    }

    private async Task ExecuteSaveOptionsCommandAsync()
    {
      MouseService.SetBusyState();

      _mementoSettings = null;
      await EnvironmentContainer.Instance.SaveSettingsAsync(_cts).ConfigureAwait(false);
    }

    #endregion
  }
}
