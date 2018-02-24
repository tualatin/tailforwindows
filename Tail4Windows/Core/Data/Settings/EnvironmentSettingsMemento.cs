﻿using System.Threading;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Environment settings object
  /// </summary>
  public partial class EnvironmentSettings
  {
    /// <summary>
    /// Save data to memento
    /// </summary>
    /// <returns>Copy of <see cref="EnvironmentSettings"/></returns>
    public MementoEnvironmentSettings SaveToMemento() => new MementoEnvironmentSettings(this);

    /// <summary>
    /// Roll object back to state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    public void RestoreFromMemento(MementoEnvironmentSettings memento)
    {
      Arg.NotNull(memento, nameof(memento));

      RestoreWindowSize = memento.RestoreWindowSize;
      SaveWindowPosition = memento.SaveWindowPosition;
      ExitWithEscape = memento.ExitWithEscape;
      DeleteLogFiles = memento.DeleteLogFiles;
      CurrentWindowStyle = memento.CurrentWindowStyle;
      Language = memento.Language;
      AlwaysScrollToEnd = memento.AlwaysScrollToEnd;
      ShowNumberLineAtStart = memento.ShowNumberLineAtStart;
      ShowLineNumbers = memento.ShowLineNumbers;
      LinesRead = memento.LinesRead;
      GroupByCategory = memento.GroupByCategory;
      AutoUpdate = memento.AutoUpdate;
      DefaultRefreshRate = memento.DefaultRefreshRate;
      DefaultThreadPriority = memento.DefaultThreadPriority;
      DefaultTimeFormat = memento.DefaultTimeFormat;
      DefaultDateFormat = memento.DefaultDateFormat;

      ProxySettings = (ProxySetting) memento.ProxySettings.Clone();
      AlertSettings = (AlertSetting) memento.AlertSettings.Clone();
      SmartWatchSettings = (SmartWatchSetting) memento.SmartWatchSettings.Clone();
      ColorSettings = (EnvironmentColorSettings) memento.ColorSettings.Clone();
    }

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class MementoEnvironmentSettings
    {
      internal MementoEnvironmentSettings(EnvironmentSettings obj)
      {
        RestoreWindowSize = obj.RestoreWindowSize;
        SaveWindowPosition = obj.SaveWindowPosition;
        ExitWithEscape = obj.ExitWithEscape;
        DeleteLogFiles = obj.DeleteLogFiles;
        CurrentWindowStyle = obj.CurrentWindowStyle;
        Language = obj.Language;
        AlwaysScrollToEnd = obj.AlwaysScrollToEnd;
        ShowNumberLineAtStart = obj.ShowNumberLineAtStart;
        ShowLineNumbers = obj.ShowLineNumbers;
        LinesRead = obj.LinesRead;
        GroupByCategory = obj.GroupByCategory;
        AutoUpdate = obj.AutoUpdate;
        DefaultRefreshRate = obj.DefaultRefreshRate;
        DefaultThreadPriority = obj.DefaultThreadPriority;
        DefaultTimeFormat = obj.DefaultTimeFormat;
        DefaultDateFormat = obj.DefaultDateFormat;

        ProxySettings = (ProxySetting) obj.ProxySettings.Clone();
        AlertSettings = (AlertSetting) obj.AlertSettings.Clone();
        SmartWatchSettings = (SmartWatchSetting) obj.SmartWatchSettings.Clone();
        ColorSettings = (EnvironmentColorSettings) obj.ColorSettings.Clone();
      }

      #region Window settings

      /// <summary>
      /// Current UI language
      /// </summary>
      public EUiLanguage Language
      {
        get;
      }

      /// <summary>
      /// Restore window size at startup
      /// </summary>
      public bool RestoreWindowSize
      {
        get;
      }

      /// <summary>
      /// Save window position
      /// </summary>
      public bool SaveWindowPosition
      {
        get;
      }

      #endregion

      /// <summary>
      /// Current proxy settings
      /// </summary>
      public ProxySetting ProxySettings
      {
        get;
      }

      /// <summary>
      /// Current alert settings
      /// </summary>
      public AlertSetting AlertSettings
      {
        get;
      }

      /// <summary>
      /// Environment color settings
      /// </summary>
      public EnvironmentColorSettings ColorSettings
      {
        get;
      }

      /// <summary>
      /// Current SmartWatch settings
      /// </summary>
      public SmartWatchSetting SmartWatchSettings
      {
        get;
      }

      /// <summary>
      /// Close/exist T4W by pressing Escape key
      /// </summary>
      public bool ExitWithEscape
      {
        get;
      }

      /// <summary>
      /// Delete old T4W log files
      /// </summary>
      public bool DeleteLogFiles
      {
        get;
      }

      /// <summary>
      /// Current window style
      /// </summary>
      public EWindowStyle CurrentWindowStyle
      {
        get;
      }

      /// <summary>
      /// Always scroll to end
      /// </summary>
      public bool AlwaysScrollToEnd
      {
        get;
      }

      /// <summary>
      /// Show number lines at start
      /// </summary>
      public bool ShowNumberLineAtStart
      {
        get;
      }

      /// <summary>
      /// Show line number
      /// </summary>
      public bool ShowLineNumbers
      {
        get;
      }

      /// <summary>
      /// Lines read
      /// </summary>
      public int LinesRead
      {
        get;
      }

      /// <summary>
      /// Group by category
      /// </summary>
      public bool GroupByCategory
      {
        get;
      }

      /// <summary>
      /// AutoUpdate
      /// </summary>
      public bool AutoUpdate
      {
        get;
      }

      /// <summary>
      /// Default refresh rate
      /// </summary>
      public ETailRefreshRate DefaultRefreshRate
      {
        get;
      }

      /// <summary>
      /// Default thread priority
      /// </summary>
      public ThreadPriority DefaultThreadPriority
      {
        get;
      }

      /// <summary>
      /// Default time format
      /// </summary>
      public ETimeFormat DefaultTimeFormat
      {
        get;
      }

      /// <summary>
      /// Default date format
      /// </summary>
      public EDateFormat DefaultDateFormat
      {
        get;
      }
    }
  }
}
