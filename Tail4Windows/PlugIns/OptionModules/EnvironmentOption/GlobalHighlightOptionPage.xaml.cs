using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for GlobalHighlightOptionPage.xaml
  /// </summary>
  public partial class GlobalHighlightOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public GlobalHighlightOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("GlobalHighlightOptionPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("c7dbca5f-e6a5-4482-a6a8-2edf975d6a98");
  }
}
