using System.Windows;
using Org.Vs.TailForWin.Business.Interfaces;


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
  }
}
