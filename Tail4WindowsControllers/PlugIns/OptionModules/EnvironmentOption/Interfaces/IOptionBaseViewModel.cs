using Org.Vs.TailForWin.Controllers.UI.Interfaces;

namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Option base view model interface
  /// </summary>
  public interface IOptionBaseViewModel : IViewModelBase
  {
    /// <summary>
    /// Unloads view model events
    /// </summary>
    void UnloadOptionViewModel();
  }
}
