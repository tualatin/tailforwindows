using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;


namespace Org.Vs.TailForWin.Template.UpdateController
{
  /// <summary>
  /// Update controller
  /// </summary>
  public class UpdateController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(UpdateController));

    private readonly Version appVersion;
    private readonly List<Version> webVersions;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public UpdateController()
    {
      appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      webVersions = new List<Version>();
    }

    /// <summary>
    /// Latest webversion
    /// </summary>
    public Version WebVersion
    {
      get;
      private set;
    }

    /// <summary>
    /// Applications version
    /// </summary>
    public Version AppVersion => appVersion;

    /// <summary>
    /// Do check if main application needs to update
    /// </summary>
    /// <param name="webData">HTML stream</param>
    /// <param name="mainTag">HTML tag pattern</param>
    /// <returns>Should update true otherwise false</returns>
    public bool UpdateNecessary(string webData, string mainTag)
    {
      try
      {
        string tag = $"{mainTag}/tag/";
        string pattern = $"<a[^>]*href=(?:\"|\'){tag}([^>]*)";
        MatchCollection matches = Regex.Matches(webData, pattern, RegexOptions.IgnoreCase);

        if(matches.Count == 0)
          return false;

        foreach(Match match in matches)
        {
          string part = match.Value.Substring(match.Value.IndexOf(mainTag, StringComparison.Ordinal)).Substring(tag.Length);
          Regex regex = new Regex(@"\d+.\d+.\d+", RegexOptions.IgnoreCase);

          if(regex.Match(part).Success)
          {
            string version = regex.Match(part).Value;
            int major = -1, minor = -1, build = -1;

            Regex rxVersion = new Regex(@"\d+", RegexOptions.IgnoreCase);

            if(rxVersion.IsMatch(version))
            {
              Match mtVersion = rxVersion.Match(version);

              major = int.Parse(mtVersion.Value);
              int length = mtVersion.Length + 1;
              version = version.Substring(length, version.Length - length);

              if(rxVersion.IsMatch(version))
              {
                mtVersion = rxVersion.Match(version);
                minor = int.Parse(mtVersion.Value);
                length = mtVersion.Length + 1;
                version = version.Substring(length, version.Length - length);

                if(rxVersion.IsMatch(version))
                {
                  mtVersion = rxVersion.Match(version);
                  build = int.Parse(mtVersion.Value);
                }
              }
            }

            Version myVersion = new Version(major, minor, build);
            webVersions.Add(myVersion);
          }
        }

        GetLatestWebVersion();

        if(DoCompare())
          return true;
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return false;
    }

    private bool DoCompare()
    {
      foreach(Version version in webVersions)
      {
        var result = version.CompareTo(appVersion);

        if(result > 0)
          return true;
      }
      return false;
    }

    private void GetLatestWebVersion()
    {
      webVersions.Sort(new VersionComparer());
      WebVersion = webVersions[webVersions.Count - 1];
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

        if(xVersion != null)
          return xVersion.CompareTo(yVersion);

        return -1;
      }
    }
  }
}
