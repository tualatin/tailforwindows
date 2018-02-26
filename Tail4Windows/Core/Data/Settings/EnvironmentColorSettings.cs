using System;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Environment color settings
  /// </summary>
  public class EnvironmentColorSettings : NotifyMaster, ICloneable
  {
    #region StatusBar settings

    private string _statusBarInactiveBackgroundColorHex;

    /// <summary>
    /// StatusBar inactive background color as hex string
    /// </summary>
    public string StatusBarInactiveBackgroundColorHex
    {
      get => _statusBarInactiveBackgroundColorHex;
      set
      {
        if ( Equals(value, _statusBarInactiveBackgroundColorHex) )
          return;

        _statusBarInactiveBackgroundColorHex = value;
        OnPropertyChanged(nameof(StatusBarInactiveBackgroundColorHex));
      }
    }

    private string _statusBarFileLoadedBackgroundColorHex;

    /// <summary>
    /// StatusBar file loaded background color as hex string
    /// </summary>
    public string StatusBarFileLoadedBackgroundColorHex
    {
      get => _statusBarFileLoadedBackgroundColorHex;
      set
      {
        if ( Equals(value, _statusBarFileLoadedBackgroundColorHex) )
          return;

        _statusBarFileLoadedBackgroundColorHex = value;
        OnPropertyChanged(nameof(StatusBarFileLoadedBackgroundColorHex));
      }
    }

    private string _statusBarTailBackgroundColorHex;

    /// <summary>
    /// StatusBar tail background color
    /// </summary>
    public string StatusBarTailBackgroundColorHex
    {
      get => _statusBarTailBackgroundColorHex;
      set
      {
        if ( Equals(value, _statusBarTailBackgroundColorHex) )
          return;

        _statusBarTailBackgroundColorHex = value;
        OnPropertyChanged(nameof(StatusBarTailBackgroundColorHex));
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
