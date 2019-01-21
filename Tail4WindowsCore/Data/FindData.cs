using Newtonsoft.Json;
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
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [JsonIgnore]
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

    private bool _searchBookmarkComment;

    /// <summary>
    /// Search bookmark comments
    /// </summary>
    [JsonIgnore]
    public bool SearchBookmarkComments
    {
      get => _searchBookmarkComment;
      set
      {
        if ( value == _searchBookmarkComment )
          return;

        _searchBookmarkComment = value;

        if ( _searchBookmarkComment )
          MarkLineAsBookmark = false;

        OnPropertyChanged();
      }
    }

    private bool _searchBookmarks;

    /// <summary>
    /// Search bookmarks
    /// </summary>
    [JsonIgnore]
    public bool SearchBookmarks
    {
      get => _searchBookmarks;
      set
      {
        if ( value == _searchBookmarks )
          return;

        _searchBookmarks = value;

        if ( _searchBookmarks )
        {
          SearchBookmarkComments = false;
          MarkLineAsBookmark = false;
        }

        OnPropertyChanged();
      }
    }

    private bool _markLineAsBookmark;

    /// <summary>
    /// Mark result as bookmark
    /// </summary>
    [JsonIgnore]
    public bool MarkLineAsBookmark
    {
      get => _markLineAsBookmark;
      set
      {
        if ( value == _markLineAsBookmark )
          return;

        _markLineAsBookmark = value;

        if ( _markLineAsBookmark )
          SearchBookmarkComments = false;

        OnPropertyChanged();
      }
    }

    private bool _useRegex;

    /// <summary>
    /// Use regex
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
