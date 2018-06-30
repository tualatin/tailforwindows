using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AlertOption
{
  /// <summary>
  /// Interaction logic for AlertOptionPage.xaml
  /// </summary>
  public partial class AlertOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public AlertOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("AlertOptionPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("a9c97f17-c00e-405a-a6ae-5834230ee4c6");
  }
}
