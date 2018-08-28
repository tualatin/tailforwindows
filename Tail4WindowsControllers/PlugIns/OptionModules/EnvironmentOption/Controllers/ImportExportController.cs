using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Controllers
{
  /// <summary>
  /// Import/Export controller
  /// </summary>
  public class ImportExportController : IImportExportController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ImportExportController));

    /// <summary>
    /// Export user settings
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If successful <c>True</c> otherwise <c>False</c></returns>
    public async Task ExportUserSettingsAsync(string fileName, CancellationToken token)
    {
      await Task.Run(() =>
      {
        string appName = AppDomain.CurrentDomain.FriendlyName;
        string appSettings = $"{AppDomain.CurrentDomain.BaseDirectory}{appName}.Config";

        try
        {
          ZipFile.CreateFromDirectory(EnvironmentContainer.UserSettingsPath, fileName, CompressionLevel.Optimal, false);

          // Add config file to Zip archive
          using ( ZipArchive zipArchive = ZipFile.Open(fileName, ZipArchiveMode.Update) )
          {
            var fileInfo = new FileInfo(appSettings);

            zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }, token);

    }

    /// <summary>
    /// Import user settings
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If successful <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> ImportUserSettingsAsync(string fileName, CancellationToken token)
    {
      var result = false;

      await Task.Run(() =>
      {
        try
        {
          foreach ( ZipArchiveEntry entry in ZipFile.Open(fileName, ZipArchiveMode.Read).Entries )
          {
            string fileNamePath = Path.Combine(EnvironmentContainer.UserSettingsPath, entry.FullName);

            if ( string.IsNullOrWhiteSpace(entry.Name) )
            {
              Directory.CreateDirectory(Path.GetDirectoryName(fileNamePath) ?? throw new InvalidOperationException());
              continue;
            }

            entry.ExtractToFile(fileNamePath, true);
          }

          // Copy config file to app folder
          string appName = AppDomain.CurrentDomain.FriendlyName;
          string appSettings = $"{AppDomain.CurrentDomain.BaseDirectory}{appName}.Config";
          string importConfig = $@"{EnvironmentContainer.UserSettingsPath}\{Path.GetFileName(appSettings)}";

          if ( !File.Exists(importConfig) )
            return;

          using ( var fs = new FileStream(importConfig, FileMode.Open) )
          {
            Stream output = File.Create(appSettings);
            var buffer = new byte[1024];
            int length;

            while ( (length = fs.Read(buffer, 0, buffer.Length)) > 0 )
            {
              output.Write(buffer, 0, length);
            }

            fs.Flush();
            output.Flush();

            fs.Close();
            output.Close();
          }

          // Delete config file from Roaming profile
          File.Delete(importConfig);
          result = true;
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }, token);

      return result;
    }
  }
}
