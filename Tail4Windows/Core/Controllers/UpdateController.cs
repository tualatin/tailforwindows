using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Controllers
{
  /// <summary>
  /// T4W update controller
  /// </summary>
  public class UpdateController : IUpdater
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(UpdateController));

    private readonly IWebController _webController;
    private readonly List<Version> _webVersions;


    /// <summary>
    /// Current Web version
    /// </summary>
    public Version WebVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Current Application version
    /// </summary>
    public Version AppVersion
    {
      get;
      set;
    } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

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
    /// <returns>Should update <c>True</c> otherwise <c>False</c></returns>
    public async Task<bool> UpdateNecessaryAsync()
    {
      Stopwatch stopUpdate = new Stopwatch();
      stopUpdate.Start();
      LOG.Trace("Check if update is necessary...");

      Match matchUrl = Regex.Match(EnvironmentContainer.ApplicationUpdateWebUrl, "https://github.com", RegexOptions.IgnoreCase);

      if ( !matchUrl.Success )
        throw new WebException("Not a valid update URL, operation aborted!");

      string tag = EnvironmentContainer.ApplicationUpdateWebUrl.Substring(matchUrl.Value.Length, EnvironmentContainer.ApplicationUpdateWebUrl.Length - matchUrl.Value.Length);
      string webRequest = await _webController.GetStringByUrlAsync(EnvironmentContainer.ApplicationUpdateWebUrl).ConfigureAwait(false);
      bool result = await Task.Run(() => UpdateNecessary(webRequest, tag)).ConfigureAwait(false);
      stopUpdate.Stop();

      LOG.Trace("Checked in {0} ms", stopUpdate.ElapsedMilliseconds);

      return result;
    }

    private bool UpdateNecessary(string webData, string tag)
    {
      if ( string.IsNullOrWhiteSpace(webData) )
        throw new WebException("WebRequest was empty, operation aborted!");

      try
      {
        string mainTag = $"{tag}/tag/";
        string pattern = $"<a[^>]*href=(?:\"|\'){tag}([^>]*)";
        MatchCollection matches = Regex.Matches(webData, pattern, RegexOptions.IgnoreCase);

        if ( matches.Count == 0 )
          return false;

        Parallel.ForEach(
          matches.OfType<Match>(),
          (f, state) =>
          {
            try
            {
              string part = f.Value.Substring(f.Value.IndexOf(mainTag, StringComparison.Ordinal)).Substring(tag.Length);
              Regex regex = new Regex(@"\d+.\d+.\d+", RegexOptions.IgnoreCase);

              if ( !regex.Match(part).Success )
                return;

              string version = regex.Match(part).Value;
              int major = -1, minor = -1, build = -1;
              Regex rxVersion = new Regex(@"\d+", RegexOptions.IgnoreCase);

              if ( rxVersion.IsMatch(version) )
              {
                Match mtVersion = rxVersion.Match(version);

                major = int.Parse(mtVersion.Value);
                int length = mtVersion.Length + 1;
                version = version.Substring(length, version.Length - length);

                if ( rxVersion.IsMatch(version) )
                {
                  mtVersion = rxVersion.Match(version);
                  minor = int.Parse(mtVersion.Value);
                  length = mtVersion.Length + 1;
                  version = version.Substring(length, version.Length - length);

                  if ( rxVersion.IsMatch(version) )
                  {
                    mtVersion = rxVersion.Match(version);
                    build = int.Parse(mtVersion.Value);
                  }
                }
              }

              if ( state.ShouldExitCurrentIteration )
                state.Break();

              Version myVersion = new Version(major, minor, build);
              _webVersions.Add(myVersion);
            }
            catch
            {
              // nothing
            }
          });

        SortWebVersions();

        return DoCompareWebVersionWithApplicationVersion();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
      return false;
    }

    private void SortWebVersions()
    {
      if ( _webVersions.Count == 0 )
        return;

      _webVersions.Sort(new VersionComparer());
      WebVersion = _webVersions.Last();
    }

    private bool DoCompareWebVersionWithApplicationVersion()
    {
      if ( _webVersions.Count == 0 )
        return false;

      bool result = false;

      Parallel.ForEach(
        _webVersions,
        (f, state) =>
        {
          int res = f.CompareTo(AppVersion);

          if ( res <= 0 )
            return;

          result = true;
          state.Break();
        });
      return result;
    }

    #region VersionComparer

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

        if ( xVersion != null )
          return xVersion.CompareTo(yVersion);

        return -1;
      }
    }

    #endregion
  }
}
