using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;


namespace Org.Vs.TailForWin.PlugIns.QuickAddModule.ViewModels
{
  /// <summary>
  /// QuickAdd view model
  /// </summary>
  public class QuickAddViewModel : NotifyMaster, IViewModelBase
  {
    private readonly CancellationTokenSource _cts;
    private readonly IFileManagerController _fileManagerController;

    private ObservableCollection<TailData> _fileManagerCollection;
    private Window _window;

    #region Properties

    private TailData _currentTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _currentTailData;
      set
      {
        if ( value == _currentTailData )
          return;

        _currentTailData = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<string> _categories;

    /// <summary>
    /// Categories
    /// </summary>
    public ObservableCollection<string> Categories
    {
      get => _categories;
      set
      {
        if ( value == _categories )
          return;

        _categories = value;
        OnPropertyChanged();
      }
    }

    private bool _hasFocus;

    /// <summary>
    /// Has focus
    /// </summary>
    public bool HasFocus
    {
      get => _hasFocus;
      set
      {
        if ( value == _hasFocus )
          return;

        _hasFocus = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public QuickAddViewModel()
    {
      _fileManagerController = new FileManagerController();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<AddTailDataToQuickAddMessage>(OnGetTailData);
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnSaveTailDataPropertyChanged;
      ((AsyncCommand<object>) SaveCommand).PropertyChanged += OnSaveAndClosePropertyChanged;
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

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((Window) p)));

    private IAsyncCommand _saveCommand;

    /// <summary>
    /// Save command
    /// </summary>
    public IAsyncCommand SaveCommand => _saveCommand ?? (_saveCommand = AsyncCommand.Create(p => CanExecuteSaveCommand(), (p, t) => ExecuteSaveCommandAsync((Window) p)));

    #endregion

    #region Command functions

    private bool CanExecuteSaveCommand() => CurrentTailData != null && string.IsNullOrWhiteSpace(CurrentTailData["Description"]);

    private async Task ExecuteSaveCommandAsync(Window window)
    {
      if ( CurrentTailData == null )
        return;

      MouseService.SetBusyState();

      _window = window;
      CurrentTailData.OpenFromFileManager = true;
      CurrentTailData.IsLoadedByXml = true;
      CurrentTailData.CommitChanges();

      var success = await _fileManagerController.AddTailDataAsync(CurrentTailData, _cts.Token, _fileManagerCollection).ConfigureAwait(false);

      if ( !success )
      {
        InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("FileManagerSaveItemsError").ToString());
      }
    }

    private void ExecuteCloseCommand(Window window)
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<AddTailDataToQuickAddMessage>(OnGetTailData);

      _cts.Cancel();
      window?.Close();
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      try
      {
        await _fileManagerController.ConvertXmlToJsonConfigAsync(_cts.Token).ConfigureAwait(false);
        await _fileManagerController.ReadJsonFileAsync(_cts.Token).ContinueWith(p =>
        {
          _fileManagerCollection = p.Result;
          _categories = _fileManagerController.GetCategoriesAsync(_cts.Token, _fileManagerCollection).Result;
        }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(false);
      }
      catch
      {
        // Nothing
      }
    }

    #endregion

    private void OnGetTailData(AddTailDataToQuickAddMessage args)
    {
      if ( !(args.Sender is LogWindowControl) )
        return;

      CurrentTailData = args.TailData;
    }

    private void OnSaveAndClosePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new DisableQuickAddInTailDataMessage(this, CurrentTailData.OpenFromFileManager));
      ExecuteCloseCommand(_window);
    }

    private void OnSaveTailDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      HasFocus = true;
      OnPropertyChanged(nameof(Categories));
    }
  }
}
