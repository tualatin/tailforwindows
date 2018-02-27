using System;
using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption
{
  /// <summary>
  /// Interaction logic for SysInfoOptionPage.xaml
  /// </summary>
  public partial class SysInfoOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SysInfoOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("AboutOptionSysInfoTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("4fe8a3cb-cbba-49a5-a3f0-33fe835de0ea");
  }
}
