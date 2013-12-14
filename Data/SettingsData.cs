using System.ComponentModel;
using System.Reflection;
using System;
using System.Windows.Media;


namespace TailForWin.Data
{
  public class SettingsData: INotifyMaster
  {
    public SettingsData ()
    {
      AlertSettings = new AlertData ( );
      ProxySettings = new ProxySettingsData ( );
    }

    /// <summary>
    /// Lines to read at the begining
    /// </summary>
    public int LinesRead
    {
      get;
      set;
    }

    private bool alwaysOnTop;

    /// <summary>
    /// Tail window always on top
    /// </summary>
    public bool AlwaysOnTop
    {
      get
      {
        return (alwaysOnTop);
      }
      set
      {
        alwaysOnTop = value;
        OnPropertyChanged ("AlwaysOnTop");
      }
    }

    private bool showNLineAtStart;

    /// <summary>
    /// Show n lines when start tail thread
    /// </summary>
    public bool ShowNLineAtStart
    {
      get
      {
        return (showNLineAtStart);
      }
      set
      {
        showNLineAtStart = value;
        OnPropertyChanged ("ShowNLineAtStart");
      }
    }

    private bool alwaysScrollToEnd;

    /// <summary>
    /// Scroll always to the end
    /// </summary>
    public bool AlwaysScrollToEnd
    {
      get
      {
        return (alwaysScrollToEnd);
      }
      set
      {
        alwaysScrollToEnd = value;
        OnPropertyChanged ("AlwaysScrollToEnd");
      }
    }

    private bool restoreWindowSize;

    /// <summary>
    /// Restore window size at startup
    /// </summary>
    public bool RestoreWindowSize
    {
      get
      {
        return (restoreWindowSize);
      }
      set
      {
        restoreWindowSize = value;
        OnPropertyChanged ("RestoreWindowSize");
      }
    }

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

    private bool saveWindowPosition;

    /// <summary>
    /// Restore window position at startup
    /// </summary>
    public bool SaveWindowPosition
    {
      get
      {
        return (saveWindowPosition);
      }
      set
      {
        saveWindowPosition = value;
        OnPropertyChanged ("SaveWindowPosition");
      }
    }

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

    private bool exitWithEsc;

    /// <summary>
    /// Exit by pressing Escape key
    /// </summary>
    public bool ExitWithEscape
    {
      get
      {
        return (exitWithEsc);
      }
      set
      {
        exitWithEsc = value;
        OnPropertyChanged ("ExitWithEscape");
      }
    }

    private bool showLineNumbers;

    /// <summary>
    /// Show line numbers in Taillogwindow
    /// </summary>
    public bool ShowLineNumbers
    {
      get
      {
        return (showLineNumbers);
      }
      set
      {
        showLineNumbers = value;
        OnPropertyChanged ("ShowLineNumbers");
      }
    }

    private string defaultForegroundColor;

    /// <summary>
    /// Default foreground color
    /// </summary>
    public string DefaultForegroundColor
    {
      get
      {
        return (defaultForegroundColor);
      }
      set
      {
        defaultForegroundColor = value;

        if (!string.IsNullOrEmpty (defaultForegroundColor))
          GuiDefaultForegroundColor = GetColorFromHex (defaultForegroundColor);
      }
    }

    private string defaultBackgroundColor;

    /// <summary>
    /// Default background color
    /// </summary>
    public string DefaultBackgroundColor
    {
      get
      {
        return (defaultBackgroundColor);
      }
      set
      {
        defaultBackgroundColor = value;

        if (!string.IsNullOrEmpty (defaultBackgroundColor))
          GuiDefaultBackgroundColor = GetColorFromHex (defaultBackgroundColor);
      }
    }

    private Brush guiDefaultForegroundColor;

    /// <summary>
    /// Gui foreground color
    /// </summary>
    public Brush GuiDefaultForegroundColor
    {
      get
      {
        return (guiDefaultForegroundColor);
      }
      set
      {
        guiDefaultForegroundColor = value;
        OnPropertyChanged ("GuiDefaultForegroundColor");
      }
    }

    private Brush guiDefaultBackgroundColor;

    /// <summary>
    /// Gui background color
    /// </summary>
    public Brush GuiDefaultBackgroundColor
    {
      get
      {
        return (guiDefaultBackgroundColor);
      }
      set
      {
        guiDefaultBackgroundColor = value;
        OnPropertyChanged ("GuiDefaultBackgroundColor");
      }
    }

    private string defaultInactiveForegroundColor;

    /// <summary>
    /// Default inactive foreground color
    /// </summary>
    public string DefaultInactiveForegroundColor
    {
      get
      {
        return (defaultInactiveForegroundColor);
      }
      set
      {
        defaultInactiveForegroundColor = value;

        if (!string.IsNullOrEmpty (defaultInactiveForegroundColor))
          GuiDefaultInactiveForegroundColor = GetColorFromHex (defaultInactiveForegroundColor);
      }
    }
        
    private Brush guiDefaultInactiveForegroundColor;

    /// <summary>
    /// Gui inactive foreground color
    /// </summary>
    public Brush GuiDefaultInactiveForegroundColor
    {
      get {
        return (guiDefaultInactiveForegroundColor);
      }
      set
      {
        guiDefaultInactiveForegroundColor = value;
        OnPropertyChanged ("GuiDefaultInactiveForegroundColor");
      }
    }

    private string defaultInactiveBackgroundColor;

    /// <summary>
    /// Default inactive background color
    /// </summary>
    public string DefaultInactiveBackgroundColor
    {
      get
      {
        return (defaultInactiveBackgroundColor);
      }
      set
      {
        defaultInactiveBackgroundColor = value;

        if (!string.IsNullOrEmpty (defaultInactiveBackgroundColor))
          GuiDefaultInactiveBackgroundColor = GetColorFromHex (defaultInactiveBackgroundColor);
      }
    }

    private Brush guiDefaultInactiveBackgroundColor;

    /// <summary>
    /// Gui inactive background color
    /// </summary>
    public Brush GuiDefaultInactiveBackgroundColor
    {
      get
      {
        return (guiDefaultInactiveBackgroundColor);
      }
      set
      {
        guiDefaultInactiveBackgroundColor = value;
        OnPropertyChanged ("GuiDefaultInactiveBackgroundColor");
      }
    }

    private string defaultFindHighlightForegroundColor;

    /// <summary>
    /// Default find highlight foreground color
    /// </summary>
    public string DefaultHighlightForegroundColor
    {
      get
      {
        return (defaultFindHighlightForegroundColor);
      }
      set
      {
        defaultFindHighlightForegroundColor = value;

        if (!string.IsNullOrEmpty (defaultFindHighlightForegroundColor))
          GuiDefaultHighlightForegroundColor = GetColorFromHex (defaultFindHighlightForegroundColor);
      }
    }

    private Brush guiDefaultHighlightForegroundColor;

    /// <summary>
    /// Gui find highlight foreground color
    /// </summary>
    public Brush GuiDefaultHighlightForegroundColor
    {
      get
      {
        return (guiDefaultHighlightForegroundColor);
      }
      set
      {
        guiDefaultHighlightForegroundColor = value;
        OnPropertyChanged ("GuiDefaultHighlightForegroundColor");
      }
    }

    private string defaultFindHighlightBackgroundColor;

    /// <summary>
    /// Default find highlight background color
    /// </summary>
    public string DefaultHighlightBackgroundColor
    {
      get
      {
        return (defaultFindHighlightBackgroundColor);
      }
      set
      {
        defaultFindHighlightBackgroundColor = value;

        if (!string.IsNullOrEmpty (defaultFindHighlightBackgroundColor))
          GuiDefaultHighlightBackgroundColor = GetColorFromHex (defaultFindHighlightBackgroundColor);
      }
    }

    private Brush guiDefaultHighlightBackgroundColor;

    /// <summary>
    /// Gui find highlight background color
    /// </summary>
    public Brush GuiDefaultHighlightBackgroundColor
    {
      get
      {
        return (guiDefaultHighlightBackgroundColor);
      }
      set
      {
        guiDefaultHighlightBackgroundColor = value;
        OnPropertyChanged ("GuiDefaultHighlightBackgroundColor");
      }
    }

    private string defaultLineNumbersColor;

    /// <summary>
    /// Default line numbers color
    /// </summary>
    public string DefaultLineNumbersColor
    {
      get
      {
        return (defaultLineNumbersColor);
      }
      set
      {
        defaultLineNumbersColor = value;

        if (!string.IsNullOrEmpty (defaultLineNumbersColor))
          GuiDefaultLineNumbersColor = GetColorFromHex (defaultLineNumbersColor);
      }
    }

    private Brush guiDefaultLineNumbersColor;

    /// <summary>
    /// Gui line numbers color
    /// </summary>
    public Brush GuiDefaultLineNumbersColor
    {
      get
      {
        return (guiDefaultLineNumbersColor);
      }
      set
      {
        guiDefaultLineNumbersColor = value;
        OnPropertyChanged ("GuiDefaultLineNumbersColor");
      }
    }

    private string defaultHighlightColor;

    /// <summary>
    /// Default highlight color
    /// </summary>
    public string DefaultHighlightColor
    {
      get
      {
        return (defaultHighlightColor);
      }
      set
      {
        defaultHighlightColor = value;

        if (!string.IsNullOrEmpty (defaultHighlightColor))
          GuiDefaultHighlightColor = GetColorFromHex (defaultHighlightColor);
      }
    }

    private Brush guiDefaultHighlightColor;

    /// <summary>
    /// Gui highlight color
    /// </summary>
    public Brush GuiDefaultHighlightColor
    {
      get
      {
        return (guiDefaultHighlightColor);
      }
      set
      {
        guiDefaultHighlightColor = value;
        OnPropertyChanged ("GuiDefaultHighlightColor");
      }
    }

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
    /// Alert settings
    /// </summary>
    public AlertData AlertSettings
    {
      get;
      set;
    }

    /// <summary>
    /// Enum File sort
    /// </summary>
    public enum EFileSort
    {
      /// <summary>
      /// File creation time
      /// </summary>
      FileCreationTime,

      /// <summary>
      /// No file sort
      /// </summary>
      Nothing
    }

    /// <summary>
    /// Enum RefreshRate
    /// </summary>
    public enum ETailRefreshRate
    {
      /// <summary>
      /// 1000 ms
      /// </summary>
      Normal = 1000,

      /// <summary>
      /// 200 ms
      /// </summary>
      Fast = 200,

      /// <summary>
      /// 50 ms
      /// </summary>
      Highest = 50
    }

    /// <summary>
    /// Enum time format
    /// </summary>
    public enum ETimeFormat
    {
      /// <summary>
      /// International time format like GB, USA ... hh:mm
      /// </summary>
      [Description ("hh:mm")]
      HHMMd,

      /// <summary>
      /// German standard time format HH:mm
      /// </summary>
      [Description ("HH:mm")]
      HHMMD,

      /// <summary>
      /// International time format like GB, USA with seconds hh:mm:ss
      /// </summary>
      [Description ("hh:mm:ss")]
      HHMMSSd,

      /// <summary>
      /// German standard time format with seconds HH:mm:ss
      /// </summary>
      [Description ("HH:mm:ss")]
      HHMMSSD
    }

    /// <summary>
    /// Enum date format
    /// </summary>
    public enum EDateFormat
    {
      /// <summary>
      /// Date format year - month - day
      /// </summary>
      [Description ("yyyy-MM-dd")]
      YYYYMMDD,

      /// <summary>
      /// Short date format year - month - day
      /// </summary>
      [Description ("yy-M-d")]
      YMDD,

      /// <summary>
      /// Date format day.month.year
      /// </summary>
      [Description ("dd.MM.yyyy")]
      DDMMYYYY,

      /// <summary>
      /// Short date format day.month.year
      /// </summary>
      [Description ("d.M.yy")]
      DMYY
    }

    /// <summary>
    /// Enum for FileManager state
    /// </summary>
    public enum EFileManagerState
    {
      /// <summary>
      /// Insert new file to FileManager
      /// </summary>
      AddFile,

      /// <summary>
      /// Open FileManager without new file
      /// </summary>
      OpenFileManager,

      /// <summary>
      /// Do something in FilterDialogue
      /// </summary>
      EditFilter,

      /// <summary>
      /// Edit selected item
      /// </summary>
      EditItem
    }

    /// <summary>
    /// Enum for Tab
    /// </summary>
    public enum ETabState
    {
      /// <summary>
      /// Normal tab with default settings
      /// </summary>
      NewTab,

      /// <summary>
      /// Tab with settings from FileManager
      /// </summary>
      FileManagerTab
    }

    /// <summary>
    /// Enum for color types
    /// </summary>
    public enum ETailLogColorTypes
    {
      /// <summary>
      /// Background color
      /// </summary>
      BackgroundColor,

      /// <summary>
      /// Foreground color
      /// </summary>
      ForegroundColor,

      /// <summary>
      /// Inactive background color
      /// </summary>
      InactiveBackgroundColor,

      /// <summary>
      /// Inactive foreground color
      /// </summary>
      InactiveForegroundColor,

      /// <summary>
      /// Find highlight background color
      /// </summary>
      FindHighlightBackgroundColor,

      /// <summary>
      /// Find highlight foreground color
      /// </summary>
      FindHighlightForegroundColor,

      /// <summary>
      /// Line numbers color
      /// </summary>
      LineNumbersColor,

      /// <summary>
      /// Highlight selection color
      /// </summary>
      HighlightColor
    }

    private Brush GetColorFromHex (string hex)
    {
      Color color = (Color) ColorConverter.ConvertFromString (hex);

      return (new SolidColorBrush (color));
    }
    
    /// <summary>
    /// Get description from enum
    /// </summary>
    /// <param name="value">Reference of enum</param>
    /// <returns>Description of enum</returns>
    public static string GetEnumDescription (Enum value)
    {
      FieldInfo fi = value.GetType ( ).GetField (value.ToString ( ));
      DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes (typeof (DescriptionAttribute), false);

      if (attributes != null && attributes.Length > 0)
        return (attributes[0].Description);
      else
        return (value.ToString ( ));
    }

    /// <summary>
    /// Get enum from description
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="description">Reference of description string</param>
    /// <returns>Enum type</returns>
    public static T GetDescriptionEnum<T> (string description)
    {
      var type = typeof (T);

      if (!type.IsEnum)
        throw new InvalidOperationException ( );

      foreach (var field in type.GetFields ( ))
      {
        var attribute = Attribute.GetCustomAttribute (field, typeof (DescriptionAttribute)) as DescriptionAttribute;
        
        if (attribute != null)
        {
          if (attribute.Description == description)
            return ((T) field.GetValue (null));
        }
        else
        {
          if (field.Name == description)
            return ((T) field.GetValue (null));
        }
      }
      throw new ArgumentException ("Not found.", "description");
      // or return default(T);
    }
  }
}
