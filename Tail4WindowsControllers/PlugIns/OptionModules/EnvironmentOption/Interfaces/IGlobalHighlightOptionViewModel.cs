using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Global highlight option view model interface
  /// </summary>
  public interface IGlobalHighlightOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// Global highlight collection
    /// </summary>
    ObservableCollection<FilterData> GlobalHighlightCollection
    {
      get;
    }
  }
}
