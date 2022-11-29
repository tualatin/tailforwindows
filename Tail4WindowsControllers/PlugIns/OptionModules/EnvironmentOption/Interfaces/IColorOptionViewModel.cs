using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Color option view model interface
  /// </summary>
  public interface IColorOptionViewModel : IOptionBaseViewModel
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
