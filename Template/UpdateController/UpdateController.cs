using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using TailForWin.Utils;


namespace TailForWin.Template.UpdateController
{
  public class UpdateController
  {
    private readonly Version appVersion;
    private readonly List<Version> webVersions;


    public UpdateController ()
    {
      appVersion = System.Reflection.Assembly.GetExecutingAssembly ( ).GetName ( ).Version;
      webVersions = new List<Version> ( );
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
    public Version AppVersion
    {
      get
      {
        return (appVersion);
      }
    }

    /// <summary>
    /// Do check if main application needs to update
    /// </summary>
    /// <param name="webData">HTML stream</param>
    /// <param name="mainTag">HTML tag pattern</param>
    /// <returns>Should update true otherwise false</returns>
    public bool UpdateNecessary (string webData, string mainTag)
    {
      try
      {
        string tag = string.Format ("{0}/tag/", mainTag);
        string pattern = string.Format ("<a[^>]*href=(?:\"|\'){0}([^>]*)", tag);
        MatchCollection matches = Regex.Matches (webData, pattern, RegexOptions.IgnoreCase);

        if (matches.Count == 0)
          return (false);

        foreach (Match match in matches)
        {
          string part = (match.Value.Substring (match.Value.IndexOf (mainTag, StringComparison.Ordinal))).Substring (tag.Length);
          Regex regex = new Regex (@"\d+.\d+.\d+", RegexOptions.IgnoreCase);

          if (regex.Match (part).Success)
          {
            string version = regex.Match (part).Value;
            int major = -1, minor = -1, build = -1;

            Regex rxVersion = new Regex (@"\d+", RegexOptions.IgnoreCase);

            if (rxVersion.IsMatch (version))
            {
              Match mtVersion = rxVersion.Match (version);

              major = int.Parse (mtVersion.Value);
              int length = mtVersion.Length + 1;
              version = version.Substring (length, version.Length - length);

              if (rxVersion.IsMatch (version))
              {
                mtVersion = rxVersion.Match (version);
                minor = int.Parse (mtVersion.Value);
                length = mtVersion.Length + 1;
                version = version.Substring (length, version.Length - length);

                if (rxVersion.IsMatch (version))
                {
                  mtVersion = rxVersion.Match (version);
                  build = int.Parse (mtVersion.Value);
                }
              }
            }

            Version myVersion = new Version (major, minor, build);
            webVersions.Add (myVersion);
          }
        }

        GetLatestWebVersion ( );

        if (DoCompare ( ))
          return (true);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "UpdateController", string.Format ("UpdateNecessary exception: {0}", ex));
      }
      return (false);
    }

    private bool DoCompare ()
    {
      foreach (Version version in webVersions)
      {
        var result = version.CompareTo (appVersion);

        if (result > 0)
          return (true);
      }
      return (false);
    }

    private void GetLatestWebVersion ()
    {
      webVersions.Sort (new VersionComparer ( ));
      WebVersion = webVersions[webVersions.Count - 1];
    }

    private class VersionComparer : IComparer<Version>
    {
      public int Compare (Version x, Version y)
      {
        var xVersion = x;
        var yVersion = y;

        return (xVersion.CompareTo (yVersion));
      }
    }
  }
}
