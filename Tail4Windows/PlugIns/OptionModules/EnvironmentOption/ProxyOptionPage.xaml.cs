using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for ProxyOptionPage.xaml
  /// </summary>
  public partial class ProxyOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ProxyOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("EnvironmentProxyOptionPageTitle").ToString();
  }
}
