using System;
using System.IO;
using System.Windows;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Language selector
  /// </summary>
  public static class LanguageSelector
  {
    /// <summary>
    /// Set language ResourceDictionary
    /// </summary>
    /// <param name="languageFile">Language file</param>
    public static void SetLanguageResourceDictionary(string languageFile)
    {
      if ( File.Exists(languageFile) )
      {
        // Read in ResourceDictionary File  
        var languageDictionary = new ResourceDictionary
        {
          Source = new Uri(languageFile)
        };
        // Remove any previous Localization dictionaries loaded  
        int langDictId = -1;

        for ( int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++ )
        {
          var md = Application.Current.Resources.MergedDictionaries[i];

          // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName
          // key and that it is set to a value starting with "Lang-".
          if ( !md.Contains("ResourceDictionaryName") )
            continue;

          if ( !md["ResourceDictionaryName"].ToString().StartsWith("Lang-") )
            continue;

          langDictId = i;
          break;
        }

        if ( langDictId == -1 )
        {
          // Add in newly loaded Resource Dictionary
          Application.Current.Resources.MergedDictionaries.Add(languageDictionary);
        }
        else
        {
          // Replace the current langage dictionary with the new one
          Application.Current.Resources.MergedDictionaries[langDictId] = languageDictionary;
        }
      }
      else
      {
        EnvironmentContainer.ShowErrorMessageBox($"{'"'} {languageFile} {'"'}  not found");
      }
    }
  }
}
