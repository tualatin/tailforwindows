using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text;
using Org.Vs.TailForWin.Data.Base;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// Dataobject for Tabproperties
  /// </summary>
  public class TailLogData : INotifyMaster, IDisposable, ICloneable
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public TailLogData()
    {
      SaveLastDecision = true;
      ListOfFilter = new ObservableCollection<FilterData>();
      ListOfFilter.CollectionChanged += ContentCollectionChanged;
    }

    /// <summary>
    /// Releases all resources used by the TailLogData.
    /// </summary>
    public void Dispose()
    {
      if(fontType == null)
        return;

      fontType.Dispose();
      fontType = null;
    }

    #region FileName

    private string fileName;

    /// <summary>
    /// Filename
    /// </summary>
    public string FileName
    {
      get => (fileName);
      set
      {
        fileName = value;
        File = Path.GetFileName(FileName);
        OnPropertyChanged("FileName");
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

    #endregion

    #region File

    private string file;

    /// <summary>
    /// Filename without path
    /// </summary>
    public string File
    {
      get => (file);
      set
      {
        file = value;
        OnPropertyChanged("File");
      }
    }

    #endregion

    #region Wrap

    private bool wrap;

    /// <summary>
    /// Wrap text in textbox
    /// </summary>
    public bool Wrap
    {
      get => (wrap);
      set
      {
        wrap = value;
        OnPropertyChanged("Wrap");
      }
    }

    #endregion

    #region KillSpace

    private bool killSpace;

    /// <summary>
    /// Remove extra space in each line
    /// </summary>
    public bool KillSpace
    {
      get => (killSpace);
      set
      {
        killSpace = value;
        OnPropertyChanged("KillSpace");
      }
    }

    #endregion

    #region RefreshRate

    private ETailRefreshRate refreshRate;

    /// <summary>
    /// Refreshrate for thread
    /// </summary>
    public ETailRefreshRate RefreshRate
    {
      get => (refreshRate);
      set
      {
        refreshRate = value;
        OnPropertyChanged("RefreshRate");
      }
    }

    #endregion

    #region TimeStamp

    private bool timeStamp;

    /// <summary>
    /// Timestamp in taillog
    /// </summary>
    public bool Timestamp
    {
      get => (timeStamp);
      set
      {
        timeStamp = value;
        OnPropertyChanged("Timestamp");
      }
    }

    #endregion

    #region FontType

    private Font fontType;

    /// <summary>
    /// Font type
    /// </summary>
    public Font FontType
    {
      get => (fontType);
      set
      {
        fontType = value;
        OnPropertyChanged("FontType");
      }
    }

    #endregion

    #region ThreadPriority

    private System.Threading.ThreadPriority threadPriority;

    /// <summary>
    /// ThreadPriority
    /// </summary>
    public System.Threading.ThreadPriority ThreadPriority
    {
      get => (threadPriority);
      set
      {
        threadPriority = value;
        OnPropertyChanged("ThreadPriority");
      }
    }

    #endregion

    /// <summary>
    /// Last refresh time
    /// </summary>
    public DateTime LastRefreshTime
    {
      get;
      set;
    }

    #region ListOfFilter

    private ObservableCollection<FilterData> listOfFilter;

    /// <summary>
    /// List of filters
    /// </summary>
    public ObservableCollection<FilterData> ListOfFilter
    {
      get => (listOfFilter);
      set
      {
        listOfFilter = value;
        OnPropertyChanged("ListOfFilter");
      }
    }

    #endregion

    #region FileEncoding

    private Encoding fileEncoding;

    /// <summary>
    /// File encoding
    /// </summary>
    public Encoding FileEncoding
    {
      get => (fileEncoding);
      set
      {
        fileEncoding = value;
        OnPropertyChanged("FileEncoding");
      }
    }

    #endregion

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

    private string patternString;

    /// <summary>
    /// Current pattern string
    /// </summary>
    public string PatternString
    {
      get => patternString;
      set
      {
        patternString = value;
        OnPropertyChanged("PatternString");
      }
    }

    private bool isRegex;

    /// <summary>
    /// Is regex pattern
    /// </summary>
    public bool IsRegex
    {
      get => isRegex;
      set
      {
        isRegex = value;
        OnPropertyChanged("IsRegex");
      }
    }

    private bool usePattern;

    /// <summary>
    /// Use pattern logic
    /// </summary>
    public bool UsePattern
    {
      get => usePattern;
      set
      {
        usePattern = value;
        OnPropertyChanged("UsePattern");
      }
    }

    #region SmartWatch

    private bool smartWatch;

    /// <summary>
    /// Tail is using SmartWatch logic
    /// </summary>
    public bool SmartWatch
    {
      get => smartWatch;
      set
      {
        smartWatch = value;
        OnPropertyChanged("SmartWatch");
      }
    }

    private bool saveLastDecision;

    /// <summary>
    /// Save last decision
    /// </summary>
    public bool SaveLastDecision
    {
      get => saveLastDecision;
      set
      {
        saveLastDecision = value;
        OnPropertyChanged("SaveLastDecision");
      }
    }

    private bool smartWatchRun;

    /// <summary>
    /// Tail automatically after tab is created
    /// </summary>
    public bool SmartWatchRun
    {
      get => smartWatchRun;
      set
      {
        smartWatchRun = value;
        OnPropertyChanged("SmartWatchRun");
      }
    }

    #endregion

    private void ContentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if(e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach(FilterData item in e.OldItems)
        {
          item.PropertyChanged -= ItemPropertyChanged;
          OnPropertyChanged("ListOfFilter");
        }
      }
      else if(e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach(FilterData item in e.NewItems)
        {
          item.PropertyChanged += ItemPropertyChanged;
          OnPropertyChanged("ListOfFilter");
        }
      }
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
      return (MemberwiseClone());
    }
  }
}
