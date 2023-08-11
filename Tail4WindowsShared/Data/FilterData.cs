using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using Org.Vs.Tail4Win.Shared.Data.Settings;
using Org.Vs.Tail4Win.Shared.Utils.UndoRedoManager;

namespace Org.Vs.Tail4Win.Shared.Data
{
  /// <summary>
  /// Filter data object
  /// </summary>
  public sealed class FilterData : StateManager, IDisposable, IDataErrorInfo, ICloneable
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FilterData()
    {
      Id = Guid.NewGuid();
      FontType = new FontType();
      FindSettingsData = new FindData();
      IsEnabled = true;

      FilterColorHex = DefaultEnvironmentSettings.FilterFontColor;
    }

    /// <summary>
    /// Releases all resources used by <see cref="FilterData"/>.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases all resources used by <see cref="FilterData"/>
    /// </summary>
    /// <param name="disposing">Disposing</param>
    private void Dispose(bool disposing)
    {
      if ( disposing )
        FontType = null;
    }

    private Guid _id;

    /// <summary>
    /// ID filter
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public Guid Id
    {
      get => _id;
      set
      {
        _id = value;
        OnPropertyChanged();
      }
    }

    private bool _isGlobal;

    /// <summary>
    /// Is global filter
    /// </summary>
    public bool IsGlobal
    {
      get => _isGlobal;
      set
      {
        if ( value == _isGlobal )
          return;

        _isGlobal = value;
        OnPropertyChanged();
      }
    }

    private bool _isEnabled;

    /// <summary>
    /// Is enabled
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool IsEnabled
    {
      get => _isEnabled;
      set
      {
        if ( value == _isEnabled )
          return;

        bool currentValue = _isEnabled;
        ChangeState(new Command(() => _isEnabled = value, () => _isEnabled = currentValue, nameof(IsEnabled), Notification));
      }
    }

    private bool _isHighlight;

    /// <summary>
    /// Is Highlight
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool IsHighlight
    {
      get => _isHighlight;
      set
      {
        if ( value == _isHighlight )
          return;

        bool currentValue = _isHighlight;
        ChangeState(new Command(() => _isHighlight = value, () => _isHighlight = currentValue, nameof(IsHighlight), Notification));
      }
    }

    private bool _filterSource;

    /// <summary>
    /// Filter source
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool FilterSource
    {
      get => _filterSource;
      set
      {
        if ( value == _filterSource )
          return;

        bool currentValue = _filterSource;
        ChangeState(new Command(() => _filterSource = value, () => _filterSource = currentValue, nameof(FilterSource), Notification));
      }
    }

    private bool _useNotification;

    /// <summary>
    /// Popup a notification information
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool UseNotification
    {
      get => _useNotification;
      set
      {
        if ( value == _useNotification )
          return;

        bool currentValue = _useNotification;
        ChangeState(new Command(() => _useNotification = value, () => _useNotification = currentValue, nameof(UseNotification), Notification));
      }
    }

    private string _description;

    /// <summary>
    /// Filter description
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Description
    {
      get => _description;
      set
      {
        if ( Equals(value, _description) )
          return;

        string currentValue = _description;
        ChangeState(new Command(() => _description = value, () => _description = currentValue, nameof(Description), Notification));
      }
    }

    private string _filter;

    /// <summary>
    /// Filter
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Filter
    {
      get => _filter;
      set
      {
        if ( Equals(value, _filter) )
          return;

        string currentValue = _filter;
        ChangeState(new Command(() => _filter = value, () => _filter = currentValue, nameof(Filter), Notification));
      }
    }

    private string _filterColorHex;

    /// <summary>
    /// Font foreground color
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "FilterColor")]
    public string FilterColorHex
    {
      get => _filterColorHex;
      set
      {
        if ( Equals(value, _filterColorHex) )
          return;

        string currentValue = _filterColorHex;
        ChangeState(new Command(() => _filterColorHex = value, () => _filterColorHex = currentValue, nameof(FilterColorHex), Notification));
      }
    }

    private FontType _fontType;

    /// <summary>
    /// <see cref="Data.FontType"/>
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "FontStyle")]
    public FontType FontType
    {
      get => _fontType;
      set
      {
        if ( Equals(value, _fontType) )
          return;

        FontType currentValue = _fontType;
        ChangeState(new Command(() => _fontType = value, () => _fontType = currentValue, nameof(FontType), Notification));
      }
    }

    private FindData _findSettingsData;

    /// <summary>
    /// FindSettings data
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "FindSettings")]
    public FindData FindSettingsData
    {
      get => _findSettingsData;
      set
      {
        if ( Equals(value, _findSettingsData) )
          return;

        FindData currentValue = _findSettingsData;
        ChangeState(new Command(() => _findSettingsData = value, () => _findSettingsData = currentValue, nameof(FindSettingsData), Notification));
      }
    }

    private bool _isAutoBookmark;

    /// <summary>
    /// IsAutoBookmark
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool IsAutoBookmark
    {
      get => _isAutoBookmark;
      set
      {
        if ( value == _isAutoBookmark )
          return;

        bool currentValue = _isAutoBookmark;
        ChangeState(new Command(() => _isAutoBookmark = value, () => _isAutoBookmark = currentValue, nameof(IsAutoBookmark), Notification));
      }
    }

    private string _autoBookmarkComment;

    /// <summary>
    /// AutoBookmark comment
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "BookmarkDescription")]
    public string AutoBookmarkComment
    {
      get => _autoBookmarkComment;
      set
      {
        if ( Equals(value, _autoBookmarkComment) )
          return;

        string currentValue = _autoBookmarkComment;
        ChangeState(new Command(() => _autoBookmarkComment = value, () => _autoBookmarkComment = currentValue, nameof(AutoBookmarkComment), Notification));
      }
    }

    #region IDataErrorInfo interface

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    [JsonIgnore]
    public string Error => throw new NotImplementedException();

    /// <summary>
    /// Gets the error message for the property with the given name.
    /// </summary>
    /// <param name="columnName">Name of column</param>
    /// <returns>Current error result</returns>
    [JsonIgnore]
    public string this[string columnName]
    {
      get
      {
        string result = null;

        switch ( columnName )
        {
        case nameof(Description):

          if ( string.IsNullOrEmpty(Description) )
            result = Application.Current.TryFindResource("ErrorEnterDescription").ToString();
          break;

        case nameof(Filter):

          if ( string.IsNullOrEmpty(Filter) )
            result = Application.Current.TryFindResource("ErrorEnterFilterPattern").ToString();
          break;

          //case nameof(FilterSource):

          //  if ( !FilterSource && !IsHighlight )
          //    result = Application.Current.TryFindResource("ErrorEnterHighlightFilterSource").ToString();
          //  break;

          //case nameof(IsHighlight):

          //  if ( !FilterSource && !IsHighlight )
          //    result = Application.Current.TryFindResource("ErrorEnterHighlightFilterSource").ToString();
          //  break;
        }
        return result;
      }
    }

    #endregion

    /// <summary>
    /// Creates a shallow copy of the current Object.
    /// </summary>
    /// <returns>A shallow copy of the current Object.</returns>
    public object Clone() => MemberwiseClone();

    /// <summary>
    /// Marks filter as global
    /// </summary>
    public void ConvertToGlobal() => _isGlobal = true;
  }
}
