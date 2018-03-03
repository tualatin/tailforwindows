using System;
using System.Windows;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption
{
  /// <summary>
  /// Interaction logic for SmtpOptionPage.xaml
  /// </summary>
  public partial class SmtpOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmtpOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("AlertSmtpOptionPageTitle").ToString();

    /// <summary>
    /// Page GuId
    /// </summary>
    public Guid PageId => Guid.Parse("cfc162ef-5755-4958-a559-ab893ca8e1ed");
  }
}
