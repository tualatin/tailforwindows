namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces
{
  /// <summary>
  /// Import/Export controller interface
  /// </summary>
  public interface IImportExportController
  {
    /// <summary>
    /// Export user settings
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If successful <c>True</c> otherwise <c>False</c></returns>
    Task ExportUserSettingsAsync(string fileName, CancellationToken token);

    /// <summary>
    /// Import user settings
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If successful <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ImportUserSettingsAsync(string fileName, CancellationToken token);
  }
}
