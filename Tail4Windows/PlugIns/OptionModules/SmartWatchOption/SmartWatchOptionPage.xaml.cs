using System;
using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.SmartWatchOption
{
  /// <summary>
  /// Interaction logic for SmartWatchOptionPage.xaml
  /// </summary>
  public partial class SmartWatchOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("SmartWatchPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("bde68ff9-54b7-4cc7-91e0-9e6ad2f021f6");
  }
}
