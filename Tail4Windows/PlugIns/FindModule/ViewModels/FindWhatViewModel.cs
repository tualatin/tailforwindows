using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.FindModule.ViewModels
{
  /// <summary>
  /// FindDialog view model
  /// </summary>
  public class FindWhatViewModel : NotifyMaster, IFindWhatViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FindWhatViewModel));

    private readonly ISettingsDbController _dbController;
    private readonly IXmlSearchHistory<IObservableDictionary<string, string>> _searchHistoryController;

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

    private IObservableDictionary<string, string> _searchHistory;

    /// <summary>
    /// Search history
    /// </summary>
    public IObservableDictionary<string, string> SearchHistory
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
        OnPropertyChanged();
      }
    }

    private KeyValuePair<string, string> _selectedItem;

    /// <summary>
    /// Selected item
    /// </summary>
    public KeyValuePair<string, string> SelectedItem
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
      _searchHistoryController = new XmlSearchHistoryController();

      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += LoadedPropertyChanged;
      ((AsyncCommand<object>) FindNextCommand).PropertyChanged += FindNextCommandPropertyChanged;
    }

    #region Commands

    private IAsyncCommand _loadeCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadeCommand ?? (_loadeCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

    private ICommand _closingCommand;

    /// <summary>
    /// Cloasing command
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

    #endregion

    #region Command functions

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
        _searchHistory = await _searchHistoryController.ReadXmlFileAsync().ConfigureAwait(false);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void ExecuteClosingCommand()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<FindWhatCountResponseMessage>(OnFindWhatCountResponse);

      SettingsHelperController.CurrentSettings.FindDialogPositionX = LeftPosition;
      SettingsHelperController.CurrentSettings.FindDialogPositionY = TopPosition;

      _dbController.UpdateFindDialogDbSettings();
    }

    /// <summary>
    /// Can execute find command
    /// </summary>
    /// <returns><c>True</c> if it can execute otherwise <c>False</c></returns>
    public bool CanExecuteFindCommand()
    {
      if ( FindSettings != null && FindSettings.SearchBookmarks )
        return true;

      return !string.IsNullOrWhiteSpace(SearchText);
    }

    private async Task ExecuteFindNextCommandAsync()
    {
      FindSettings.CountFind = false;
      SearchFieldHasFocus = false;

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

      _searchHistoryController.Wrap = FindSettings.Wrap;
      await _searchHistoryController.SaveSearchHistoryWrapAttributeAsync().ConfigureAwait(false);
    }

    #endregion

    #region HelperFunctions

    private async Task HandleFindAsync()
    {
      MouseService.SetBusyState();

      if ( !SearchHistory.ContainsKey(SearchText) && !string.IsNullOrWhiteSpace(SearchText) )
      {
        _searchHistory.Add(new KeyValuePair<string, string>(SearchText, SearchText));
        await _searchHistoryController.SaveSearchHistoryAsync(SearchText).ConfigureAwait(false);
      }
    }

    #endregion

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

      TopPosition = SettingsHelperController.CurrentSettings.FindDialogPositionY;
      LeftPosition = SettingsHelperController.CurrentSettings.FindDialogPositionX;
      FindSettings = new FindData
      {
        Wrap = _searchHistoryController.Wrap
      };

      SearchFieldHasFocus = true;

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<FindWhatCountResponseMessage>(OnFindWhatCountResponse);
      OnPropertyChanged(nameof(SearchHistory));
    }

    private void OnFindWhatCountResponse(FindWhatCountResponseMessage args)
    {
      if ( WindowGuid != args.WindowGuid )
        return;

      CountMatches = string.Format(Application.Current.TryFindResource("FindDialogSearchCount").ToString(), args.Count);
    }
  }
}
