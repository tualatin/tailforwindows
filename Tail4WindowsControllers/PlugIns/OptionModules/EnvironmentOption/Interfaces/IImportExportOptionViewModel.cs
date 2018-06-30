using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.UI.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Import export option view model interface
  /// </summary>
  public interface IImportExportOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// Current settings path
    /// </summary>
    string CurrentSettingsPath
    {
      get; set;
    }

    /// <summary>
    /// Export current settings command
    /// </summary>
    IAsyncCommand ExportCommand
    {
      get;
    }

    /// <summary>
    /// Import new settings command
    /// </summary>
    IAsyncCommand ImportCommand
    {
      get;
    }

    /// <summary>
    /// Reset current settings command
    /// </summary>
    IAsyncCommand ResetSettingsCommand
    {
      get;
    }
  }
}
