using System;
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

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("bb1ac306-643f-49d0-a7c9-6e6532a6dd17");
  }
}
