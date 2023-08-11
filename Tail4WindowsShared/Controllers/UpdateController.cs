using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using log4net;
using Org.Vs.Tail4Win.Shared.Data;
using Org.Vs.Tail4Win.Shared.Interfaces;
using Org.Vs.Tail4Win.Shared.Utils;

namespace Org.Vs.Tail4Win.Shared.Controllers
{
  /// <summary>
  /// T4W update controller
  /// </summary>
  public class UpdateController : IUpdater
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(UpdateController));

    private static readonly SemaphoreSlim UpdateLock = new SemaphoreSlim(1);
    private readonly IWebController _webController;
    private readonly List<Version> _webVersions;
    private UpdateData _result;

    /// <summary>
    /// Standard constructor with WebController injection
    /// </summary>
    /// <param name="webController">WebController interface</param>
    public UpdateController(IWebController webController)
    {
      _webController = webController;
      _webVersions = new List<Version>();
    }

    /// <summary>
    /// Do check if main application needs to update
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="version">Current main application version</param>
    /// <returns>Should update <c>True</c> otherwise <c>False</c></returns>
    public async Task<UpdateData> UpdateNecessaryAsync(CancellationToken token, Version version)
    {
      var stopUpdate = new Stopwatch();
      stopUpdate.Start();
      LOG.Info("Check if update is necessary...");

      Match matchUrl = Regex.Match(CoreEnvironment.ApplicationUpdateWebUrl, CoreEnvironment.ApplicationRegexWebUrl, RegexOptions.IgnoreCase);
      _result = new UpdateData
      {
        ApplicationVersion = version
      };

      if ( !matchUrl.Success )
        throw new WebException("Not a valid update URL, operation aborted!");

      string webRequest = await _webController.GetStringByUrlAsync(CoreEnvironment.ApplicationUpdateWebUrl).ConfigureAwait(false);
      await UpdateNecessaryInternalAsync(webRequest, token).ConfigureAwait(false);

      stopUpdate.Stop();

      LOG.Trace("Checked in {0} ms", stopUpdate.ElapsedMilliseconds);

      return _result;
    }

    private async Task UpdateNecessaryInternalAsync(string webData, CancellationToken token)
    {
      if ( string.IsNullOrWhiteSpace(webData) )
        throw new WebException("WebRequest was empty, operation aborted!");

      await UpdateLock.WaitAsync(token).ConfigureAwait(false);

      try
      {
        if ( !webData.Contains("\n") )
          return;

        _webVersions.Clear();
        var versions = webData.Split('\n').ToList();

        Parallel.ForEach(versions, p =>
        {
          if ( p == null )
            return;

          if ( Version.TryParse(p, out var v) )
            _webVersions.Add(v);
        });

        SortWebVersions();
        DoCompareWebVersionWithApplicationVersion();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
      finally
      {
        UpdateLock.Release();
      }
    }

    private void SortWebVersions()
    {
      if ( _webVersions.Count == 0 )
        return;

      _webVersions.Sort(new VersionComparer());
      _result.WebVersion = _webVersions.Last();
    }

    private void DoCompareWebVersionWithApplicationVersion()
    {
      if ( _webVersions.Count == 0 )
        return;

      Parallel.ForEach(_webVersions, (f, state) =>
      {
        if ( f == null )
          return;

        int res = f.CompareTo(_result.ApplicationVersion);

        if ( res <= 0 )
          return;

        _result.Update = true;
        state.Break();
      });
    }

    private class VersionComparer : IComparer<Version>
    {
      /// <summary>
      /// Compares two versions
      /// </summary>
      /// <param name="x">Version X</param>
      /// <param name="y">Version Y</param>
      /// <returns>A signed integer that indicates the relative values of x and y, as shown in the following table.</returns>
      public int Compare(Version x, Version y)
      {
        var xVersion = x;
        var yVersion = y;

        return xVersion != null ? xVersion.CompareTo(yVersion) : -1;
      }
    }
  }
}
