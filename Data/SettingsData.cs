using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;
using Org.Vs.TailForWin.Data.Base;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// SettingsData object
  /// </summary>
  public class SettingsData : INotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SettingsData()
    {
      AlertSettings = new AlertData();
      ProxySettings = new ProxySettingsData();
    }

    /// <summary>
    /// Lines to read at the begining
    /// </summary>
    public int LinesRead
    {
      get;
      set;
    }

    #region AlwaysOnTop

    private bool alwaysOnTop;

    /// <summary>
    /// Tail window always on top
    /// </summary>
    public bool AlwaysOnTop
    {
      get => (alwaysOnTop);
      set
      {
        alwaysOnTop = value;
        OnPropertyChanged("AlwaysOnTop");
      }
    }

    #endregion

    #region ShowNLineAtStart

    private bool showNLineAtStart;

    /// <summary>
    /// Show n lines when start tail thread
    /// </summary>
    public bool ShowNLineAtStart
    {
      get => (showNLineAtStart);
      set
      {
        showNLineAtStart = value;
        OnPropertyChanged("ShowNLineAtStart");
      }
    }

    #endregion

    #region AlwaysScrollToEnd

    private bool alwaysScrollToEnd;

    /// <summary>
    /// Scroll always to the end
    /// </summary>
    public bool AlwaysScrollToEnd
    {
      get => (alwaysScrollToEnd);
      set
      {
        alwaysScrollToEnd = value;
        OnPropertyChanged("AlwaysScrollToEnd");
      }
    }

    #endregion

    #region RestoreWindowSize

    private bool restoreWindowSize;

    /// <summary>
    /// Restore window size at startup
    /// </summary>
    public bool RestoreWindowSize
    {
      get => (restoreWindowSize);
      set
      {
        restoreWindowSize = value;
        OnPropertyChanged("RestoreWindowSize");
      }
    }

    #endregion

    /// <summary>
    /// Window height
    /// </summary>
    public double WndHeight
    {
      get;
      set;
    }

    /// <summary>
    /// Window width
    /// </summary>
    public double WndWidth
    {
      get;
      set;
    }

    #region SaveWindowPosition

    private bool saveWindowPosition;

    /// <summary>
    /// Restore window position at startup
    /// </summary>
    public bool SaveWindowPosition
    {
      get => (saveWindowPosition);
      set
      {
        saveWindowPosition = value;
        OnPropertyChanged("SaveWindowPosition");
      }
    }

    #endregion

    /// <summary>
    /// Window x position
    /// </summary>
    public double WndXPos
    {
      get;
      set;
    }

    /// <summary>
    /// Window y position
    /// </summary>
    public double WndYPos
    {
      get;
      set;
    }

    /// <summary>
    /// Search window x position
    /// </summary>
    public double SearchWndXPos
    {
      get;
      set;
    }

    /// <summary>
    /// Search window y position
    /// </summary>
    public double SearchWndYPos
    {
      get;
      set;
    }

    /// <summary>
    /// Default thread priority
    /// </summary>
    public System.Threading.ThreadPriority DefaultThreadPriority
    {
      get;
      set;
    }

    #region ExitWithEsc

    private bool exitWithEsc;

    /// <summary>
    /// Exit by pressing Escape key
    /// </summary>
    public bool ExitWithEscape
    {
      get => (exitWithEsc);
      set
      {
        exitWithEsc = value;
        OnPropertyChanged("ExitWithEscape");
      }
    }

    #endregion

    #region ShowLineNumbers

    private bool showLineNumbers;

    /// <summary>
    /// Show line numbers in Taillogwindow
    /// </summary>
    public bool ShowLineNumbers
    {
      get => (showLineNumbers);
      set
      {
        showLineNumbers = value;
        OnPropertyChanged("ShowLineNumbers");
      }
    }

    #endregion

    #region DefaultForegroundColor

    private string defaultForegroundColor;

    /// <summary>
    /// Default foreground color
    /// </summary>
    public string DefaultForegroundColor
    {
      get => (defaultForegroundColor);
      set
      {
        defaultForegroundColor = value;

        if(!string.IsNullOrEmpty(defaultForegroundColor))
          GuiDefaultForegroundColor = GetColorFromHex(defaultForegroundColor);
      }
    }

    #endregion

    #region DefaultBackgroundColor

    private string defaultBackgroundColor;

    /// <summary>
    /// Default background color
    /// </summary>
    public string DefaultBackgroundColor
    {
      get => (defaultBackgroundColor);
      set
      {
        defaultBackgroundColor = value;

        if(!string.IsNullOrEmpty(defaultBackgroundColor))
          GuiDefaultBackgroundColor = GetColorFromHex(defaultBackgroundColor);
      }
    }

    #endregion

    #region GuiDefaultForegroundColor

    private Brush guiDefaultForegroundColor;

    /// <summary>
    /// Gui foreground color
    /// </summary>
    public Brush GuiDefaultForegroundColor
    {
      get => (guiDefaultForegroundColor);
      set
      {
        guiDefaultForegroundColor = value;
        OnPropertyChanged("GuiDefaultForegroundColor");
      }
    }

    #endregion

    #region GuiDefaultBackgroundColor

    private Brush guiDefaultBackgroundColor;

    /// <summary>
    /// Gui background color
    /// </summary>
    public Brush GuiDefaultBackgroundColor
    {
      get => (guiDefaultBackgroundColor);
      set
      {
        guiDefaultBackgroundColor = value;
        OnPropertyChanged("GuiDefaultBackgroundColor");
      }
    }

    #endregion

    #region DefaultInactiveForegroundColor

    private string defaultInactiveForegroundColor;

    /// <summary>
    /// Default inactive foreground color
    /// </summary>
    public string DefaultInactiveForegroundColor
    {
      get => (defaultInactiveForegroundColor);
      set
      {
        defaultInactiveForegroundColor = value;

        if(!string.IsNullOrEmpty(defaultInactiveForegroundColor))
          GuiDefaultInactiveForegroundColor = GetColorFromHex(defaultInactiveForegroundColor);
      }
    }

    #endregion

    #region GuiDefaultInactiveForegroundColor

    private Brush guiDefaultInactiveForegroundColor;

    /// <summary>
    /// Gui inactive foreground color
    /// </summary>
    public Brush GuiDefaultInactiveForegroundColor
    {
      get => (guiDefaultInactiveForegroundColor);
      set
      {
        guiDefaultInactiveForegroundColor = value;
        OnPropertyChanged("GuiDefaultInactiveForegroundColor");
      }
    }

    #endregion

    #region DefaultInactiveBackgroundColor

    private string defaultInactiveBackgroundColor;

    /// <summary>
    /// Default inactive background color
    /// </summary>
    public string DefaultInactiveBackgroundColor
    {
      get => (defaultInactiveBackgroundColor);
      set
      {
        defaultInactiveBackgroundColor = value;

        if(!string.IsNullOrEmpty(defaultInactiveBackgroundColor))
          GuiDefaultInactiveBackgroundColor = GetColorFromHex(defaultInactiveBackgroundColor);
      }
    }

    #endregion

    #region GuiDefaultInactiveBackgroundColor

    private Brush guiDefaultInactiveBackgroundColor;

    /// <summary>
    /// Gui inactive background color
    /// </summary>
    public Brush GuiDefaultInactiveBackgroundColor
    {
      get => (guiDefaultInactiveBackgroundColor);
      set
      {
        guiDefaultInactiveBackgroundColor = value;
        OnPropertyChanged("GuiDefaultInactiveBackgroundColor");
      }
    }

    #endregion

    #region DefaultFindHiglightForegoundColor

    private string defaultFindHighlightForegroundColor;

    /// <summary>
    /// Default find highlight foreground color
    /// </summary>
    public string DefaultHighlightForegroundColor
    {
      get => (defaultFindHighlightForegroundColor);
      set
      {
        defaultFindHighlightForegroundColor = value;

        if(!string.IsNullOrEmpty(defaultFindHighlightForegroundColor))
          GuiDefaultHighlightForegroundColor = GetColorFromHex(defaultFindHighlightForegroundColor);
      }
    }

    #endregion

    #region GuiDefaultHighlightForegroundColor

    private Brush guiDefaultHighlightForegroundColor;

    /// <summary>
    /// Gui find highlight foreground color
    /// </summary>
    public Brush GuiDefaultHighlightForegroundColor
    {
      get => (guiDefaultHighlightForegroundColor);
      set
      {
        guiDefaultHighlightForegroundColor = value;
        OnPropertyChanged("GuiDefaultHighlightForegroundColor");
      }
    }

    #endregion

    #region DefaultFindHighlightBackgroundColor

    private string defaultFindHighlightBackgroundColor;

    /// <summary>
    /// Default find highlight background color
    /// </summary>
    public string DefaultHighlightBackgroundColor
    {
      get => (defaultFindHighlightBackgroundColor);
      set
      {
        defaultFindHighlightBackgroundColor = value;

        if(!string.IsNullOrEmpty(defaultFindHighlightBackgroundColor))
          GuiDefaultHighlightBackgroundColor = GetColorFromHex(defaultFindHighlightBackgroundColor);
      }
    }

    #endregion

    #region GuiDefaultHighlightBackgroundColor

    private Brush guiDefaultHighlightBackgroundColor;

    /// <summary>
    /// Gui find highlight background color
    /// </summary>
    public Brush GuiDefaultHighlightBackgroundColor
    {
      get => (guiDefaultHighlightBackgroundColor);
      set
      {
        guiDefaultHighlightBackgroundColor = value;
        OnPropertyChanged("GuiDefaultHighlightBackgroundColor");
      }
    }

    #endregion

    #region DefaultLineNumbersColor

    private string defaultLineNumbersColor;

    /// <summary>
    /// Default line numbers color
    /// </summary>
    public string DefaultLineNumbersColor
    {
      get => (defaultLineNumbersColor);
      set
      {
        defaultLineNumbersColor = value;

        if(!string.IsNullOrEmpty(defaultLineNumbersColor))
          GuiDefaultLineNumbersColor = GetColorFromHex(defaultLineNumbersColor);
      }
    }

    #endregion

    #region GuiDefaultLineNumbersColor

    private Brush guiDefaultLineNumbersColor;

    /// <summary>
    /// Gui line numbers color
    /// </summary>
    public Brush GuiDefaultLineNumbersColor
    {
      get => (guiDefaultLineNumbersColor);
      set
      {
        guiDefaultLineNumbersColor = value;
        OnPropertyChanged("GuiDefaultLineNumbersColor");
      }
    }

    #endregion

    #region DefaultHighlightColor

    private string defaultHighlightColor;

    /// <summary>
    /// Default highlight color
    /// </summary>
    public string DefaultHighlightColor
    {
      get => (defaultHighlightColor);
      set
      {
        defaultHighlightColor = value;

        if(!string.IsNullOrEmpty(defaultHighlightColor))
          GuiDefaultHighlightColor = GetColorFromHex(defaultHighlightColor);
      }
    }

    #endregion

    #region GuiDefaultHighlightColor

    private Brush guiDefaultHighlightColor;

    /// <summary>
    /// Gui highlight color
    /// </summary>
    public Brush GuiDefaultHighlightColor
    {
      get => (guiDefaultHighlightColor);
      set
      {
        guiDefaultHighlightColor = value;
        OnPropertyChanged("GuiDefaultHighlightColor");
      }
    }

    #endregion

    /// <summary>
    /// Proxy settings
    /// </summary>
    public ProxySettingsData ProxySettings
    {
      get;
      set;
    }

    /// <summary>
    /// Default thread refresh rate
    /// </summary>
    public ETailRefreshRate DefaultRefreshRate
    {
      get;
      set;
    }

    /// <summary>
    /// Default time format
    /// </summary>
    public ETimeFormat DefaultTimeFormat
    {
      get;
      set;
    }

    /// <summary>
    /// Default date format
    /// </summary>
    public EDateFormat DefaultDateFormat
    {
      get;
      set;
    }

    /// <summary>
    /// Default file sort
    /// </summary>
    public EFileSort DefaultFileSort
    {
      get;
      set;
    }


    /// <summary>
    /// Group GridControl in FileManager by Category
    /// </summary>
    public bool GroupByCategory
    {
      get;
      set;
    }

    /// <summary>
    /// Alert settings
    /// </summary>
    public AlertData AlertSettings
    {
      get;
      set;
    }

    #region AutoUpdate

    private bool autoUpdate;

    /// <summary>
    /// Check for update at startup
    /// </summary>
    public bool AutoUpdate
    {
      get => (autoUpdate);
      set
      {
        autoUpdate = value;
        OnPropertyChanged("AutoUpdate");
      }
    }

    #endregion

    #region SmartWatch

    private bool useSmartWatch;

    /// <summary>
    /// Use Smart watch at tailing a log file
    /// </summary>
    public bool SmartWatch
    {
      get => useSmartWatch;
      set
      {
        useSmartWatch = value;
        OnPropertyChanged("SmartWatch");
      }
    }

    #endregion

    /// <summary>
    /// Webside for new releases
    /// </summary>
    public static string ApplicationWebUrl => ("https://github.com/tualatin/tailforwindows/releases");

    private static Brush GetColorFromHex(string hex)
    {
      Color color = (Color) ColorConverter.ConvertFromString(hex);

      return (new SolidColorBrush(color));
    }

    /// <summary>
    /// Get description from enum
    /// </summary>
    /// <param name="value">Reference of enum</param>
    /// <returns>Description of enum</returns>
    public static string GetEnumDescription(Enum value)
    {
      FieldInfo fi = value.GetType().GetField(value.ToString());
      DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

      if(attributes != null && attributes.Length > 0)
        return (attributes[0].Description);
      else
        return (value.ToString());
    }

    /// <summary>
    /// Get enum from description
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="description">Reference of description string</param>
    /// <returns>Enum type</returns>
    public static T GetDescriptionEnum<T>(string description)
    {
      var type = typeof(T);

      if(!type.IsEnum)
        throw new InvalidOperationException();

      foreach(var field in type.GetFields())
      {
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if(attribute != null)
        {
          if(string.Compare(attribute.Description, description, false) == 0)
            return ((T) field.GetValue(null));
        }
        else
        {
          if(string.Compare(field.Name, description, false) == 0)
            return ((T) field.GetValue(null));
        }
      }
      throw new ArgumentException("Not found.", "description");
      // or return default(T);
    }
  }
}
