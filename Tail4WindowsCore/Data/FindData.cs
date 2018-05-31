using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// FindData
  /// </summary>
  public class FindData : StateManager
  {
    private bool _wrap;

    /// <summary>
    /// Wrap search
    /// </summary>
    public bool Wrap
    {
      get => _wrap;
      set
      {
        if ( value == _wrap )
          return;

        _wrap = value;
        OnPropertyChanged();
      }
    }

    private bool _countFind;

    /// <summary>
    /// Counts search result
    /// </summary>
    public bool CountFind
    {
      get => _countFind;
      set
      {
        if ( value == _countFind )
          return;

        _countFind = value;
        OnPropertyChanged();
      }
    }

    private bool _searchBookmarks;

    /// <summary>
    /// Search bookmarks
    /// </summary>
    public bool SearchBookmarks
    {
      get => _searchBookmarks;
      set
      {
        if ( value == _searchBookmarks )
          return;

        _searchBookmarks = value;
        OnPropertyChanged();
      }
    }

    private bool _markLineAsBoomark;

    /// <summary>
    /// Mark result as bookmark
    /// </summary>
    public bool MarkLineAsBookmark
    {
      get => _markLineAsBoomark;
      set
      {
        if ( value == _markLineAsBoomark )
          return;

        _markLineAsBoomark = value;
        OnPropertyChanged();
      }
    }

    private bool _useRegex;

    /// <summary>
    /// Use regex
    /// </summary>
    public bool UseRegex
    {
      get => _useRegex;
      set
      {
        bool currentValue = _useRegex;
        ChangeState(new Command(() => _useRegex = value, () => _useRegex = currentValue, nameof(UseRegex), Notification));
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
        bool currentValue = _wholeWord;
        ChangeState(new Command(() => _wholeWord = value, () => _wholeWord = currentValue, nameof(WholeWord), Notification));
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
        bool currentValue = _caseSensitive;
        ChangeState(new Command(() => _caseSensitive = value, () => _caseSensitive = currentValue, nameof(CaseSensitive), Notification));
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
        bool currentValue = _useWildcard;
        ChangeState(new Command(() => _useWildcard = value, () => _useWildcard = currentValue, nameof(UseWildcard), Notification));
      }
    }
  }
}
