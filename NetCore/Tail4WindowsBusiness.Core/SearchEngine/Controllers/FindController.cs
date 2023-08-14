using System.Text.RegularExpressions;
using log4net;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Business.SearchEngine.Controllers
{
  /// <summary>
  /// FindController
  /// </summary>
  public class FindController : IFindController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FindController));
    private static readonly object FindControllerLock = new object();

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    /// <summary>
    /// FindController is busy indicator
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    /// <summary>
    /// Matches a text
    /// </summary>
    /// <param name="findSettings">Current find settings <see cref="FindData"/></param>
    /// <param name="value">Value as string</param>
    /// <param name="pattern">Search pattern</param>
    /// <returns>List of valid strings, otherwise null</returns>
    public async Task<List<string>> MatchTextAsync(FindData findSettings, string value, string pattern)
    {
      List<string> result = null;

      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
      {
        await Task.Run(() =>
        {
          if ( Monitor.TryEnter(FindControllerLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
          {
            try
            {
              if ( string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern) )
                return;

              IsBusy = true;

              value = value.Trim();
              pattern = pattern.Trim();
              string ignoreCase = string.Empty;

              // if not case sensitive
              if ( !findSettings.CaseSensitive )
                ignoreCase = "(?i)";

              Regex regex;

              // use wild cards as '*' or '?'
              if ( findSettings.UseWildcard )
              {
                string regString = WildCardToRegular(pattern);
                regex = new Regex(ignoreCase + regString);

                if ( !regex.IsMatch(value) )
                  return;

                result = GetStringResult(value, regex);
                return;
              }

              // searching a whole word with regex
              if ( findSettings.WholeWord && findSettings.UseRegex )
              {
                if ( !VerifyRegex(pattern) )
                  return;

                regex = new Regex(ignoreCase + $"\\b({pattern})\\b");
                result = GetStringResult(value, regex);
                return;
              }

              // searching a whole word
              if ( findSettings.WholeWord )
              {
                regex = new Regex(ignoreCase + $"\\b{pattern}\\b");
                result = GetStringResult(value, regex);
                return;
              }

              // searching with regex
              if ( findSettings.UseRegex )
              {
                if ( !VerifyRegex(pattern) )
                  return;

                regex = new Regex(ignoreCase + pattern);
                result = GetStringResult(value, regex);
                return;
              }

              pattern = $@"{ignoreCase}{pattern}\w+|{ignoreCase}{pattern}|{ignoreCase}\w+{pattern}";
              regex = new Regex(pattern);
              result = GetStringResult(value, regex);
            }
            finally
            {
              IsBusy = false;
              Monitor.Exit(FindControllerLock);
            }
          }
          else
          {
            LOG.Error("Can not lock!");
          }
        }, cts.Token).ConfigureAwait(false);
      }
      return result;
    }

    private bool VerifyRegex(string pattern)
    {
      bool isValid = true;

      if ( pattern != null && pattern.Trim().Length > 0 )
      {
        try
        {
          // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
          Regex.Match(string.Empty, pattern);
        }
        catch ( ArgumentException )
        {
          // BAD PATTERN: Syntax error
          isValid = false;
          LOG.Error($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name} wrong Regex pattern!");
        }
      }
      else
      {
        //BAD PATTERN: Pattern is null or blank
        isValid = false;
        LOG.Error($"{System.Reflection.MethodBase.GetCurrentMethod()?.Name} wrong Regex pattern!");
      }
      return isValid;
    }

    private static List<string> GetStringResult(string value, Regex regex)
    {
      var result = new List<string>();

      foreach ( Match match in regex.Matches(value) )
      {
        result.Add(match.Value);
      }
      return result;
    }

    /// <summary>
    /// WildCard to regular expression
    /// ? - any character  (one and only one)
    /// * - any characters(zero or more)
    /// </summary>
    /// <param name="value">Value as string</param>
    /// <returns>A <see cref="Regex"/> string</returns>
    private static string WildCardToRegular(string value) => Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*");
  }
}
