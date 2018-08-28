using System;
using System.Windows;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.Data.Messages.FindWhat;
using Org.Vs.TailForWin.PlugIns.FindModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.FindModule
{
  /// <summary>
  /// Interaction logic for FindResult.xaml
  /// </summary>
  public partial class FindWhatResult
  {
    private readonly IFindWhatResultViewModel _findWhatResultViewModel;
    private Action<FindWhatResultMessage> _findWhatResultHandler;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindWhatResult()
    {
      InitializeComponent();

      _findWhatResultViewModel = (FindWhatResultViewModel) DataContext;
      _findWhatResultHandler = EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<FindWhatResultMessage>(OnShowFindWhatResults);
    }

    private void FindResultOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if ( IsVisible )
      {
        if ( _findWhatResultHandler != null )
          return;

        _findWhatResultHandler = EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<FindWhatResultMessage>(OnShowFindWhatResults);
      }
      else
      {
        EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<FindWhatResultMessage>(OnShowFindWhatResults);
        _findWhatResultHandler = null;
      }
    }

    private void OnShowFindWhatResults(FindWhatResultMessage args)
    {
      if ( _findWhatResultViewModel == null )
        return;

      _findWhatResultViewModel.FindWhatResultSource = args.FindWhatResults;
      _findWhatResultViewModel.WindowGuid = args.WindowGuid;
    }
  }
}
