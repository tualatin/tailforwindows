using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for EnvironmentOptionPage.xaml
  /// </summary>
  public partial class EnvironmentOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public EnvironmentOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("EnvironmentOptionPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("add169f3-968a-4b4d-b292-4e0cc8637dab");
  }
}
