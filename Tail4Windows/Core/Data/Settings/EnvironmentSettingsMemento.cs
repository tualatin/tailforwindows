using System.Windows.Media;
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
      StatusBarInactiveBackgroundColor = memento.StatusBarInactiveBackgroundColor;
      StatusBarFileLoadedBackgroundColor = memento.StatusBarFileLoadedBackgroundColor;
      StatusBarTailBackgroundColor = memento.StatusBarTailBackgroundColor;
      ExitWithEscape = memento.ExitWithEscape;
      DeleteLogFiles = memento.DeleteLogFiles;
      CurrentWindowStyle = memento.CurrentWindowStyle;
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
        StatusBarInactiveBackgroundColor = obj.StatusBarInactiveBackgroundColor;
        StatusBarFileLoadedBackgroundColor = obj.StatusBarFileLoadedBackgroundColor;
        StatusBarTailBackgroundColor = obj.StatusBarTailBackgroundColor;
        ExitWithEscape = obj.ExitWithEscape;
        DeleteLogFiles = obj.DeleteLogFiles;
        CurrentWindowStyle = obj.CurrentWindowStyle;
      }

      #region Window settings

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

      #region StatusBar settings

      /// <summary>
      /// StatusBar inactive background color
      /// </summary>
      public Brush StatusBarInactiveBackgroundColor
      {
        get;
      }

      /// <summary>
      /// StatusBar file loaded background color
      /// </summary>
      public Brush StatusBarFileLoadedBackgroundColor
      {
        get;
      }

      /// <summary>
      /// StatusBar tail background color
      /// </summary>
      public Brush StatusBarTailBackgroundColor
      {
        get;
      }

      #endregion

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
    }
  }
}
