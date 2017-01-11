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
  public class TailLogData : INotifyMaster, IDisposable
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public TailLogData ()
    {
      ListOfFilter = new ObservableCollection<FilterData>();
      ListOfFilter.CollectionChanged += ContentCollectionChanged;
    }

    public void Dispose ()
    {
      if (fontType == null)
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
      get
      {
        return (fileName);
      }
      set
      {
        fileName = value;
        File = Path.GetFileName(FileName);
        OnPropertyChanged("FileName");
      }
    }

    #endregion

    #region File

    private string file;

    /// <summary>
    /// Filename without path
    /// </summary>
    public string File
    {
      get
      {
        return (file);
      }
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
      get
      {
        return (wrap);
      }
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
      get
      {
        return (killSpace);
      }
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
      get
      {
        return (refreshRate);
      }
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
      get
      {
        return (timeStamp);
      }
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
      get
      {
        return (fontType);
      }
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
      get
      {
        return (threadPriority);
      }
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
      get
      {
        return (listOfFilter);
      }
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
      get
      {
        return (fileEncoding);
      }
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

    private void ContentCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (FilterData item in e.OldItems)
        {
          item.PropertyChanged -= ItemPropertyChanged;
          OnPropertyChanged("ListOfFilter");
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (FilterData item in e.NewItems)
        {
          item.PropertyChanged += ItemPropertyChanged;
          OnPropertyChanged("ListOfFilter");
        }
      }
    }
  }
}
