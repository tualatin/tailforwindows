using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for ImportExportOptionPage.xaml
  /// </summary>
  public partial class ImportExportOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ImportExportOptionPage() => InitializeComponent();

    /// <summary>
    /// Current option page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("EnvironmentImportExportPageTitle").ToString();
  }
}
