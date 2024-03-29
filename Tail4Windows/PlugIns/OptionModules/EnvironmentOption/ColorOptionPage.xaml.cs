﻿using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for ColorOptionPage.xaml
  /// </summary>
  public partial class ColorOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("EnvironmentColorOptionPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("dd213455-e73a-4e98-9b2b-0dffdec2b4be");

    /// <summary>
    /// Current page settings changed
    /// </summary>
    public bool PageSettingsChanged => false;

    /// <summary>
    /// Unloads the option page
    /// </summary>
    public void UnloadPage()
    {
      if ( !(DataContext is IOptionBaseViewModel viewModel) )
        return;

      viewModel.UnloadOptionViewModel();
    }
  }
}
