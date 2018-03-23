using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces
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
