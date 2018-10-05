using System.Threading;
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

      IsUserSettings = memento.IsUserSettings;
      RestoreWindowSize = memento.RestoreWindowSize;
      SaveWindowPosition = memento.SaveWindowPosition;
      ExitWithEscape = memento.ExitWithEscape;
      SingleInstance = memento.SingleInstance;
      ActivateDragDropWindow = memento.ActivateDragDropWindow;
      DeleteLogFiles = memento.DeleteLogFiles;
      LogFilesOlderThan = memento.LogFilesOlderThan;
      CurrentWindowStyle = memento.CurrentWindowStyle;
      Language = memento.Language;
      AlwaysScrollToEnd = memento.AlwaysScrollToEnd;
      ContinuedScroll = memento.ContinuedScroll;
      ShowNumberLineAtStart = memento.ShowNumberLineAtStart;
      ShowLineNumbers = memento.ShowLineNumbers;
      LinesRead = memento.LinesRead;
      GroupByCategory = memento.GroupByCategory;
      AutoUpdate = memento.AutoUpdate;
      DefaultRefreshRate = memento.DefaultRefreshRate;
      DefaultThreadPriority = memento.DefaultThreadPriority;
      DefaultTimeFormat = memento.DefaultTimeFormat;
      DefaultDateFormat = memento.DefaultDateFormat;
      DefaultFileSort = memento.DefaultFileSort;
      LogLineLimit = memento.LogLineLimit;
      SmartWatch = memento.SmartWatch;
      Statistics = memento.Statistics;
      SaveLogFileHistory = memento.SaveLogFileHistory;
      HistoryMaxSize = memento.HistoryMaxSize;
      ShowExtendedSettings = memento.ShowExtendedSettings;
      SplitterWindowBehavior = memento.SplitterWindowBehavior;
      EditorPath = memento.EditorPath;

      ProxySettings = (ProxySetting) memento.ProxySettings.Clone();
      AlertSettings = (AlertSetting) memento.AlertSettings.Clone();
      SmartWatchSettings = (SmartWatchSetting) memento.SmartWatchSettings.Clone();
      ColorSettings = (EnvironmentColorSettings) memento.ColorSettings.Clone();
      SmtpSettings = (SmtpSetting) memento.SmtpSettings.Clone();

      ProxySettings.RaiseOnPropertyChanged();
      AlertSettings.RaiseOnPropertyChanged();
      SmartWatchSettings.RaiseOnPropertyChanged();
      ColorSettings.RaiseOnPropertyChanged();
      SmtpSettings.RaiseOnPropertyChanged();
    }

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class MementoEnvironmentSettings
    {
      internal MementoEnvironmentSettings(EnvironmentSettings obj)
      {
        IsUserSettings = obj.IsUserSettings;
        RestoreWindowSize = obj.RestoreWindowSize;
        SaveWindowPosition = obj.SaveWindowPosition;
        ExitWithEscape = obj.ExitWithEscape;
        SingleInstance = obj.SingleInstance;
        ActivateDragDropWindow = obj.ActivateDragDropWindow;
        DeleteLogFiles = obj.DeleteLogFiles;
        LogFilesOlderThan = obj.LogFilesOlderThan;
        CurrentWindowStyle = obj.CurrentWindowStyle;
        Language = obj.Language;
        AlwaysScrollToEnd = obj.AlwaysScrollToEnd;
        ContinuedScroll = obj.ContinuedScroll;
        ShowNumberLineAtStart = obj.ShowNumberLineAtStart;
        ShowLineNumbers = obj.ShowLineNumbers;
        LinesRead = obj.LinesRead;
        GroupByCategory = obj.GroupByCategory;
        AutoUpdate = obj.AutoUpdate;
        DefaultRefreshRate = obj.DefaultRefreshRate;
        DefaultThreadPriority = obj.DefaultThreadPriority;
        DefaultTimeFormat = obj.DefaultTimeFormat;
        DefaultDateFormat = obj.DefaultDateFormat;
        DefaultFileSort = obj.DefaultFileSort;
        LogLineLimit = obj.LogLineLimit;
        SmartWatch = obj.SmartWatch;
        Statistics = obj.Statistics;
        SaveLogFileHistory = obj.SaveLogFileHistory;
        HistoryMaxSize = obj.HistoryMaxSize;
        ShowExtendedSettings = obj.ShowExtendedSettings;
        SplitterWindowBehavior = obj.SplitterWindowBehavior;
        EditorPath = obj.EditorPath;

        ProxySettings = (ProxySetting) obj.ProxySettings.Clone();
        AlertSettings = (AlertSetting) obj.AlertSettings.Clone();
        SmartWatchSettings = (SmartWatchSetting) obj.SmartWatchSettings.Clone();
        ColorSettings = (EnvironmentColorSettings) obj.ColorSettings.Clone();
        SmtpSettings = (SmtpSetting) obj.SmtpSettings.Clone();
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
      /// Save settings in user roaming path or use it as portable app
      /// </summary>
      public bool IsUserSettings
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

      /// <summary>
      /// Activate Drag'n'Drop window behavior
      /// </summary>
      public bool ActivateDragDropWindow
      {
        get;
      }

      /// <summary>
      /// Save LogFile history
      /// </summary>
      public bool SaveLogFileHistory
      {
        get;
      }

      /// <summary>
      /// LogFile history max size
      /// </summary>
      public int HistoryMaxSize
      {
        get;
      }

      /// <summary>
      /// SplitterWindow behavior
      /// </summary>
      public bool SplitterWindowBehavior
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
      /// Current SMTP settings
      /// </summary>
      public SmtpSetting SmtpSettings
      {
        get;
      }

      /// <summary>
      /// Single instance
      /// </summary>
      public bool SingleInstance
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
      /// ContinuedScroll or pushed method
      /// </summary>
      public bool ContinuedScroll
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

      /// <summary>
      /// Default file sort
      /// </summary>
      public EFileSort DefaultFileSort
      {
        get;
      }

      /// <summary>
      /// Log line limitation
      /// </summary>
      public int LogLineLimit
      {
        get;
      }

      /// <summary>
      /// Enable SmartWatch
      /// </summary>
      public bool SmartWatch
      {
        get;
      }

      /// <summary>
      /// Statistics for nerds
      /// </summary>
      public bool Statistics
      {
        get;
      }

      /// <summary>
      /// Log files older than xxx days
      /// </summary>
      public int LogFilesOlderThan
      {
        get;
      }

      /// <summary>
      /// Show extended settings
      /// </summary>
      public bool ShowExtendedSettings
      {
        get;
      }

      /// <summary>
      /// Editor path
      /// </summary>
      public string EditorPath
      {
        get;
      }
    }
  }
}
