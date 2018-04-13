using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.LogWindowModule;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.QuickAddModule.ViewModels
{
  /// <summary>
  /// QuickAdd view model
  /// </summary>
  public class QuickAddViewModel : NotifyMaster
  {
    private readonly CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;

    private ObservableCollection<TailData> _fileManagerCollection;
    private Window _window;

    #region Properties

    private TailData _currenTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _currenTailData;
      set
      {
        if ( value == _currenTailData )
          return;

        _currenTailData = value;
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
      _xmlFileManagerController = new XmlFileManagerController();
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

      await _xmlFileManagerController.AddTailDataToXmlFileAsync(_cts.Token, CurrentTailData).ConfigureAwait(false);
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
        _fileManagerCollection = await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
        _categories = await _xmlFileManagerController.GetCategoriesFromXmlFileAsync(_fileManagerCollection).ConfigureAwait(false);
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
