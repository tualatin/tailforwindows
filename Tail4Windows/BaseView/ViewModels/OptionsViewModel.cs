using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.BaseView.UserControls.Interfaces;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption;
using Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption;
using Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption.ViewModels;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;
using Org.Vs.TailForWin.PlugIns.OptionModules.SmartWatchOption;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// Options view model
  /// </summary>
  public class OptionsViewModel : NotifyMaster, IOptionsViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(OptionsViewModel));

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
        if ( Equals(_title, value) )
          return;

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
        if ( value == _currentViewModel )
          return;

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

      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<OpenSmtpSettingMessage>(OnOpenSmtpSettings);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenSmtpSettingMessage>(OnOpenSmtpSettings);

      InitializeOptionPages();
      NotifyTaskCompletion.Create(InitializeOptionViewAsync);
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

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new NotImplementedException();

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

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

      SettingsHelperController.CurrentSettings.LastViewedOptionPage = CurrentViewModel.PageId;
      _mementoSettings = null;

      _cts.Cancel();
      window?.Close();
    }

    private async Task ExecuteSaveOptionsCommandAsync()
    {
      MouseService.SetBusyState();

      SettingsHelperController.CurrentSettings.LastViewedOptionPage = CurrentViewModel.PageId;
      _mementoSettings = null;

      await EnvironmentContainer.Instance.SaveSettingsAsync(_cts).ConfigureAwait(false);
    }

    #endregion

    #region HelperFunctions

    private void InitializeOptionPages()
    {
      var environment = new EnvironmentOptionPage();
      var optionPage1 = new TreeNodeOptionViewModel(environment, new[]
      {
        new TreeNodeOptionViewModel(environment, null),
        new TreeNodeOptionViewModel(new ExtraOptionPage(), null),
        new TreeNodeOptionViewModel(new ColorOptionPage(), null),
        new TreeNodeOptionViewModel(new ProxyOptionPage(), null),
        new TreeNodeOptionViewModel(new SmtpOptionPage(), null),
        new TreeNodeOptionViewModel(new ImportExportOptionPage(), null)
      }, "system.ico");

      var smartWatch = new SmartWatchOptionPage();
      var optionPage2 = new TreeNodeOptionViewModel(smartWatch, new[]
      {
        new TreeNodeOptionViewModel(smartWatch, null)
      }, "main.ico");

      var alert = new AlertOptionPage();
      var optionPage3 = new TreeNodeOptionViewModel(alert, new[]
      {
        new TreeNodeOptionViewModel(alert, null),
      }, "alert.ico");

      var about = new AboutOptionPage();
      var optionPage4 = new TreeNodeOptionViewModel(about, new[]
      {
        new TreeNodeOptionViewModel(about, null),
        new TreeNodeOptionViewModel(new UpdateOptionPage(), null),
        new TreeNodeOptionViewModel(new SysInfoOptionPage(), null)
      }, "about.png");

      Root = new ObservableCollection<TreeNodeOptionViewModel>
      {
        optionPage1,
        optionPage2,
        optionPage3,
        optionPage4
      };
    }

    private async Task InitializeOptionViewAsync()
    {
      if ( Root == null || Root.Count == 0 )
        return;

      MouseService.SetBusyState();

      await Task.Run(
        () =>
        {
          try
          {
            foreach ( var node in Root )
            {
              node.ApplyCriteria(string.Empty, new Stack<ITreeNodeViewModel>());
            }

            if ( SettingsHelperController.CurrentSettings.LastViewedOptionPage == Guid.Empty )
            {
              OpenRootNode();
              return;
            }

            GetCertainSettingsPage(SettingsHelperController.CurrentSettings.LastViewedOptionPage);
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
          }
        }, _cts.Token).ConfigureAwait(false);
    }

    private void OpenRootNode()
    {
      // Expand and select the first node
      Root.First().IsExpanded = true;
      Root.First().IsSelected = true;
      CurrentViewModel = Root.First().OptionPage;
    }

    private void GetCertainSettingsPage(Guid idToOpen)
    {
      TreeNodeOptionViewModel isSelected = null;
      TreeNodeOptionViewModel parent = null;

      foreach ( var treeNodeOptionViewModel in Root )
      {
        if ( treeNodeOptionViewModel.OptionPage.PageId == idToOpen )
        {
          isSelected = treeNodeOptionViewModel;
          parent = treeNodeOptionViewModel;
          break;
        }

        if ( !treeNodeOptionViewModel.Children.Any() )
          continue;

        isSelected = SelectLastOpenOption((IEnumerable<TreeNodeOptionViewModel>) treeNodeOptionViewModel.Children, idToOpen);

        if ( isSelected == null )
          continue;

        parent = treeNodeOptionViewModel;
        break;
      }

      if ( isSelected == null )
      {
        OpenRootNode();
        return;
      }

      parent.IsExpanded = true;
      isSelected.IsSelected = true;
      CurrentViewModel = isSelected.OptionPage;
    }

    private TreeNodeOptionViewModel SelectLastOpenOption(IEnumerable<TreeNodeOptionViewModel> node, Guid idToOpen)
    {
      foreach ( var treeNodeOptionViewModel in node )
      {
        if ( treeNodeOptionViewModel.OptionPage.PageId == idToOpen )
          return treeNodeOptionViewModel;

        if ( treeNodeOptionViewModel.Children.Count() != 0 )
          return SelectLastOpenOption((IEnumerable<TreeNodeOptionViewModel>) treeNodeOptionViewModel.Children, idToOpen);
      }
      return null;
    }

    private void OnOpenSmtpSettings(OpenSmtpSettingMessage args)
    {
      if ( !(args.Sender is AlertOptionViewModel) )
        return;

      GetCertainSettingsPage(Guid.Parse("cfc162ef-5755-4958-a559-ab893ca8e1ed"));
    }

    #endregion
  }
}
