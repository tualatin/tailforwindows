using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Data;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Color option view model interface
  /// </summary>
  public interface IColorOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// Log viewer color data collection
    /// </summary>
    ObservableCollection<ControlColorData> LogViewerColorData
    {
      get;
    }

    /// <summary>
    /// Statusbar color data collection
    /// </summary>
    ObservableCollection<ControlColorData> StatusbarColorData
    {
      get;
    }
  }
}
