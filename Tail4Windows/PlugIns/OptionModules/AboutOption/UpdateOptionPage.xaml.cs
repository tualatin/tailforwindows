using System;
using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


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

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("a8108c5f-1c1e-4898-bff0-7d9badac5456");
  }
}
