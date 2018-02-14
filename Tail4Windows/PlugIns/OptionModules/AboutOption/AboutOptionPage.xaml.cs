using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption
{
  /// <summary>
  /// Interaction logic for AboutOptionPage.xaml
  /// </summary>
  public partial class AboutOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public AboutOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("AboutOptionPageTitle").ToString();
  }
}
