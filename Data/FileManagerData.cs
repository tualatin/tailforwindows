using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// Dataobject for FileManagerProperties
  /// </summary>
  public class FileManagerData : TailLogData
  {
    #region Description

    private string description;

    /// <summary>
    /// Description of item
    /// </summary>
    public string Description
    {
      get
      {
        if(description == null)
          return (null);

        return (description.Trim());
      }
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
      get
      {
        if(category == null)
          return (null);

        return (category.Trim());
      }
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
    public int ID
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
      get
      {
        return (newWindow);
      }
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
        if(System.IO.File.Exists(FileName))
          return (System.IO.File.GetCreationTime(FileName));
        else
          return (null);
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
          return (now.Subtract((DateTime) FileCreationTime));
        }
        catch(ArgumentOutOfRangeException ex)
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
    public FileManagerData Clone()
    {
      return (this.MemberwiseClone() as FileManagerData);
    }

    /// <summary>
    /// Save data to memenento
    /// </summary>
    /// <returns>Copy of FileManagerData</returns>
    public MementoFileManagerData SaveToMemento()
    {
      return (new MementoFileManagerData(this));
    }

    /// <summary>
    /// Equals two objects
    /// </summary>
    /// <param name="obj">Reference of FileManagerData</param>
    /// <returns>If equal true otherwise false</returns>
    public bool EqualsProperties(object obj)
    {
      FileManagerData.MementoFileManagerData other = obj as FileManagerData.MementoFileManagerData;

      if(other == null)
        return (false);

      bool equal = true;

      equal &= object.Equals(other.Category, Category);
      equal &= object.Equals(other.Description, Description);
      equal &= object.Equals(other.FileEncoding, FileEncoding);
      equal &= object.Equals(other.FileName, FileName);
      equal &= object.Equals(other.FontType, FontType);
      equal &= object.Equals(other.KillSpace, KillSpace);
      equal &= object.Equals(other.ListOfFilter, ListOfFilter);
      equal &= object.Equals(other.NewWindow, NewWindow);
      equal &= object.Equals(other.RefreshRate, RefreshRate);
      equal &= object.Equals(other.ThreadPriority, ThreadPriority);
      equal &= object.Equals(other.Wrap, Wrap);
      equal &= object.Equals(other.TimeStamp, Timestamp);

      return (equal);
    }

    /// <summary>
    /// Roll object back to the state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    public void RestoreFromMemento(MementoFileManagerData memento)
    {
      MementoFileManagerData mementoFMData = memento as MementoFileManagerData;

      if(mementoFMData == null)
        throw new ArgumentException(string.Format("Argument is not a MementoFileManagerData {0}!", memento.GetType().Name), "memento");

      ID = mementoFMData.ID;
      Category = mementoFMData.Category;
      Description = mementoFMData.Description;
      NewWindow = mementoFMData.NewWindow;
      FileName = mementoFMData.FileName;
      Wrap = mementoFMData.Wrap;
      KillSpace = mementoFMData.KillSpace;
      RefreshRate = mementoFMData.RefreshRate;
      Timestamp = mementoFMData.TimeStamp;
      FontType = mementoFMData.FontType;
      ThreadPriority = mementoFMData.ThreadPriority;
      ListOfFilter = mementoFMData.ListOfFilter;
      FileEncoding = mementoFMData.FileEncoding;
    }

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
        ListOfFilter = obj.ListOfFilter;
        FileEncoding = obj.FileEncoding;
      }

      #region Properties memento

      /// <summary>
      /// Unique ID of FileManager node
      /// </summary>
      public int ID
      {
        get;
        private set;
      }

      /// <summary>
      /// Category of item
      /// </summary>
      public string Category
      {
        get;
        private set;
      }

      /// <summary>
      /// Description of item
      /// </summary>
      public string Description
      {
        get;
        private set;
      }

      /// <summary>
      /// Open thread in new window
      /// </summary>
      public bool NewWindow
      {
        get;
        private set;
      }

      /// <summary>
      /// File encoding
      /// </summary>
      public Encoding FileEncoding
      {
        get;
        private set;
      }

      /// <summary>
      /// FileName
      /// </summary>
      public string FileName
      {
        get;
        private set;
      }

      /// <summary>
      /// Wrap text in textbox
      /// </summary>
      public bool Wrap
      {
        get;
        private set;
      }

      /// <summary>
      /// Remove extra space in each line
      /// </summary>
      public bool KillSpace
      {
        get;
        private set;
      }

      /// <summary>
      /// Refresh rate of thread
      /// </summary>
      public ETailRefreshRate RefreshRate
      {
        get;
        private set;
      }

      /// <summary>
      /// Timestamp in taillog
      /// </summary>
      public bool TimeStamp
      {
        get;
        private set;
      }

      /// <summary>
      /// Font of textbox
      /// </summary>
      public Font FontType
      {
        get;
        private set;
      }

      /// <summary>
      /// Thread priority
      /// </summary>
      public System.Threading.ThreadPriority ThreadPriority
      {
        get;
        private set;
      }

      /// <summary>
      /// List of filters
      /// </summary>
      public ObservableCollection<FilterData> ListOfFilter
      {
        get;
        private set;
      }

      #endregion
    }
  }
}