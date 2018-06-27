using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.Services;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.Business.Services.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.WindowEventReadModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.WindowEventReadModule.ViewModels
{
  /// <summary>
  /// Windows event categories view model
  /// </summary>
  public class WindowsEventCategoriesViewModel : NotifyMaster, IWindowsEventCategoriesViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(WindowsEventCategoriesViewModel));

    private readonly ILogReadService _windowLogReadService;
    private ObservableCollection<WindowsEventCategory> _categories;
    private CancellationTokenSource _cts;

    #region Properties

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get;
      private set;
    }

    /// <summary>
    /// TreeView items
    /// </summary>
    public ObservableCollection<TreeNodeWindowsEventsViewModel> Root
    {
      get;
      set;
    }

    private string _computerName;

    /// <summary>
    /// Name of connected computer
    /// </summary>
    public string ComputerName
    {
      get => _computerName;
      set
      {
        if ( Equals(value, _computerName) )
          return;

        _computerName = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public WindowsEventCategoriesViewModel()
    {
      CurrentTailData = new TailData { IsWindowsEvent = true };
      _windowLogReadService = new WindowsEventReadService { TailData = CurrentTailData };

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedCompleted;
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

    private ICommand _setSelectedItemCommand;

    /// <summary>
    /// Set selected item command
    /// </summary>
    public ICommand SetSelectedItemCommand => _setSelectedItemCommand ?? (_setSelectedItemCommand = new RelayCommand(ExecuteSelectedItemCommand));

    private ICommand _openCommand;

    /// <summary>
    /// Open command
    /// </summary>
    public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(p => CanExecuteOpenCommand(), p => ExecuteOpenCommand((Window) p)));

    private ICommand _mouseDoubleClickCommand;

    /// <summary>
    /// MouseDouble click command
    /// </summary>
    public ICommand MouseDoubleClickCommand => _mouseDoubleClickCommand ?? (_mouseDoubleClickCommand = new RelayCommand(p => CanExecuteOpenCommand(), ExecuteMouseDoubleClickCommmand));

    #endregion

    #region Command functions

    private void ExecuteMouseDoubleClickCommmand(object param)
    {
      if ( !(param is object[] o) )
        return;

      if ( !(o.First() is MouseButtonEventArgs) || !(o.Last() is Window wnd) )
        return;

      ExecuteOpenCommand(wnd);
    }

    private bool CanExecuteOpenCommand() => CurrentTailData != null && !string.IsNullOrWhiteSpace(CurrentTailData.WindowsEvent.Category);

    private void ExecuteOpenCommand(Window window)
    {
      window.Close();
    }

    private void ExecuteSelectedItemCommand(object parameter)
    {
      if ( !(parameter is TreeNodeWindowsEventsViewModel treeNode) )
        return;

      if ( string.IsNullOrWhiteSpace(treeNode.LogDisplayName) )
      {
        CurrentTailData.WindowsEvent.Category = string.Empty;
        return;
      }

      CurrentTailData.WindowsEvent.Category = treeNode.LogDisplayName;
      CurrentTailData.WindowsEvent.Machine = ComputerName == Environment.MachineName ? "." : ComputerName;

      LOG.Trace($"Current category: {CurrentTailData.WindowsEvent.Category}");
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

      _categories = await _windowLogReadService.GetCategoriesAsync(_cts.Token).ConfigureAwait(false);
    }

    #endregion

    private void LoadedCompleted(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( _categories == null || _categories.Count == 0 )
        return;

      ComputerName = Environment.MachineName;

      InitializeTreeView();
      NotifyTaskCompletion.Create(InitializeOptionViewAsync);
    }

    private void InitializeTreeView()
    {
      string windowsLabel = Application.Current.TryFindResource("WindowsEventWindowsLogs").ToString();
      string applicationLabel = Application.Current.TryFindResource("WindowsEventApplicationLogs").ToString();
      var windowsLogs = _categories.Where(p => string.Compare(p.Category, windowsLabel, StringComparison.OrdinalIgnoreCase) == 0).
        Select(p => new TreeNodeWindowsEventsViewModel(p.LogDisplayName, p.Log, "event.png")).ToList();
      var applicationLogs = _categories.Where(p => string.Compare(p.Category, applicationLabel, StringComparison.OrdinalIgnoreCase) == 0).
        Select(p => new TreeNodeWindowsEventsViewModel(p.LogDisplayName, p.Log, "event.png")).ToList();

      var windowsTreeView = new TreeNodeWindowsEventsViewModel(windowsLabel, string.Empty, windowsLogs, "openfolder.ico");
      var applicationTreeView = new TreeNodeWindowsEventsViewModel(applicationLabel, string.Empty, applicationLogs, "openfolder.ico");

      Root = new ObservableCollection<TreeNodeWindowsEventsViewModel>
      {
        windowsTreeView,
        applicationTreeView
      };

      OnPropertyChanged(nameof(Root));
    }

    private async Task InitializeOptionViewAsync()
    {
      if ( Root == null || Root.Count == 0 )
        return;

      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await Task.Run(
        () =>
        {
          try
          {
            foreach ( TreeNodeWindowsEventsViewModel node in Root )
            {
              node.ApplyCriteria(string.Empty, new Stack<ITreeNodeViewModel>());
            }

            OpenRootNode();
          }
          catch ( Exception ex )
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
        }, _cts.Token).ConfigureAwait(false);
    }

    private void OpenRootNode()
    {
      // Expand and select the first node
      Root.First().IsExpanded = true;
      Root.First().IsSelected = true;
    }

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
