using System.Windows;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.ViewModels
{
  /// <summary>
  /// About option view model
  /// </summary>
  public class AboutOptionViewModel : NotifyMaster, IOptionPage
  {
    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle
    {
      get;
    } = Application.Current.TryFindResource("AboutPageTitle").ToString();
  }
}
