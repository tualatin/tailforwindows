using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.PatternUtil.Interfaces;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.PatternUtil.Utils
{
  /// <summary>
  /// Search pattern controller
  /// </summary>
  public class SearchPatternController : IDisposable, ISearchPatternController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SearchPatternController));

    private TailLogData currentProperty;


    /// <summary>
    /// Get current log file by certain saved pattern
    /// </summary>
    /// <param name="tailProperty">TailForLog property</param>
    /// <returns>Log file with full path otherwise null</returns>
    /// <exception cref="ArgumentException">If tailProperty is null</exception>
    public string GetCurrentFileByPattern(TailLogData tailProperty)
    {
      Arg.NotNull(tailProperty, "TailProperty");

      currentProperty = (TailLogData) tailProperty.Clone();

      return (GetLatestFileByPattern());
    }

    private string GetLatestFileByPattern()
    {
      var path = Path.GetDirectoryName(currentProperty.FileName);
      DirectoryInfo directoryInfo = new DirectoryInfo(path);

      if(directoryInfo == null || !directoryInfo.Exists)
        return (null);

      FileInfo[] files;

      if(!currentProperty.IsRegex)
      {
        files = directoryInfo.GetFiles(currentProperty.PatternString, SearchOption.TopDirectoryOnly);
      }
      else
      {
        try
        {
          Regex regex = new Regex(currentProperty.PatternString, RegexOptions.None);
          files = directoryInfo.GetFiles().Where(p => regex.IsMatch(p.Name)).ToArray();
        }
        catch(ArgumentNullException ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          files = null;
        }
      }

      if(files == null || files.Length == 0)
        return (null);

      DateTime latestWrite = DateTime.MinValue;
      FileInfo latestFile = null;

      foreach(var item in files)
      {
        if(item.LastWriteTime > latestWrite)
        {
          latestFile = item;
          latestWrite = item.LastWriteTime;
        }
      }
      return (latestFile.FullName);
    }

    #region IDisposable Members

    /// <summary>
    /// Releases all resources used by SearchPatternController.
    /// </summary>
    public void Dispose()
    {
      currentProperty.Dispose();
    }

    #endregion
  }
}
