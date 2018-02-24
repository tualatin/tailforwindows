using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for ExtraOptionPage.xaml
  /// </summary>
  public partial class ExtraOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExtraOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("ExtrasPageTitle").ToString();
  }
}
