using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


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

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("abcbce64-6928-46d2-a948-fea7df675266");
  }
}
