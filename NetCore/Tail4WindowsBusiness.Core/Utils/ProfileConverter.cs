using log4net;
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Core.Logging;
using Org.Vs.TailForWin.Core.Utils;
using Application = System.Windows.Application;

namespace Org.Vs.TailForWin.Business.Utils
{
  /// <summary>
  /// Converts a roaming profile, if exists, to local profile <c>Or</c>
  /// Converts a local profile, if exists, to roaming profile
  /// </summary>
  public class ProfileConverter : IProfileConverter
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ProfileConverter));

    private readonly string _roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $@"\{CoreEnvironment.ApplicationTitle}";
    private readonly string _localPath = CoreEnvironment.ApplicationPath + @"\Settings";
    private CancellationTokenSource _cts;

    /// <summary>
    /// Converts a roaming profile into local profile
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    public async Task ConvertIntoLocalProfileAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

      await Task.Run(() =>
      {
        if ( !Directory.Exists(_roamingPath) )
          return;

        LOG.Info("Convert roaming profile into local profile");

        try
        {
          var dirInfo = CreateDirectoryIfNotExists(_localPath);
          var files = Directory.GetFiles(_roamingPath, "*.*", SearchOption.TopDirectoryOnly);

          MoveFiles(dirInfo, files);

          Directory.Delete(_roamingPath);
          InteractionService.ShowInformationMessageBox(Application.Current.TryFindResource("ConvertProfileSuccess").ToString());
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("ConvertProfileFailed").ToString());
        }
      }, _cts.Token);
    }

    /// <summary>
    /// Converts a local profile into roaming profile
    /// </summary>
    /// <returns><see cref="Task"/></returns>
    public async Task ConvertIntoRoamingProfileAsync()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

      await Task.Run(() =>
      {
        if ( !Directory.Exists(_localPath) )
          return;

        LOG.Info("Convert local profile into roaming profile");

        try
        {
          var dirInfo = CreateDirectoryIfNotExists(_roamingPath);
          var files = Directory.GetFiles(_localPath, "*.*", SearchOption.TopDirectoryOnly);

          MoveFiles(dirInfo, files);

          Directory.Delete(_localPath);
          InteractionService.ShowInformationMessageBox(Application.Current.TryFindResource("ConvertProfileSuccess").ToString());
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
          InteractionService.ShowErrorMessageBox(Application.Current.TryFindResource("ConvertProfileFailed").ToString());
        }
      }, _cts.Token);
    }

    private void MoveFiles(DirectoryInfo dirInfo, string[] files)
    {
      foreach ( string file in files )
      {
        var fileInfo = new FileInfo(file);
        var existsFile = new FileInfo(dirInfo + $@"\{fileInfo.Name}");

        if ( existsFile.Exists )
          return;

        fileInfo.MoveTo(dirInfo + $@"\{fileInfo.Name}");
      }
    }

    private DirectoryInfo CreateDirectoryIfNotExists(string path)
    {
      var dirInfo = new DirectoryInfo(path);

      if ( !dirInfo.Exists )
        Directory.CreateDirectory(path);

      return dirInfo;
    }
  }
}
