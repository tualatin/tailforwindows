using System.Windows;
using Org.Vs.Tail4Win.Core.Enums;

namespace Org.Vs.Tail4Win.Core.Data.Mappings
{
  /// <summary>
  /// Language mapping
  /// </summary>
  public class LanguageMapping
  {
    /// <summary>
    /// Language as Enum <see cref="EUiLanguage"/>
    /// </summary>
    public EUiLanguage Language
    {
      get;
      set;
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description
    {
      get
      {
        try
        {
          var resourceKey = Application.Current.TryFindResource(Language.ToString());

          return resourceKey?.ToString() ?? string.Empty;
        }
        catch
        {
          return string.Empty;
        }
      }
    }
  }
}
