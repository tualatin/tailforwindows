using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.JsonConverters;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Tail data object
  /// </summary>
  public class TailData : StateManager, ICloneable, IDisposable, IDataErrorInfo
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TailData));


    /// <summary>
    /// Standard constructor
    /// </summary>
    public TailData()
    {
      Id = Guid.NewGuid();

      FontType = new FontType();
      FindSettings = new FindData
      {
        WholeWord = true
      };
      WindowsEvent = new WindowsEventData();
      WindowsEvent.PropertyChanged += OnWindowsEventPropertyChanged;

      AutoRun = true;
      TabItemBackgroundColorStringHex = DefaultEnvironmentSettings.TabItemHeaderBackgroundColor;
      RefreshRate = SettingsHelperController.CurrentSettings.DefaultRefreshRate;
      ThreadPriority = SettingsHelperController.CurrentSettings.DefaultThreadPriority;

      ListOfFilter = new ObservableCollection<FilterData>();
      ListOfFilter.CollectionChanged += ListOfFilterCollectionChanged;
    }

    private void OnWindowsEventPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !Equals(e.PropertyName, "Name") )
        return;

      OnPropertyChanged(nameof(File));
    }

    private void ListOfFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch ( e.Action )
      {
      case NotifyCollectionChangedAction.Remove:

        foreach ( FilterData item in e.OldItems )
        {
          item.PropertyChanged -= OnFilterItemPropertyChanged;
          OnPropertyChanged(nameof(ListOfFilter));
        }
        break;

      case NotifyCollectionChangedAction.Add:

        foreach ( FilterData item in e.NewItems )
        {
          item.PropertyChanged -= OnFilterItemPropertyChanged;
          item.PropertyChanged += OnFilterItemPropertyChanged;

          OnPropertyChanged(nameof(ListOfFilter));
        }
        break;

      case NotifyCollectionChangedAction.Replace:

        break;

      case NotifyCollectionChangedAction.Move:

        break;

      case NotifyCollectionChangedAction.Reset:

        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }

    private void OnFilterItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName == "CanUndo" || e.PropertyName == "CanRedo" )
        return;

      // ReSharper disable once ExplicitCallerInfoArgument
      OnPropertyChanged("FilterItem");
    }

    private decimal _version;

    /// <summary>
    /// Current XML version
    /// </summary>
    [JsonIgnore]
    public decimal Version
    {
      get => _version;
      set
      {
        _version = value;
        OnPropertyChanged(nameof(Version));
      }
    }

    private Guid _id;

    /// <summary>
    /// Unique ID of FileManager node
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public Guid Id
    {
      get => _id;
      set
      {
        _id = value;
        OnPropertyChanged(nameof(Id));
      }
    }

    private bool _isLoadedByXml;

    /// <summary>
    /// This data comes from saved XML file
    /// </summary>
    [JsonIgnore]
    public bool IsLoadedByXml
    {
      get => _isLoadedByXml;
      set
      {
        if ( value == _isLoadedByXml )
          return;

        _isLoadedByXml = value;
        OnPropertyChanged();
      }
    }

    private bool _isWindowsEvent;

    /// <summary>
    /// Is Windows event
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool IsWindowsEvent
    {
      get => _isWindowsEvent;
      set
      {
        if ( value == _isWindowsEvent )
          return;

        bool currentValue = _isWindowsEvent;

        if ( value )
          ResetData();

        ChangeState(new Command(() => _isWindowsEvent = value, () => _isWindowsEvent = currentValue, nameof(IsWindowsEvent), Notification));
      }
    }

    private WindowsEventData _windowsEvent;

    /// <summary>
    /// Windows event data
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public WindowsEventData WindowsEvent
    {
      get => _windowsEvent;
      set
      {
        if ( Equals(value, _windowsEvent) )
          return;

        bool currentValue = _isWindowsEvent;
        ChangeState(new Command(() => _windowsEvent = value, () => _isWindowsEvent = currentValue, nameof(WindowsEvent), Notification));
      }
    }

    private string _fileName;

    /// <summary>
    /// Filename
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string FileName
    {
      get => _fileName;
      set
      {
        if ( Equals(value, _fileName) )
          return;

        string currentValue = _fileName;
        ChangeState(new Command(() => _fileName = value, () => _fileName = currentValue, nameof(FileName), FileNameChangedNotification));
      }
    }

    /// <summary>
    /// To set the file name without path, we need a special notification handler
    /// </summary>
    /// <param name="propertyName">Name of property</param>
    private void FileNameChangedNotification(string propertyName)
    {
      OnPropertyChanged(nameof(File));
      OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Original filename from XML file or Tail4Windows window
    /// </summary>
    [JsonIgnore]
    public string OriginalFileName
    {
      get;
      set;
    }

    /// <summary>
    /// Filename without path
    /// </summary>
    [JsonIgnore]
    public string File
    {
      get
      {
        if ( !IsWindowsEvent )
          return Path.GetFileName(_fileName);

        string text = WindowsEvent.Machine == "." ? Environment.MachineName : WindowsEvent.Machine;
        return $"{text}: {WindowsEvent.Name}";
      }
    }

    private string _description;

    /// <summary>
    /// Description of item
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Description
    {
      get => _description;
      set
      {
        if ( value == _description )
          return;

        string currentValue = _description;
        ChangeState(new Command(() => _description = value, () => _description = currentValue, nameof(Description), Notification));
      }
    }

    private string _category;

    /// <summary>
    /// Category of item
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Category
    {
      get => _category;
      set
      {
        if ( value == _category )
          return;

        string currentValue = _category;
        ChangeState(new Command(() => _category = value, () => _category = currentValue, nameof(Category), Notification));
      }
    }

    private bool _newWindow;

    /// <summary>
    /// Open thread in new window
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "OpenInNewWindow")]
    public bool NewWindow
    {
      get => _newWindow;
      set
      {
        if ( value == _newWindow )
          return;

        bool currentValue = _newWindow;
        ChangeState(new Command(() => _newWindow = value, () => _newWindow = currentValue, nameof(NewWindow), Notification));
      }
    }

    /// <summary>
    /// File creation time
    /// </summary>
    [JsonIgnore]
    public DateTime? FileCreationTime => System.IO.File.Exists(FileName) ? (DateTime?) System.IO.File.GetCreationTime(FileName) : null;

    /// <summary>
    /// FileAge
    /// </summary>
    [JsonIgnore]
    public TimeSpan? FileAge
    {
      get
      {
        DateTime now = DateTime.Now;

        try
        {
          return FileCreationTime != null ? (TimeSpan?) now.Subtract((DateTime) FileCreationTime) : null;
        }
        catch ( ArgumentOutOfRangeException ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          throw;
        }
      }
    }

    private bool _wrap;

    /// <summary>
    /// Wrap text in textbox
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Wrap
    {
      get => _wrap;
      set
      {
        if ( value == _wrap )
          return;

        bool currentValue = _wrap;
        ChangeState(new Command(() => _wrap = value, () => _wrap = currentValue, nameof(Wrap), Notification));
      }
    }

    private bool _removeSpace;

    /// <summary>
    /// Removes extra space in each line
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool RemoveSpace
    {
      get => _removeSpace;
      set
      {
        if ( value == _removeSpace )
          return;

        bool currentValue = _removeSpace;
        ChangeState(new Command(() => _removeSpace = value, () => _removeSpace = currentValue, nameof(RemoveSpace), Notification));
      }
    }

    private bool _showTimeDelta;

    /// <summary>
    /// Shows time delta
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ShowTimeDelta
    {
      get => _showTimeDelta;
      set
      {
        if ( value == _showTimeDelta )
          return;

        bool currentValue = _showTimeDelta;
        ChangeState(new Command(() => _showTimeDelta = value, () => _showTimeDelta = currentValue, nameof(ShowTimeDelta), Notification));
      }
    }

    private ETailRefreshRate _refreshRate;

    /// <summary>
    /// RefreshRate for thread
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    [JsonConverter(typeof(StringEnumConverter))]
    public ETailRefreshRate RefreshRate
    {
      get => _refreshRate;
      set
      {
        if ( value == _refreshRate )
          return;

        var currentValue = _refreshRate;
        ChangeState(new Command(() => _refreshRate = value, () => _refreshRate = currentValue, nameof(RefreshRate), Notification));
      }
    }

    private bool _timeStamp;

    /// <summary>
    /// Timestamp in taillog
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "UseTimestamp")]
    public bool Timestamp
    {
      get => _timeStamp;
      set
      {
        if ( value == _timeStamp )
          return;

        bool currentValue = _timeStamp;
        ChangeState(new Command(() => _timeStamp = value, () => _timeStamp = currentValue, nameof(Timestamp), Notification));
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

        var currentValue = _fontType;
        ChangeState(new Command(() => _fontType = value, () => _fontType = currentValue, nameof(FontType), Notification));
      }
    }

    private System.Threading.ThreadPriority _threadPriority;

    /// <summary>
    /// ThreadPriority
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    [JsonConverter(typeof(StringEnumConverter))]
    public System.Threading.ThreadPriority ThreadPriority
    {
      get => _threadPriority;
      set
      {
        if ( value == _threadPriority )
          return;

        var currentValue = _threadPriority;
        ChangeState(new Command(() => _threadPriority = value, () => _threadPriority = currentValue, nameof(ThreadPriority), Notification));
      }
    }

    private DateTime _lastRefreshTime;

    /// <summary>
    /// Last refresh time
    /// </summary>
    [JsonIgnore]
    public DateTime LastRefreshTime
    {
      get => _lastRefreshTime;
      set
      {
        _lastRefreshTime = value;
        OnPropertyChanged(nameof(LastRefreshTime));
      }
    }

    private ObservableCollection<FilterData> _listOfFilter;

    /// <summary>
    /// List of filters
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "Filters")]
    public ObservableCollection<FilterData> ListOfFilter
    {
      get => _listOfFilter;
      set
      {
        _listOfFilter = value;
        OnPropertyChanged(nameof(ListOfFilter));
      }
    }

    private Encoding _fileEncoding;

    /// <summary>
    /// File encoding
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    [JsonConverter(typeof(EncodingJsonConverter))]
    public Encoding FileEncoding
    {
      get => _fileEncoding;
      set
      {
        if ( Equals(value, _fileEncoding) )
          return;

        var currentValue = _fileEncoding;
        ChangeState(new Command(() => _fileEncoding = value, () => _fileEncoding = currentValue, nameof(FileEncoding), Notification));
      }
    }

    private bool _openFromSmartWatch;

    /// <summary>
    /// Item opened by SmartWatch logic
    /// </summary>
    [JsonIgnore]
    public bool OpenFromSmartWatch
    {
      get => _openFromSmartWatch;
      set
      {
        if ( value == _openFromSmartWatch )
          return;

        _openFromSmartWatch = value;
        OnPropertyChanged();
      }
    }

    private bool _openFromFileManager;

    /// <summary>
    /// Is item opened from FileManager
    /// </summary>
    [JsonIgnore]
    public bool OpenFromFileManager
    {
      get => _openFromFileManager;
      set
      {
        if ( value == _openFromFileManager )
          return;

        _openFromFileManager = value;
        OnPropertyChanged();
      }
    }

    private bool _filterState;

    /// <summary>
    /// Is filter checkbox on/off
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "UseFilters")]
    public bool FilterState
    {
      get => _filterState;
      set
      {
        if ( value == _filterState )
          return;

        bool currentValue = _filterState;
        ChangeState(new Command(() => _filterState = value, () => _filterState = currentValue, nameof(FilterState), Notification));
      }
    }

    private string _patternString;

    /// <summary>
    /// Current pattern string
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "Pattern")]
    public string PatternString
    {
      get => _patternString;
      set
      {
        if ( Equals(value, _patternString) )
          return;

        string currentValue = _patternString;
        ChangeState(new Command(() => _patternString = value, () => _patternString = currentValue, nameof(PatternString), Notification));
      }
    }

    private FindData _findSettings;

    /// <summary>
    /// FindSettings
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public FindData FindSettings
    {
      get => _findSettings;
      set
      {
        if ( Equals(value, _findSettings) )
          return;

        var currentValue = _findSettings;
        ChangeState(new Command(() => _findSettings = value, () => _findSettings = currentValue, nameof(FindSettings), Notification));
      }
    }

    private bool _usePattern;

    /// <summary>
    /// Use pattern logic
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool UsePattern
    {
      get => _usePattern;
      set
      {
        if ( value == _usePattern )
          return;

        bool currentValue = _usePattern;
        ChangeState(new Command(() => _usePattern = value, () => _usePattern = currentValue, nameof(UsePattern), Notification));
      }
    }

    private bool _smartWatch;

    /// <summary>
    /// Tail is using SmartWatch logic
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "UseSmartWatch")]
    public bool SmartWatch
    {
      get => _smartWatch;
      set
      {
        if ( value == _smartWatch )
          return;

        bool currentValue = _smartWatch;
        ChangeState(new Command(() => _smartWatch = value, () => _smartWatch = currentValue, nameof(SmartWatch), Notification));
      }
    }

    private bool _autoRun;

    /// <summary>
    /// Tail automatically after tab is created
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool AutoRun
    {
      get => _autoRun;
      set
      {
        if ( value == _autoRun )
          return;

        bool currentValue = _autoRun;
        ChangeState(new Command(() => _autoRun = value, () => _autoRun = currentValue, nameof(AutoRun), Notification));
      }
    }

    private string _tabItemBackgroundColorStringHex;

    /// <summary>
    /// TabItem background color as hex string
    /// </summary>
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "TabHeaderColor")]
    public string TabItemBackgroundColorStringHex
    {
      get => _tabItemBackgroundColorStringHex;
      set
      {
        if ( Equals(value, _tabItemBackgroundColorStringHex) )
          return;

        string currentValue = _tabItemBackgroundColorStringHex;
        ChangeState(new Command(() => _tabItemBackgroundColorStringHex = value, () => _tabItemBackgroundColorStringHex = currentValue, nameof(TabItemBackgroundColorStringHex), Notification));
      }
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone() => MemberwiseClone();

    /// <summary>
    /// Releases all resources used by the TailData.
    /// </summary>
    public void Dispose() => FontType = null;

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

          if ( string.IsNullOrWhiteSpace(Description) )
            result = Application.Current.TryFindResource("ErrorEnterDescription").ToString();
          break;

        case nameof(FileName):

          if ( string.IsNullOrWhiteSpace(FileName) && !IsWindowsEvent )
            result = Application.Current.TryFindResource("ErrorEnterFileName").ToString();
          break;
        }

        return result;
      }
    }

    private void ResetData()
    {
      SmartWatch = false;
      FileName = string.Empty;
      PatternString = string.Empty;
      UsePattern = false;
      Timestamp = true;
      Wrap = true;
    }

    /// <summary>
    /// Set File
    /// </summary>
    public void SetFile() => OnPropertyChanged(nameof(File));

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    [JsonIgnore]
    public string Error => throw new NotImplementedException();
  }
}
