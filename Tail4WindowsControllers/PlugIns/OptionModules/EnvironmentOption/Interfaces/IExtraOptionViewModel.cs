using System.Windows.Input;
using System.Windows.Media;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Extra option view model interface
  /// </summary>
  public interface IExtraOptionViewModel : IOptionBaseViewModel
  {
    /// <summary>
    /// ImageSource
    /// </summary>
    ImageSource IconSource
    {
      get;
    }

    /// <summary>
    /// Selected editor command
    /// </summary>
    ICommand SelectEditorCommand
    {
      get;
    }
  }
}
