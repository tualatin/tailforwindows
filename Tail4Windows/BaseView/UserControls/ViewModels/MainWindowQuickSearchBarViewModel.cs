using System;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.BaseView.UserControls.Interfaces;
using Org.Vs.TailForWin.BaseView.ViewModels;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.UI.Vml.Attributes;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.Data.Messages.QuickSearchbar;


namespace Org.Vs.TailForWin.BaseView.UserControls.ViewModels
{
  /// <summary>
  /// MainWindowQuickSearch view model
  /// </summary>
  [Locator(nameof(MainWindowQuickSearchBarViewModel))]
  public class MainWindowQuickSearchBarViewModel : NotifyMaster, IMainWindowQuickSearchViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(MainWindowQuickSearchBarViewModel));

    private bool _isFocused;

    /// <summary>
    /// Has quick search textbox the focus
    /// </summary>
    public bool IsFocused
    {
      get => _isFocused;
      set
      {
        _isFocused = value;
        OnPropertyChanged(nameof(IsFocused));
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

    /// <summary>
    /// Window id
    /// </summary>
    public Guid WindowGuid
    {
      get;
      set;
    }

    #region Commands

    private ICommand _quickSearchCommand;

    /// <summary>
    /// Quick search command
    /// </summary>
    public ICommand QuickSearchCommand => _quickSearchCommand ?? (_quickSearchCommand = new RelayCommand(p => ExecuteQuickSearchCommand()));

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public MainWindowQuickSearchBarViewModel()
    {
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<QuickSearchTextBoxGetFocusMessage>(OnFocusChangedMessage);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ChangeWindowGuiMessage>(OnChangeWindowGuid);
    }

    private void OnChangeWindowGuid(ChangeWindowGuiMessage args) => WindowGuid = args.WindowGuid;

    private void OnFocusChangedMessage(QuickSearchTextBoxGetFocusMessage args)
    {
      IsFocused = false;

      if ( args.Sender is T4WindowViewModel )
        IsFocused = args.IsFocused;
    }

    #region Command functions

    private void ExecuteQuickSearchCommand()
    {
      if ( string.IsNullOrWhiteSpace(SearchText) )
        return;

      LOG.Trace("Execute quick search...");

      var findSettings = new FindData
      {
        WholeWord = true
      };
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new StartSearchAllMessage(WindowGuid, findSettings, SearchText));

      IsFocused = false;
      SearchText = string.Empty;
    }

    #endregion
  }
}
