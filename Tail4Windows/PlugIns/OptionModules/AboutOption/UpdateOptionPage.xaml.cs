using System.Windows;
using Org.Vs.TailForWin.Business.Interfaces;

namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption
{
  /// <summary>
  /// Interaction logic for UpdateOptionPage.xaml
  /// </summary>
  public partial class UpdateOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public UpdateOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("AboutOptionUpdatePageTitle").ToString();
  }
}
