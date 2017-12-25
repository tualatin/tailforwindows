using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using log4net;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Tail data object
  /// </summary>
  public partial class TailData : NotifyMaster, ICloneable, IDisposable, IDataErrorInfo
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TailData));


    /// <summary>
    /// Standard constructor
    /// </summary>
    public TailData()
    {
      AutoRun = true;
      ListOfFilter = new ObservableCollection<FilterData>();
      ListOfFilter.CollectionChanged += ListOfFilter_CollectionChanged;
    }

    private void ListOfFilter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

    private string _fileName;

    /// <summary>
    /// Filename
    /// </summary>
    public string FileName
    {
      get => _fileName;
      set
      {
        _fileName = value;
        File = Path.GetFileName(FileName);
        OnPropertyChanged(nameof(FileName));
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
      get => _description?.Trim();
      set
      {
        _description = value;
        OnPropertyChanged(nameof(Description));
      }
    }

    private string _category;

    /// <summary>
    /// Category of item
    /// </summary>
    public string Category
    {
      get => _category?.Trim();
      set
      {
        _category = value;
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
        _newWindow = value;
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
        _wrap = value;
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
        _removeSpace = value;
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
        _refreshRate = value;
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
        _timeStamp = value;
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
        _fontType = value;
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
        _threadPriority = value;
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
        _fileEncoding = value;
        OnPropertyChanged(nameof(FileEncoding));
      }
    }

    /// <summary>
    /// Is item opened from FileManager
    /// </summary>
    public bool OpenFromFileManager
    {
      get;
      set;
    }

    /// <summary>
    /// Is filter checkbox on/off
    /// </summary>
    public bool FilterState
    {
      get;
      set;
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
        _patternString = value;
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
        _isRegex = value;
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
        _usePattern = value;
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
        _smartWatch = value;
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
        _autoRun = value;
        OnPropertyChanged(nameof(AutoRun));
      }
    }

    /// <summary>
    /// Properties comes from SmartWatch
    /// </summary>
    public bool OpenFromSmartWatch
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
      return MemberwiseClone();
    }

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
        return result;
      }
    }

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    public string Error => throw new NotImplementedException();
  }
}
