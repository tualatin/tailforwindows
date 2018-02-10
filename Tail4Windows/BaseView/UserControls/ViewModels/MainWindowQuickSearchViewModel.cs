using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.BaseView.ViewModels;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.BaseView.UserControls.ViewModels
{
  /// <summary>
  /// MainWindowQuickSearch view model
  /// </summary>
  public class MainWindowQuickSearchViewModel : NotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(MainWindowQuickSearchViewModel));

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

    #region Commands

    private IAsyncCommand _quickSearchCommand;

    /// <summary>
    /// Quick search command
    /// </summary>
    public IAsyncCommand QuickSearchCommand => _quickSearchCommand ?? (_quickSearchCommand = AsyncCommand.Create(ExecuteQuickSearchCommandAsync));

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public MainWindowQuickSearchViewModel() => EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<QuickSearchTextBoxGetFocusMessage>(FocusChangedMessage);

    private void FocusChangedMessage(QuickSearchTextBoxGetFocusMessage args)
    {
      if ( args.Sender is T4WindowViewModel )
        IsFocused = args.IsFocused;
    }

    #region Command functions

    private async Task ExecuteQuickSearchCommandAsync()
    {
      LOG.Trace("Execute quick search...");

      await Task.Run(
        () =>
        {
          IsFocused = false;
        }).ConfigureAwait(false);
    }

    #endregion
  }
}
