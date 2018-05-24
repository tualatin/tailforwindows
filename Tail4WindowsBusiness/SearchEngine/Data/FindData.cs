using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Business.SearchEngine.Data
{
  /// <summary>
  /// FindData
  /// </summary>
  public class FindData : NotifyMaster
  {
    private bool _useRegex;

    /// <summary>
    /// Use regex
    /// </summary>
    public bool UseRegex
    {
      get => _useRegex;
      set
      {
        _useRegex = value;
        OnPropertyChanged();
      }
    }

    private bool _wholeWord;

    /// <summary>
    /// Whole words
    /// </summary>
    public bool WholeWord
    {
      get => _wholeWord;
      set
      {
        _wholeWord = value;
        OnPropertyChanged();
      }
    }

    private bool _caseSensitive;

    /// <summary>
    /// Case sensitive
    /// </summary>
    public bool CaseSensitive
    {
      get => _caseSensitive;
      set
      {
        _caseSensitive = value;
        OnPropertyChanged();
      }
    }

    private bool _useWildcard;

    /// <summary>
    /// Use wild cards
    /// </summary>
    public bool UseWildcard
    {
      get => _useWildcard;
      set
      {
        _useWildcard = value;
        OnPropertyChanged();
      }
    }
  }
}
