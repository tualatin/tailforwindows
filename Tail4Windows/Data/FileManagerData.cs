using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Extensions;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// Dataobject for FileManagerProperties
  /// </summary>
  public class FileManagerData : TailLogData, IComparer, IDataErrorInfo
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FileManagerData()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tailData">Tail log data</param>
    public FileManagerData(TailLogData tailData)
    {
      Wrap = tailData.Wrap;
      KillSpace = tailData.KillSpace;
      ThreadPriority = tailData.ThreadPriority;
      RefreshRate = tailData.RefreshRate;
      FileEncoding = tailData.FileEncoding;
      FontType = tailData.FontType;
      SmartWatch = tailData.SmartWatch;
      AutoRun = tailData.AutoRun;
      FilterState = tailData.FilterState;
      PatternString = tailData.PatternString;
      IsRegex = tailData.IsRegex;
      Timestamp = tailData.Timestamp;
      ListOfFilter = tailData.ListOfFilter;
      OpenFromFileManager = tailData.OpenFromFileManager;
      UsePattern = tailData.UsePattern;
    }

    #region Description

    private string description;

    /// <summary>
    /// Description of item
    /// </summary>
    public string Description
    {
      get => description?.Trim();
      set
      {
        description = value;
        OnPropertyChanged("Description");
      }
    }

    #endregion

    #region Category

    private string category;

    /// <summary>
    /// Category of item
    /// </summary>
    public string Category
    {
      get => category?.Trim();
      set
      {
        category = value;
        OnPropertyChanged("Category");
      }
    }

    #endregion

    /// <summary>
    /// Unique ID of FileManager node
    /// </summary>
    public Guid ID
    {
      get;
      set;
    }

    /// <summary>
    /// Compatiblity old Id
    /// </summary>
    [Obsolete("Use Guid in futur")]
    public int OldId
    {
      get;
      set;
    }

    #region NewWindow

    private bool newWindow;

    /// <summary>
    /// Open thread in new window
    /// </summary>
    public bool NewWindow
    {
      get => newWindow;
      set
      {
        newWindow = value;
        OnPropertyChanged("NewWindow");
      }
    }

    #endregion

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
          System.Diagnostics.Debug.WriteLine(ex);
          throw;
        }
      }
    }

    /// <summary>
    /// Create copy of object
    /// </summary>
    /// <returns>A clone of object</returns>
    public new FileManagerData Clone()
    {
      return MemberwiseClone() as FileManagerData;
    }

    /// <summary>
    /// Save data to memenento
    /// </summary>
    /// <returns>Copy of FileManagerData</returns>
    public MementoFileManagerData SaveToMemento()
    {
      return new MementoFileManagerData(this);
    }

    /// <summary>
    /// Equals two objects
    /// </summary>
    /// <param name="obj">Reference of FileManagerData</param>
    /// <returns>If equal true otherwise false</returns>
    public bool EqualsProperties(object obj)
    {
      MementoFileManagerData other = obj as MementoFileManagerData;

      if ( other == null )
        return false;

      bool equal = true;

      equal &= Equals(other.Category, Category);
      equal &= Equals(other.Description, Description);
      equal &= Equals(other.FileEncoding, FileEncoding);
      equal &= Equals(other.FileName, FileName);
      equal &= Equals(other.FontType, FontType);
      equal &= Equals(other.KillSpace, KillSpace);
      equal &= CompareFilterList(ListOfFilter, other.ListOfFilter);
      equal &= Equals(other.NewWindow, NewWindow);
      equal &= Equals(other.RefreshRate, RefreshRate);
      equal &= Equals(other.ThreadPriority, ThreadPriority);
      equal &= Equals(other.Wrap, Wrap);
      equal &= Equals(other.TimeStamp, Timestamp);
      equal &= Equals(other.PatternString, PatternString);
      equal &= Equals(other.IsRegex, IsRegex);
      equal &= Equals(other.UsePattern, UsePattern);
      equal &= Equals(other.SmartWatch, SmartWatch);

      return equal;
    }

    private bool CompareFilterList(ObservableCollection<FilterData> a, ObservableCollection<FilterData> b)
    {
      if ( a.Count != b.Count )
        return false;

      int index = 0;
      bool equal = true;

      foreach ( FilterData item in a )
      {
        equal &= Equals(item.Description, b[index].Description);
        equal &= Equals(item.Filter, b[index].Filter);
        equal &= Equals(item.FilterColor, b[index].FilterColor);
        equal &= Equals(item.FilterFontType, b[index].FilterFontType);

        index++;
      }
      return equal;
    }

    /// <summary>
    /// Roll object back to the state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    /// <exception cref="ArgumentException">If memento object is no an MementoFileManagerData object</exception>
    public void RestoreFromMemento(MementoFileManagerData memento)
    {
      MementoFileManagerData mementoFmData = memento;
      Arg.NotNull(mementoFmData, "Argument is not a MementoFileManagerData");

      ID = mementoFmData.ID;
      Category = mementoFmData.Category;
      Description = mementoFmData.Description;
      NewWindow = mementoFmData.NewWindow;
      FileName = mementoFmData.FileName;
      Wrap = mementoFmData.Wrap;
      KillSpace = mementoFmData.KillSpace;
      RefreshRate = mementoFmData.RefreshRate;
      Timestamp = mementoFmData.TimeStamp;
      FontType = mementoFmData.FontType;
      ThreadPriority = mementoFmData.ThreadPriority;
      ListOfFilter = CloneObservableCollection.DeepCopy(mementoFmData.ListOfFilter);
      FileEncoding = mementoFmData.FileEncoding;
      PatternString = mementoFmData.PatternString;
      IsRegex = memento.IsRegex;
      UsePattern = memento.UsePattern;
      SmartWatch = memento.SmartWatch;
    }

    #region IComparer interface

    /// <summary>
    /// Compare
    /// </summary>
    /// <param name="x">FileManagerData x</param>
    /// <param name="y">FileManagerData y</param>
    /// <returns>Compareable result</returns>
    public int Compare(object x, object y)
    {
      if ( x is FileManagerData && y is FileManagerData )
      {
        var xFm = x as FileManagerData;
        var yFm = y as FileManagerData;

        DateTime nx = xFm.FileCreationTime ?? DateTime.MaxValue;
        DateTime ny = yFm.FileCreationTime ?? DateTime.MaxValue;

        return -nx.CompareTo(ny);
      }
      return 1;
    }

    #endregion

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

        if ( columnName == "Description" )
        {
          if ( string.IsNullOrEmpty(Description) )
            result = "Please enter a Description";
        }
        return result;
      }
    }

    #endregion

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class MementoFileManagerData
    {
      internal MementoFileManagerData(FileManagerData obj)
      {
        ID = obj.ID;
        Category = obj.Category;
        Description = obj.Description;
        NewWindow = obj.NewWindow;
        FileName = obj.FileName;
        Wrap = obj.Wrap;
        KillSpace = obj.KillSpace;
        RefreshRate = obj.RefreshRate;
        TimeStamp = obj.Timestamp;
        FontType = obj.FontType;
        ThreadPriority = obj.ThreadPriority;
        ListOfFilter = CloneObservableCollection.DeepCopy(obj.ListOfFilter);
        FileEncoding = obj.FileEncoding;
        PatternString = obj.PatternString;
        IsRegex = obj.IsRegex;
        UsePattern = obj.UsePattern;
        SmartWatch = obj.SmartWatch;
      }

      #region Properties memento

      /// <summary>
      /// Unique ID of FileManager node
      /// </summary>
      public Guid ID
      {
        get;
      }

      /// <summary>
      /// SmartWatch
      /// </summary>
      public bool SmartWatch
      {
        get;
      }

      /// <summary>
      /// Category of item
      /// </summary>
      public string Category
      {
        get;
      }

      /// <summary>
      /// Description of item
      /// </summary>
      public string Description
      {
        get;
      }

      /// <summary>
      /// Open thread in new window
      /// </summary>
      public bool NewWindow
      {
        get;
      }

      /// <summary>
      /// File encoding
      /// </summary>
      public Encoding FileEncoding
      {
        get;
      }

      /// <summary>
      /// FileName
      /// </summary>
      public string FileName
      {
        get;
      }

      /// <summary>
      /// Wrap text in textbox
      /// </summary>
      public bool Wrap
      {
        get;
      }

      /// <summary>
      /// Remove extra space in each line
      /// </summary>
      public bool KillSpace
      {
        get;
      }

      /// <summary>
      /// Refresh rate of thread
      /// </summary>
      public ETailRefreshRate RefreshRate
      {
        get;
      }

      /// <summary>
      /// Timestamp in taillog
      /// </summary>
      public bool TimeStamp
      {
        get;
      }

      /// <summary>
      /// Font of textbox
      /// </summary>
      public Font FontType
      {
        get;
      }

      /// <summary>
      /// Thread priority
      /// </summary>
      public System.Threading.ThreadPriority ThreadPriority
      {
        get;
      }

      /// <summary>
      /// List of filters
      /// </summary>
      public ObservableCollection<FilterData> ListOfFilter
      {
        get;
      }

      /// <summary>
      /// Current pattern string
      /// </summary>
      public string PatternString
      {
        get;
      }

      /// <summary>
      /// Is regex pattern
      /// </summary>
      public bool IsRegex
      {
        get;
      }

      /// <summary>
      /// Use pattern
      /// </summary>
      public bool UsePattern
      {
        get;
      }

      #endregion
    }
  }
}