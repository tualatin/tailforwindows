﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Org.Vs.TailForWin.Core.Data;
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
    /// <returns>Should update <c>True</c> otherwise <c>False</c></returns>
    public async Task<UpdateData> UpdateNecessaryAsync()
    {
      Stopwatch stopUpdate = new Stopwatch();
      stopUpdate.Start();
      LOG.Trace("Check if update is necessary...");

      var matchUrl = Regex.Match(EnvironmentContainer.ApplicationUpdateWebUrl, "https://www.virtual-studios.de", RegexOptions.IgnoreCase);
      _result = new UpdateData
      {
        ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
      };

      if ( !matchUrl.Success )
        throw new WebException("Not a valid update URL, operation aborted!");

      string webRequest = await _webController.GetStringByUrlAsync(EnvironmentContainer.ApplicationUpdateWebUrl).ConfigureAwait(false);
      await Task.Run(() => UpdateNecessary(webRequest)).ConfigureAwait(false);
      stopUpdate.Stop();

      LOG.Trace("Checked in {0} ms", stopUpdate.ElapsedMilliseconds);

      return _result;
    }

    private void UpdateNecessary(string webData)
    {
      if ( string.IsNullOrWhiteSpace(webData) )
        throw new WebException("WebRequest was empty, operation aborted!");

      try
      {
        if ( !webData.Contains("\n") )
          return;

        var versions = webData.Split('\n').ToList();

        Parallel.ForEach(
          versions,
          p =>
          {
            if ( Version.TryParse(p, out var v) )
              _webVersions.Add(v);
          });

        SortWebVersions();
        DoCompareWebVersionWithApplicationVersion();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
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

      Parallel.ForEach(
        _webVersions,
        (f, state) =>
        {
          int res = f.CompareTo(_result.ApplicationVersion);

          if ( res <= 0 )
            return;

          _result.Update = true;
          state.Break();
        });
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
