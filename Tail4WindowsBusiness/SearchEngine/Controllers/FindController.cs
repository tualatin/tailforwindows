using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Business.SearchEngine.Controllers
{
  /// <summary>
  /// FindController
  /// </summary>
  public class FindController : IFindController
  {
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
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>List of valid strings, otherwise null</returns>
    public async Task<List<string>> MatchTextAsync(FindData findSettings, string value, string pattern, CancellationToken token)
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
            // if not case sensitive
            if ( !findSettings.CaseSensitive )
              value = value.ToLower();

            Regex regex;

            // use wild cards as '*' or '?'
            if ( findSettings.UseWildcard )
            {
              string regString = WildCardToRegular(pattern);
              regex = new Regex(regString);

              if ( regex.IsMatch(value) )
                return;

              result = GetStringResult(value, regex);
              return;
            }

            // searching a whole word with regex
            if ( findSettings.WholeWord && findSettings.UseRegex )
            {
              regex = new Regex($"\\b({pattern})\\b");

              if ( !regex.IsMatch(value) )
                return;

              result = GetStringResult(value, regex);
              return;
            }

            // searching a whole word
            if ( findSettings.WholeWord )
            {
              if ( !value.Contains(pattern) )
                return;

              regex = new Regex(pattern);
              result = GetStringResult(value, regex);
              return;
            }

            // searching with regex
            if ( !findSettings.UseRegex )
              return;

            regex = new Regex(pattern);

            if ( !regex.IsMatch(value) )
              return;

            result = GetStringResult(value, regex);
          }
        }, token).ConfigureAwait(false);

      IsBusy = false;
      return result;
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
    /// </summary>
    /// <param name="value">Value as string</param>
    /// <returns>A <see cref="Regex"/> string</returns>
    private static string WildCardToRegular(string value) => "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
  }
}
