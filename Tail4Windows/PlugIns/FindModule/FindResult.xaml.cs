﻿using System;
using System.Windows;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.FindModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.FindModule
{
  /// <summary>
  /// Interaction logic for FindResult.xaml
  /// </summary>
  public partial class FindResult
  {
    private readonly IFindResultViewModel _findWhatResultViewModel;
    private Action<FindWhatResultMessage> _findWhatResultHandler;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindResult()
    {
      InitializeComponent();

      _findWhatResultViewModel = (FindResultViewModel) DataContext;
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
    }
  }
}
