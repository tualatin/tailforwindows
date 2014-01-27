using System.IO;
using System;
using System.Drawing;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace TailForWin.Data
{
  /// <summary>
  /// Dataobject for Tabproperties
  /// </summary>
  public class TailLogData: INotifyMaster, IDisposable
  {
    private Font fontType;
    private bool killSpace;
    private bool wrap;
    private bool timeStamp;
    private string fileName;
    private string file;


    public TailLogData ()
    {
      ListOfFilter = new ObservableCollection<FilterData> ( );
      ListOfFilter.CollectionChanged += ContentCollectionChanged;
    }

    public void Dispose ()
    {
      if (fontType != null)
      {
        fontType.Dispose ( );
        fontType = null;
      }
    }

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
        File = Path.GetFileName (FileName);
        OnPropertyChanged ("FileName");
      }
    }

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
        OnPropertyChanged ("File");
      }
    }

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
        OnPropertyChanged ("Wrap");
      }
    }

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
        OnPropertyChanged ("KillSpace");
      }
    }

    /// <summary>
    /// Refreshrate for thread
    /// </summary>
    public SettingsData.ETailRefreshRate RefreshRate
    {
      get;
      set;
    }

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
        OnPropertyChanged ("Timestamp");
      }
    }

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
        OnPropertyChanged ("FontType");
      }
    }

    /// <summary>
    /// ThreadPriority
    /// </summary>
    public System.Threading.ThreadPriority ThreadPriority
    {
      get;
      set;
    }

    /// <summary>
    /// Last refresh time
    /// </summary>
    public DateTime LastRefreshTime
    {
      get;
      set;
    }

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
        OnPropertyChanged ("ListOfFilter");
      }
    }

    /// <summary>
    /// File encoding
    /// </summary>
    public Encoding FileEncoding
    {
      get;
      set;
    }

    /// <summary>
    /// Is item opened from FileManager
    /// </summary>
    public bool OpenFromFileManager
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
          OnPropertyChanged ("ListOfFilter");
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (FilterData item in e.NewItems)
        {
          item.PropertyChanged += ItemPropertyChanged;
          OnPropertyChanged ("ListOfFilter");
        }
      }
    }
  }
}
