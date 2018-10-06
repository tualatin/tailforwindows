﻿using System.Globalization;
using System.Threading;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// App wide settings
  /// </summary>
  public partial class AppSettings : NotifyMaster
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public AppSettings()
    {
#if DEBUG
      DebugTailReader = false;
#endif
    }

    private CultureInfo _cultureInfo;

    /// <summary>
    /// Current culture info
    /// </summary>
    public CultureInfo CurrentCultureInfo
    {
      get => _cultureInfo ?? Thread.CurrentThread.CurrentUICulture;
      private set
      {
        if ( Equals(_cultureInfo, value) )
          return;

        _cultureInfo = value;
        Thread.CurrentThread.CurrentUICulture = value;
        Thread.CurrentThread.CurrentCulture = value;

        OnPropertyChanged(nameof(CurrentCultureInfo));
      }
    }

    /// <summary>
    /// Current application should close now
    /// </summary>
    public bool ShouldClose
    {
      get;
      set;
    }

    /// <summary>
    /// Use debug tail reader
    /// </summary>
    public bool DebugTailReader
    {
      get;
      set;
    }

    private bool _isUserSettings;

    /// <summary>
    /// Save settings in user roaming path or use it as portable app
    /// </summary>
    public bool IsUserSettings
    {
      get => _isUserSettings;
      set
      {
        if ( value == _isUserSettings )
          return;

        _isUserSettings = value;
        OnPropertyChanged();
      }
    }
  }
}
