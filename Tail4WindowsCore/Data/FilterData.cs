using System;
using System.ComponentModel;
using System.Windows;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Filter data object
  /// </summary>
  public class FilterData : StateManager, IDisposable, IDataErrorInfo, ICloneable
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
    /// Releases all resources used by the FilterData.
    /// </summary>
    public void Dispose() => FontType = null;

    private Guid _id;

    /// <summary>
    /// ID filter
    /// </summary>
    public Guid Id
    {
      get => _id;
      set
      {
        _id = value;
        OnPropertyChanged(nameof(Id));
      }
    }

    private bool _isEnabled;

    /// <summary>
    /// Is enabled
    /// </summary>
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

    private bool _isHightlight;

    /// <summary>
    /// Is Hightlight
    /// </summary>
    public bool IsHighlight
    {
      get => _isHightlight;
      set
      {
        if ( value == _isHightlight )
          return;

        bool currentValue = _isHightlight;
        ChangeState(new Command(() => _isHightlight = value, () => _isHightlight = currentValue, nameof(IsHighlight), Notification));
      }
    }

    private bool _filterSource;

    /// <summary>
    /// Filter source
    /// </summary>
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
    public FontType FontType
    {
      get => _fontType;
      set
      {
        if ( Equals(value, _fontType) )
          return;

        var currentValue = _fontType;
        ChangeState(new Command(() => _fontType = value, () => _fontType = currentValue, nameof(FontType), Notification));
      }
    }

    private FindData _findSettingsData;

    /// <summary>
    /// FindSettings data
    /// </summary>
    public FindData FindSettingsData
    {
      get => _findSettingsData;
      set
      {
        if ( Equals(value, _findSettingsData) )
          return;

        var currentValue = _findSettingsData;
        ChangeState(new Command(() => _findSettingsData = value, () => _findSettingsData = currentValue, nameof(FindSettingsData), Notification));
      }
    }

    #region IDataErrorInfo interface

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    public string Error => throw new NotImplementedException();

    /// <summary>
    /// Gets the error message for the property with the given name.
    /// </summary>
    /// <param name="columnName">Name of column</param>
    /// <returns>Current error result</returns>
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

        case nameof(FilterSource):

          if ( !FilterSource && !IsHighlight )
            result = Application.Current.TryFindResource("ErrorEnterHighlightFilterSource").ToString();
          break;

        case nameof(IsHighlight):

          if ( !FilterSource && !IsHighlight )
            result = Application.Current.TryFindResource("ErrorEnterHighlightFilterSource").ToString();
          break;
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
  }
}
