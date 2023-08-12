using Org.Vs.Tail4Win.Controllers.Commands.Interfaces;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Import export option view model interface
  /// </summary>
  public interface IImportExportOptionViewModel : IOptionBaseViewModel
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
