using System.Globalization;
using Org.Vs.Tail4Win.Core.Data.Base;

namespace Org.Vs.Tail4Win.Core.Data.Settings
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

        OnPropertyChanged();
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

    private bool _isPortable;

    /// <summary>
    /// Save settings in user roaming path or use it as portable app
    /// </summary>
    public bool IsPortable
    {
      get => _isPortable;
      set
      {
        if ( value == _isPortable )
          return;

        _isPortable = value;
        OnPropertyChanged();
      }
    }
  }
}
