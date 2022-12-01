using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.PlugIns.FindModule.ViewModels
{
  /// <summary>
  /// FindDialog view model
  /// </summary>
  public class FindWhatViewModel : NotifyMaster, IFindWhatViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FindWhatViewModel));

    private CancellationTokenSource _cts;
    private readonly ISettingsDbController _dbController;
    private readonly IHistory<HistoryData> _searchHistoryController;

    #region Properties

    private double _topPosition;

    /// <summary>
    /// Top position
    /// </summary>
    public double TopPosition
    {
      get => _topPosition;
      set
      {
        _topPosition = value;
        OnPropertyChanged();
      }
    }

    private double _leftPosition;

    /// <summary>
    /// Left position
    /// </summary>
    public double LeftPosition
    {
      get => _leftPosition;
      set
      {
        _leftPosition = value;
        OnPropertyChanged();
      }
    }

    private bool _searchFieldHasFocus;

    /// <summary>
    /// SearchField has focus
    /// </summary>
    public bool SearchFieldHasFocus
    {
      get => _searchFieldHasFocus;
      set
      {
        _searchFieldHasFocus = value;
        OnPropertyChanged();
      }
    }

    private HistoryData _searchHistory;

    /// <summary>
    /// Search history
    /// </summary>
    public HistoryData SearchHistory
    {
      get => _searchHistory;
      set
      {
        if ( value == _searchHistory )
          return;

        _searchHistory = value;
        OnPropertyChanged();
      }
    }

    private FindData _findSettings;

    /// <summary>
    /// Find settings
    /// </summary>
    public FindData FindSettings
    {
      get => _findSettings;
      set
      {
        _findSettings = value;
        OnPropertyChanged();
      }
    }

    private string _countMatches;

    /// <summary>
    /// Count current matches
    /// </summary>
    public string CountMatches
    {
      get => _countMatches;
      set
      {
        _countMatches = value;
        OnPropertyChanged();
      }
    }

    private string _searchText;

    /// <summary>
    /// Search text
    /// </summary>
    public string SearchText
    {
      get => _searchText;
      set
      {
        _searchText = value;
        CountMatches = string.Empty;

        EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new FindWhatChangedClosedMessage(WindowGuid));
        OnPropertyChanged();
      }
    }

    private string _selectedItem;

    /// <summary>
    /// Selected item
    /// </summary>
    public string SelectedItem
    {
      get => _selectedItem;
      set
      {
        _selectedItem = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// ParentWindow <see cref="Guid"/>
    /// </summary>
    public Guid WindowGuid
    {
      get;
      set;
    }

    private int _caretIndex;

    /// <summary>
    /// Caret index
    /// </summary>
    public int CaretIndex
    {
      get => _caretIndex;
      set
      {
        if ( value == _caretIndex )
          return;

        _caretIndex = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindWhatViewModel()
    {
      _dbController = SettingsDbController.Instance;
      _searchHistoryController = new HistoryController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedPropertyChanged;
      ((AsyncCommand<object>) FindNextCommand).PropertyChanged += FindNextCommandPropertyChanged;
      ((AsyncCommand<object>) CountCommand).PropertyChanged += FindNextCommandPropertyChanged;
      ((AsyncCommand<object>) FindAllCommand).PropertyChanged += FindNextCommandPropertyChanged;
      ((AsyncCommand<object>) DeleteHistoryCommand).PropertyChanged += DeleteHistoryPropertyChanged;
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

    private ICommand _closingCommand;

    /// <summary>
    /// Closing command
    /// </summary>
    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand(p => ExecuteClosingCommand()));

    private IAsyncCommand _findNextCommand;

    /// <summary>
    /// FindNext command
    /// </summary>
    public IAsyncCommand FindNextCommand => _findNextCommand ?? (_findNextCommand = AsyncCommand.Create(p => CanExecuteFindCommand(), ExecuteFindNextCommandAsync));

    private IAsyncCommand _findAllCommand;

    /// <summary>
    /// FindAll command
    /// </summary>
    public IAsyncCommand FindAllCommand => _findAllCommand ?? (_findAllCommand = AsyncCommand.Create(p => CanExecuteFindCommand(), ExecuteFindAllCommandAsync));

    private IAsyncCommand _countCommand;

    /// <summary>
    /// Count command
    /// </summary>
    public IAsyncCommand CountCommand => _countCommand ?? (_countCommand = AsyncCommand.Create(p => CanExecuteFindCommand(), ExecuteCountCommandAsync));

    private ICommand _previewKeyDownCommand;

    /// <summary>
    /// PreviewKeyDown command
    /// </summary>
    public ICommand PreviewKeyDownCommand => _previewKeyDownCommand ?? (_previewKeyDownCommand = new RelayCommand(ExecutePreviewKeyDownCommand));

    private IAsyncCommand _wrapAroundCommand;

    /// <summary>
    /// Wrap around command
    /// </summary>
    public IAsyncCommand WrapAroundCommand => _wrapAroundCommand ?? (_wrapAroundCommand = AsyncCommand.Create(ExecuteWrapAroundCommandAsync));

    private IAsyncCommand _keyDownCommand;

    /// <summary>
    /// KeyDown command
    /// </summary>
    public IAsyncCommand KeyDownCommand => _keyDownCommand ?? (_keyDownCommand = AsyncCommand.Create((p, t) => ExecuteKeyDownCommandAsync(p)));

    private IAsyncCommand _deleteHistoryCommand;

    /// <summary>
    /// Delete history command
    /// </summary>
    public IAsyncCommand DeleteHistoryCommand => _deleteHistoryCommand ?? (_deleteHistoryCommand = AsyncCommand.Create(p => CanDeleteHistory(), ExecuteDeleteHistoryCommandAsync));

    #endregion

    #region Command functions

    private bool CanDeleteHistory() => SearchHistory != null && SearchHistory.FindCollection.Count > 0;

    private async Task ExecuteDeleteHistoryCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await _searchHistoryController.DeleteHistoryAsync(_searchHistory, _cts.Token).ContinueWith(p =>
      {
        if ( !p.Result )
        {
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("HistoryDeleteError").ToString());
        }

        _searchHistory = _searchHistoryController.ReadHistoryAsync(_cts.Token).Result;
      }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(false);
    }

    private async Task ExecuteKeyDownCommandAsync(object param)
    {
      if ( !(param is KeyEventArgs e) )
        return;

      if ( string.IsNullOrWhiteSpace(SearchText) || e.Key != Key.Enter )
        return;

      await ExecuteFindAllCommandAsync();
    }

    private void ExecutePreviewKeyDownCommand(object param)
    {
      if ( !(param is object[] o) )
        return;

      if ( !(o.First() is KeyEventArgs e) || !(o.Last() is Window window) )
        return;

      if ( e.Key != Key.Escape )
        return;

      e.Handled = true;
      window.Close();
    }

    private async Task ExecuteLoadedCommandAsync()
    {
      try
      {
        SetCancellationTokenSource();

        _searchHistory = await _searchHistoryController.ReadHistoryAsync(_cts.Token).ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }

    private void ExecuteClosingCommand()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<FindWhatCountResponseMessage>(OnFindWhatCountResponse);
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new FindWhatChangedClosedMessage(WindowGuid));

      SettingsHelperController.CurrentSettings.FindDialogPositionX = LeftPosition;
      SettingsHelperController.CurrentSettings.FindDialogPositionY = TopPosition;

      _dbController.UpdateFindDialogDbSettingsAsync(new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token).SafeAwait();
    }

    /// <summary>
    /// Can execute find command
    /// </summary>
    /// <returns><c>True</c> if it can execute otherwise <c>False</c></returns>
    public bool CanExecuteFindCommand() => FindSettings != null && FindSettings.SearchBookmarks || !string.IsNullOrWhiteSpace(SearchText);

    private async Task ExecuteFindNextCommandAsync()
    {
      FindSettings.CountFind = false;
      SearchFieldHasFocus = false;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StartSearchFindNextMessage(WindowGuid, FindSettings, SearchText));

      if ( !FindSettings.SearchBookmarks )
        await HandleFindAsync();
    }

    private async Task ExecuteFindAllCommandAsync()
    {
      FindSettings.CountFind = false;
      SearchFieldHasFocus = false;
      CountMatches = string.Empty;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StartSearchAllMessage(WindowGuid, FindSettings, SearchText));

      if ( !FindSettings.SearchBookmarks )
        await HandleFindAsync();
    }

    private async Task ExecuteCountCommandAsync()
    {
      FindSettings.CountFind = true;
      SearchFieldHasFocus = false;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StartSearchCountMessage(WindowGuid, FindSettings, SearchText));

      if ( !FindSettings.SearchBookmarks )
        await HandleFindAsync();
    }

    private async Task ExecuteWrapAroundCommandAsync()
    {
      MouseService.SetBusyState();

      SearchHistory.Wrap = FindSettings.Wrap;

      SetCancellationTokenSource();

      if ( !await _searchHistoryController.UpdateHistoryAsync(_searchHistory, string.Empty, _cts.Token).ConfigureAwait(false) )
      {
        InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("HistoryUpdateError").ToString());
      }
    }

    #endregion

    #region HelperFunctions

    private async Task HandleFindAsync()
    {
      MouseService.SetBusyState();

      if ( string.IsNullOrWhiteSpace(SearchText) )
        return;

      SetCancellationTokenSource();

      await _searchHistoryController.UpdateHistoryAsync(_searchHistory, SearchText, _cts.Token).ContinueWith(p =>
      {
        if ( !p.Result )
        {
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("HistoryUpdateError").ToString());
        }

        _searchHistory = _searchHistoryController.ReadHistoryAsync(_cts.Token).Result;
      }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(false);
    }

    #endregion

    private void DeleteHistoryPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      OnPropertyChanged(nameof(SearchHistory));
    }

    private void FindNextCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      OnPropertyChanged(nameof(SearchHistory));
    }

    private void LoadedPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      MoveInfoView();

      TopPosition = SettingsHelperController.CurrentSettings.FindDialogPositionY;
      LeftPosition = SettingsHelperController.CurrentSettings.FindDialogPositionX;
      FindSettings = new FindData
      {
        Wrap = SearchHistory.Wrap
      };
      FindSettings.PropertyChanged += OnFindSettingsPropertyChanged;

      SearchFieldHasFocus = true;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<FindWhatCountResponseMessage>(OnFindWhatCountResponse);
      OnPropertyChanged(nameof(SearchHistory));
    }

    private static void MoveInfoView()
    {
      double posX = SettingsHelperController.CurrentSettings.FindDialogPositionX;
      double posY = SettingsHelperController.CurrentSettings.FindDialogPositionY;

      UiHelper.MoveIntoView(Application.Current.TryFindResource("FindDialogWindowTitle").ToString(), ref posX, ref posY, 315, 380);

      SettingsHelperController.CurrentSettings.FindDialogPositionX = posX;
      SettingsHelperController.CurrentSettings.FindDialogPositionY = posY;
    }

    private void OnFindSettingsPropertyChanged(object sender, PropertyChangedEventArgs e) =>
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new FindWhatChangedClosedMessage(WindowGuid));

    private void OnFindWhatCountResponse(FindWhatCountResponseMessage args)
    {
      if ( WindowGuid != args.WindowGuid )
        return;

      CountMatches = string.Format(Application.Current.TryFindResource("FindDialogSearchCount").ToString(), args.Count);
    }

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
