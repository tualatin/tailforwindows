using System;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// Environment color settings
  /// </summary>
  public class EnvironmentColorSettings : NotifyMaster, ICloneable, IPropertyNotify
  {
    #region StatusBar colors

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

    #region TailLog viewer colors

    private string _foregroundColorHex;

    /// <summary>
    /// Log viewer foreground color
    /// </summary>
    public string ForegroundColorHex
    {
      get => _foregroundColorHex;
      set
      {
        if ( Equals(value, _foregroundColorHex) )
          return;

        _foregroundColorHex = value;
        OnPropertyChanged(nameof(ForegroundColorHex));
      }
    }

    private string _backgroundColorHex;

    /// <summary>
    /// Log viewer background color
    /// </summary>
    public string BackgroundColorHex
    {
      get => _backgroundColorHex;
      set
      {
        if ( Equals(value, _backgroundColorHex) )
          return;

        _backgroundColorHex = value;
        OnPropertyChanged(nameof(BackgroundColorHex));
      }
    }

    #endregion

    #region Find highlight colors

    private string _findHighlightForegroundColorHex;

    /// <summary>
    /// Find highlight foreground color
    /// </summary>
    public string FindHighlightForegroundColorHex
    {
      get => _findHighlightForegroundColorHex;
      set
      {
        if ( Equals(value, _findHighlightForegroundColorHex) )
          return;

        _findHighlightForegroundColorHex = value;
        OnPropertyChanged(nameof(FindHighlightForegroundColorHex));
      }
    }

    private string _findHighlightBackgroundColorHex;

    /// <summary>
    /// Find highlight background color
    /// </summary>
    public string FindHighlightBackgroundColorHex
    {
      get => _findHighlightBackgroundColorHex;
      set
      {
        if ( Equals(value, _findHighlightBackgroundColorHex) )
          return;

        _findHighlightBackgroundColorHex = value;
        OnPropertyChanged(nameof(FindHighlightBackgroundColorHex));
      }
    }

    #endregion

    #region Line number colors

    private string _lineNumberColorHex;

    /// <summary>
    /// Line number color
    /// </summary>
    public string LineNumberColorHex
    {
      get => _lineNumberColorHex;
      set
      {
        if ( Equals(value, _lineNumberColorHex) )
          return;

        _lineNumberColorHex = value;
        OnPropertyChanged(nameof(LineNumberColorHex));
      }
    }

    private string _lineNumberHighlightColorHex;

    /// <summary>
    /// Line number highlight color
    /// </summary>
    public string LineNumberHighlightColorHex
    {
      get => _lineNumberHighlightColorHex;
      set
      {
        if ( Equals(value, _lineNumberHighlightColorHex) )
          return;

        _lineNumberHighlightColorHex = value;
        OnPropertyChanged(nameof(LineNumberHighlightColorHex));
      }
    }

    #endregion

    private string _splitterBackgroundColorHex;

    /// <summary>
    /// Splitter background color hex
    /// </summary>
    public string SplitterBackgroundColorHex
    {
      get => _splitterBackgroundColorHex;
      set
      {
        if ( Equals(value, _backgroundColorHex) )
          return;

        _splitterBackgroundColorHex = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Clone the object
    /// </summary>
    /// <returns>Cloned object</returns>
    public object Clone() => MemberwiseClone();

    /// <summary>
    /// Call OnPropertyChanged
    /// </summary>
    public void RaiseOnPropertyChanged() => OnPropertyChanged();
  }
}
