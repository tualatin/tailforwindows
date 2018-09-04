using System.Windows.Input;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Extra option view model interface
  /// </summary>
  public interface IExtraOptionViewModel
  {
    /// <summary>
    /// Selected editor command
    /// </summary>
    ICommand SelectEditorCommand
    {
      get;
    }
  }
}
