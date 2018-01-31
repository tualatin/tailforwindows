﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Tail data object
  /// </summary>
  public partial class TailData
  {
    /// <summary>
    /// Save data to memento
    /// </summary>
    /// <returns>Copy of TailData</returns>
    public MementoTailData SaveToMemento()
    {
      return new MementoTailData(this);
    }

    /// <summary>
    /// Roll object back to state of provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    public void RestoreFromMemento(MementoTailData memento)
    {
      MementoTailData mementoTailData = memento;
      Arg.NotNull(mementoTailData, nameof(mementoTailData));

      Id = mementoTailData.Id;
      Category = mementoTailData.Category;
      Description = mementoTailData.Description;
      NewWindow = mementoTailData.NewWindow;
      FileEncoding = mementoTailData.FileEncoding;
      RefreshRate = mementoTailData.RefreshRate;
      SmartWatch = mementoTailData.SmartWatch;
      OpenFromSmartWatch = mementoTailData.OpenFromSmartWatch;
      RemoveSpace = mementoTailData.RemoveSpace;
      FileName = mementoTailData.FileName;
      Wrap = mementoTailData.Wrap;
      Timestamp = mementoTailData.TimeStamp;
      FontType = mementoTailData.FontType;
      ThreadPriority = mementoTailData.ThreadPriority;
      PatternString = mementoTailData.PatternString;
      IsRegex = mementoTailData.IsRegex;
      UsePattern = mementoTailData.UsePattern;
      AutoRun = mementoTailData.AutoRun;
      FilterState = mementoTailData.FilterState;
      LastRefreshTime = mementoTailData.LastRefreshTime;
      ListOfFilter = new ObservableCollection<FilterData>(mementoTailData.ListOfFilter);
    }

    /// <summary>
    /// Memento Tail data pattern
    /// </summary>
    public class MementoTailData
    {
      internal MementoTailData(TailData value)
      {
        Id = value.Id;
        Category = value.Category;
        Description = value.Description;
        NewWindow = value.NewWindow;
        FileEncoding = value.FileEncoding;
        RefreshRate = value.RefreshRate;
        SmartWatch = value.SmartWatch;
        OpenFromSmartWatch = value.OpenFromSmartWatch;
        RemoveSpace = value.RemoveSpace;
        FileName = value.FileName;
        Wrap = value.Wrap;
        TimeStamp = value.Timestamp;
        FontType = value.FontType;
        ThreadPriority = value.ThreadPriority;
        PatternString = value.PatternString;
        IsRegex = value.IsRegex;
        UsePattern = value.UsePattern;
        AutoRun = value.AutoRun;
        FilterState = value.FilterState;
        LastRefreshTime = value.LastRefreshTime;
        ListOfFilter = new ObservableCollection<FilterData>(value.ListOfFilter);
      }

      /// <summary>
      /// Unique ID of FileManager node
      /// </summary>
      public Guid Id
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
      public bool RemoveSpace
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

      /// <summary>
      /// Last refresh time
      /// </summary>
      public DateTime LastRefreshTime
      {
        get;
      }

      /// <summary>
      /// Is filter checkbox on/off
      /// </summary>
      public bool FilterState
      {
        get;
      }

      /// <summary>
      /// Tail automatically after tab is created
      /// </summary>
      public bool AutoRun
      {
        get;
      }

      /// <summary>
      /// Properties comes from SmartWatch
      /// </summary>
      public bool OpenFromSmartWatch
      {
        get;
      }
    }
  }
}