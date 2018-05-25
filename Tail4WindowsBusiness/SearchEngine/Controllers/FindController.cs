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
    /// <returns><c>True</c> if match, otherwise <c>False</c></returns>
    public async Task<bool> MatchTextAsync(FindData findSettings, string value, string pattern, CancellationToken token)
    {
      if ( string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern) )
        return false;

      bool result = false;
      IsBusy = true;

      await Task.Run(
        () =>
        {
          lock ( FindControllerLock )
          {
            // if not case sensitive
            if ( !findSettings.CaseSensitive )
              value = value.ToLower();

            // use wild cards as '*' or '?'
            if ( findSettings.UseWildcard )
            {
              string regString = WildCardToRegular(pattern);
              result = Regex.IsMatch(value, regString);
              return;
            }

            Regex regex;

            // searching a whole word with regex
            if ( findSettings.WholeWord && findSettings.UseRegex )
            {
              regex = new Regex($"\\b({pattern})\\b");
              result = regex.IsMatch(value);
              return;
            }

            // searching a whole word
            if ( findSettings.WholeWord )
            {
              result = value.Contains(pattern);
              return;
            }

            // searching with regex
            if ( !findSettings.UseRegex )
              return;

            regex = new Regex(pattern);
            result = regex.IsMatch(value);
          }
        }, token).ConfigureAwait(false);

      IsBusy = false;
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
