namespace Org.Vs.TailForWin.BaseView.Interfaces
{
  /// <summary>
  /// BaseWindowStatusbar view model interface
  /// </summary>
  public interface IBaseWindowStatusbarViewModel
  {
    /// <summary>
    /// CurrentStatusBarBackground color as string
    /// </summary>
    string CurrentBusyState
    {
      get; set;
    }

    /// <summary>
    /// CurrentBusy state
    /// </summary>
    string CurrentStatusBarBackgroundColorHex
    {
      get; set;
    }
  }
}
