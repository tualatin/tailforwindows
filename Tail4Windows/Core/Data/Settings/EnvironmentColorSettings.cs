using System;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Environment color settings
  /// </summary>
  public class EnvironmentColorSettings : NotifyMaster, ICloneable
  {
    #region StatusBar settings

    private Brush _statusBarInactiveBackgroundColor;

    /// <summary>
    /// StatusBar inactive background color
    /// </summary>
    public Brush StatusBarInactiveBackgroundColor
    {
      get => _statusBarInactiveBackgroundColor;
      set
      {
        if ( Equals(value, _statusBarInactiveBackgroundColor) )
          return;

        _statusBarInactiveBackgroundColor = value;
        OnPropertyChanged(nameof(StatusBarInactiveBackgroundColor));
      }
    }

    private Brush _statusBarFileLoadedBackgroundColor;

    /// <summary>
    /// StatusBar file loaded background color
    /// </summary>
    public Brush StatusBarFileLoadedBackgroundColor
    {
      get => _statusBarFileLoadedBackgroundColor;
      set
      {
        if ( Equals(value, _statusBarFileLoadedBackgroundColor) )
          return;

        _statusBarFileLoadedBackgroundColor = value;
        OnPropertyChanged(nameof(StatusBarFileLoadedBackgroundColor));
      }
    }

    private Brush _statusBarTailBackgroundColor;

    /// <summary>
    /// StatusBar tail background color
    /// </summary>
    public Brush StatusBarTailBackgroundColor
    {
      get => _statusBarTailBackgroundColor;
      set
      {
        if ( Equals(value, _statusBarTailBackgroundColor) )
          return;

        _statusBarTailBackgroundColor = value;
        OnPropertyChanged(nameof(StatusBarTailBackgroundColor));
      }
    }

    #endregion

    /// <summary>
    /// Clone the object
    /// </summary>
    /// <returns>Cloned object</returns>
    public object Clone() => MemberwiseClone();
  }
}
