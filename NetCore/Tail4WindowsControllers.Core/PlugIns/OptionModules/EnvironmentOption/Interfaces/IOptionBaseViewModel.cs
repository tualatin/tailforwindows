using Org.Vs.Tail4Win.Controllers.UI.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
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
