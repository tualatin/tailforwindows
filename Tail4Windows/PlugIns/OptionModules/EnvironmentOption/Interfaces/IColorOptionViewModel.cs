using System.Collections.ObjectModel;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Data;


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
