using System.Windows;
using Org.Vs.TailForWin.Business.Interfaces;


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
  }
}
