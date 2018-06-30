using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


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

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("bf8d441f-14f9-42f4-bcb5-db3e0ec3e774");
  }
}
