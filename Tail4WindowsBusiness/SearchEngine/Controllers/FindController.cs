using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
    /// FindControll is busy indicator
    /// </summary>
    public bool IsBusy
    {
      get;
      private set;
    }

    /// <summary>
    /// Mathes a text
    /// </summary>
    /// <param name="findSettings">Current find settings <see cref="FindData"/></param>
    /// <param name="value">Value as string</param>
    /// <param name="pattern">Search pattern</param>
    /// <returns>List of valid strings, otherwise null</returns>
    public async Task<List<string>> MatchTextAsync(FindData findSettings, string value, string pattern)
    {
      if ( string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern) )
        return null;

      List<string> result = null;
      IsBusy = true;

      await Task.Run(
        () =>
        {
          lock ( FindControllerLock )
          {
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

              regex = new Regex(pattern);
              result = GetStringResult(value, regex);
              return;
            }

            pattern = $@"{ignoreCase}{pattern}\w+|{ignoreCase}{pattern}";
            regex = new Regex(pattern);
            result = GetStringResult(value, regex);
          }
        }, new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token).ConfigureAwait(false);

      IsBusy = false;
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
          Regex.Match("", pattern);
        }
        catch ( ArgumentException )
        {
          // BAD PATTERN: Syntax error
          isValid = false;
          LOG.Error("{0} wrong Regex pattern!", System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
      }
      else
      {
        //BAD PATTERN: Pattern is null or blank
        isValid = false;
        LOG.Error("{0} wrong Regex pattern!", System.Reflection.MethodBase.GetCurrentMethod().Name);
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
