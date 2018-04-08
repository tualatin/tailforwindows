using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels
{
  /// <summary>
  /// FileManager view model
  /// </summary>
  public class FileManagerViewModel : NotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileManagerViewModel));

    private TailData.MementoTailData _mementoTailData;
    private readonly CancellationTokenSource _cts;

    #region Properties

    private TailData _selectedItem;

    /// <summary>
    /// Current selected item
    /// </summary>
    public TailData SelectedItem
    {
      get => _selectedItem;
      set
      {
        _selectedItem = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FileManagerViewModel()
    {
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    private ICommand _openCommand;

    /// <summary>
    /// Open command
    /// </summary>
    public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(p => ExecuteOpenCommand()));

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((Window) p)));

    #endregion

    #region Command functions

    private async Task ExecuteLoadedCommandAsync()
    {

    }

    private void ExecuteOpenCommand()
    {

    }

    private void ExecuteCloseCommand(Window window)
    {
      MouseService.SetBusyState();

      if ( _mementoTailData != null )
        SelectedItem.RestoreFromMemento(_mementoTailData);

      _mementoTailData = null;
      _cts.Cancel();
      window?.Close();
    }

    #endregion
  }
}
