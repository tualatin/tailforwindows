using System;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// SmartWatch settings
  /// </summary>
  public class SmartWatchSetting : NotifyMaster, ICloneable
  {
    private bool _filterByExtension;

    /// <summary>
    /// Filter new files by extension
    /// </summary>
    public bool FilterByExtension
    {
      get => _filterByExtension;
      set
      {
        if ( _filterByExtension == value )
          return;

        _filterByExtension = value;
        OnPropertyChanged(nameof(FilterByExtension));
      }
    }

    private ESmartWatchMode _mode;

    /// <summary>
    /// SmartWatch mode
    /// </summary>
    public ESmartWatchMode Mode
    {
      get => _mode;
      set
      {
        if ( _mode == value )
          return;

        _mode = value;
        OnPropertyChanged(nameof(Mode));
      }
    }

    private bool _newTab;

    /// <summary>
    /// Open in new tab
    /// </summary>
    public bool NewTab
    {
      get => _newTab;
      set
      {
        if ( _newTab == value )
          return;

        _newTab = value;
        OnPropertyChanged(nameof(NewTab));
      }
    }

    private bool _autoRun;

    /// <summary>
    /// AutoRun mode of SmartWatch
    /// </summary>
    public bool AutoRun
    {
      get => _autoRun;
      set
      {
        if ( _autoRun == value )
          return;

        _autoRun = value;
        OnPropertyChanged(nameof(AutoRun));
      }
    }

    /// <summary>
    /// Clone the object
    /// </summary>
    /// <returns>Cloned object</returns>
    public object Clone() => MemberwiseClone();
  }
}
