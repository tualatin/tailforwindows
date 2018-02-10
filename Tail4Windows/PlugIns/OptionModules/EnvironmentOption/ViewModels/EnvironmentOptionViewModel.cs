using System.Windows;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Environment option view model
  /// </summary>
  public class EnvironmentOptionViewModel : NotifyMaster, IOptionPage
  {
    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle
    {
      get;
    } = Application.Current.TryFindResource("EnvironmentPageTitle").ToString();
  }
}
