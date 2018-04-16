using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using log4net;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Tail data object
  /// </summary>
  public class TailData : StateManager, ICloneable, IDisposable, IDataErrorInfo, IComparer
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TailData));


    /// <summary>
    /// Standard constructor
    /// </summary>
    public TailData()
    {
      Id = Guid.NewGuid();
      FontType = new Font("Segoe UI", 11f, FontStyle.Regular);
      AutoRun = true;
      TabItemBackgroundColorStringHex = DefaultEnvironmentSettings.TabItemHeaderBackgroundColor;
      RefreshRate = SettingsHelperController.CurrentSettings.DefaultRefreshRate;
      ThreadPriority = SettingsHelperController.CurrentSettings.DefaultThreadPriority;

      ListOfFilter = new ObservableCollection<FilterData>();
      ListOfFilter.CollectionChanged += ListOfFilterCollectionChanged;
    }

    private void ListOfFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      switch ( e.Action )
      {
      case NotifyCollectionChangedAction.Remove:

        foreach ( FilterData item in e.OldItems )
        {
          item.PropertyChanged -= ItemPropertyChanged;
          OnPropertyChanged(nameof(ListOfFilter));
        }
        break;

      case NotifyCollectionChangedAction.Add:

        foreach ( FilterData item in e.NewItems )
        {
          item.PropertyChanged += ItemPropertyChanged;
          OnPropertyChanged(nameof(ListOfFilter));
        }
        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }

    private decimal _version;

    /// <summary>
    /// Current XML version
    /// </summary>
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

    private string _fileName;

    /// <summary>
    /// Filename
    /// </summary>
    public string FileName
    {
      get => _fileName;
      set
      {
        string currentValue = _fileName;

        ChangeState(new Command(() => _fileName = value, () => FileName = currentValue));
        OnPropertyChanged(nameof(FileName));

        File = Path.GetFileName(FileName);
      }
    }

    /// <summary>
    /// Original filename from XML file or Tail4Windows window
    /// </summary>
    public string OriginalFileName
    {
      get;
      set;
    }

    private string _file;

    /// <summary>
    /// Filename without path
    /// </summary>
    public string File
    {
      get => _file;
      private set
      {
        _file = value;
        OnPropertyChanged(nameof(File));
      }
    }

    private string _description;

    /// <summary>
    /// Description of item
    /// </summary>
    public string Description
    {
      get => _description;
      set
      {
        string currentValue = _description;

        ChangeState(new Command(() => _description = value, () => Description = currentValue));
        OnPropertyChanged(nameof(Description));
      }
    }

    private string _category;

    /// <summary>
    /// Category of item
    /// </summary>
    public string Category
    {
      get => _category;
      set
      {
        string currentValue = _category;

        ChangeState(new Command(() => _category = value, () => Category = currentValue));
        OnPropertyChanged(nameof(Category));
      }
    }

    private bool _newWindow;

    /// <summary>
    /// Open thread in new window
    /// </summary>
    public bool NewWindow
    {
      get => _newWindow;
      set
      {
        bool currentValue = _newWindow;

        ChangeState(new Command(() => _newWindow = value, () => NewWindow = currentValue));
        OnPropertyChanged(nameof(NewWindow));
      }
    }

    /// <summary>
    /// File creation time
    /// </summary>
    public DateTime? FileCreationTime
    {
      get
      {
        if ( System.IO.File.Exists(FileName) )
          return System.IO.File.GetCreationTime(FileName);

        return null;
      }
    }

    /// <summary>
    /// FileAge
    /// </summary>
    public TimeSpan? FileAge
    {
      get
      {
        DateTime now = DateTime.Now;

        try
        {
          if ( FileCreationTime != null )
            return now.Subtract((DateTime) FileCreationTime);

          return null;
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
    public bool Wrap
    {
      get => _wrap;
      set
      {
        bool currentValue = _wrap;

        ChangeState(new Command(() => _wrap = value, () => Wrap = currentValue));
        OnPropertyChanged(nameof(Wrap));
      }
    }

    private bool _removeSpace;

    /// <summary>
    /// Remove extra space in each line
    /// </summary>
    public bool RemoveSpace
    {
      get => _removeSpace;
      set
      {
        bool currentValue = _removeSpace;

        ChangeState(new Command(() => _removeSpace = value, () => RemoveSpace = currentValue));
        OnPropertyChanged(nameof(RemoveSpace));
      }
    }

    private ETailRefreshRate _refreshRate;

    /// <summary>
    /// Refreshrate for thread
    /// </summary>
    public ETailRefreshRate RefreshRate
    {
      get => _refreshRate;
      set
      {
        var currentValue = _refreshRate;

        ChangeState(new Command(() => _refreshRate = value, () => RefreshRate = currentValue));
        OnPropertyChanged(nameof(RefreshRate));
      }
    }

    private bool _timeStamp;

    /// <summary>
    /// Timestamp in taillog
    /// </summary>
    public bool Timestamp
    {
      get => _timeStamp;
      set
      {
        bool currentValue = _timeStamp;

        ChangeState(new Command(() => _timeStamp = value, () => Timestamp = currentValue));
        OnPropertyChanged(nameof(Timestamp));
      }
    }

    private Font _fontType;

    /// <summary>
    /// Font type
    /// </summary>
    public Font FontType
    {
      get => _fontType;
      set
      {
        var currentValue = _fontType;

        ChangeState(new Command(() => _fontType = value, () => FontType = currentValue));
        OnPropertyChanged(nameof(FontType));
      }
    }

    private System.Threading.ThreadPriority _threadPriority;

    /// <summary>
    /// ThreadPriority
    /// </summary>
    public System.Threading.ThreadPriority ThreadPriority
    {
      get => _threadPriority;
      set
      {
        var currentValue = _threadPriority;

        ChangeState(new Command(() => _threadPriority = value, () => ThreadPriority = currentValue));
        OnPropertyChanged(nameof(ThreadPriority));
      }
    }

    private DateTime _lastRefreshTime;

    /// <summary>
    /// Last refresh time
    /// </summary>
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
    public Encoding FileEncoding
    {
      get => _fileEncoding;
      set
      {
        var currentValue = _fileEncoding;

        ChangeState(new Command(() => _fileEncoding = value, () => FileEncoding = currentValue));
        OnPropertyChanged(nameof(FileEncoding));
      }
    }

    private bool _openFromFileManager;

    /// <summary>
    /// Is item opened from FileManager
    /// </summary>
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
    public bool FilterState
    {
      get => _filterState;
      set
      {
        bool currentValue = _filterState;

        ChangeState(new Command(() => _filterState = value, () => FilterState = currentValue));
        OnPropertyChanged();
      }
    }

    private string _patternString;

    /// <summary>
    /// Current pattern string
    /// </summary>
    public string PatternString
    {
      get => _patternString;
      set
      {
        string currentValue = _patternString;

        ChangeState(new Command(() => _patternString = value, () => PatternString = currentValue));
        OnPropertyChanged(nameof(PatternString));
      }
    }

    private bool _isRegex;

    /// <summary>
    /// Is regex pattern
    /// </summary>
    public bool IsRegex
    {
      get => _isRegex;
      set
      {
        bool currentValue = _isRegex;

        ChangeState(new Command(() => _isRegex = value, () => IsRegex = currentValue));
        OnPropertyChanged(nameof(IsRegex));
      }
    }

    private bool _usePattern;

    /// <summary>
    /// Use pattern logic
    /// </summary>
    public bool UsePattern
    {
      get => _usePattern;
      set
      {
        bool currentValue = _usePattern;

        ChangeState(new Command(() => _usePattern = value, () => UsePattern = currentValue));
        OnPropertyChanged(nameof(UsePattern));
      }
    }

    private bool _smartWatch;

    /// <summary>
    /// Tail is using SmartWatch logic
    /// </summary>
    public bool SmartWatch
    {
      get => _smartWatch;
      set
      {
        bool currentValue = _smartWatch;

        ChangeState(new Command(() => _smartWatch = value, () => SmartWatch = currentValue));
        OnPropertyChanged(nameof(SmartWatch));
      }
    }

    private bool _autoRun;

    /// <summary>
    /// Tail automatically after tab is created
    /// </summary>
    public bool AutoRun
    {
      get => _autoRun;
      set
      {
        bool currentValue = _autoRun;

        ChangeState(new Command(() => _autoRun = value, () => AutoRun = currentValue));
        OnPropertyChanged(nameof(AutoRun));
      }
    }

    private string _tabItemBackgroundColorStringHex;

    /// <summary>
    /// TabItem background color as hex string
    /// </summary>
    public string TabItemBackgroundColorStringHex
    {
      get => _tabItemBackgroundColorStringHex;
      set
      {
        string currentValue = _tabItemBackgroundColorStringHex;

        ChangeState(new Command(() => _tabItemBackgroundColorStringHex = value, () => TabItemBackgroundColorStringHex = currentValue));
        OnPropertyChanged(nameof(TabItemBackgroundColorStringHex));
      }
    }

    private bool _openFromSmartWatch;

    /// <summary>
    /// Properties comes from SmartWatch
    /// </summary>
    public bool OpenFromSmartWatch
    {
      get => _openFromSmartWatch;
      set
      {
        _openFromSmartWatch = value;
        OnPropertyChanged();
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
    public void Dispose()
    {
      if ( FontType == null )
        return;

      FontType.Dispose();
      FontType = null;
    }

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

        if ( columnName == nameof(Description) )
        {
          if ( string.IsNullOrEmpty(Description) )
            result = "Please enter a Description";
        }

        if ( columnName == nameof(FileName) )
        {
          if ( string.IsNullOrWhiteSpace(FileName) )
            result = "Please enter a FileName";
        }
        return result;
      }
    }

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    public string Error => throw new NotImplementedException();

    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x">FileManagerData x</param>
    /// <param name="y">FileManagerData y</param>
    /// <returns>Compareable result</returns>
    public int Compare(object x, object y)
    {
      if ( !(x is TailData) || !(y is TailData) )
        return 1;

      var xFm = (TailData) x;
      var yFm = (TailData) y;

      var nx = xFm.FileCreationTime ?? DateTime.MaxValue;
      var ny = yFm.FileCreationTime ?? DateTime.MaxValue;

      return -nx.CompareTo(ny);
    }
  }
}
