using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Data.Settings
{
  /// <summary>
  /// SmartWatch settings
  /// </summary>
  public class SmartWatchSetting : NotifyMaster, ICloneable, IPropertyNotify
  {
    private int _smartWatchInterval;

    /// <summary>
    /// SmartWatch interval
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SmartWatchInterval)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int SmartWatchInterval
    {
      get => _smartWatchInterval;
      set
      {
        if ( Equals(value, _smartWatchInterval) )
          return;

        _smartWatchInterval = value;
        OnPropertyChanged();
      }
    }

    private bool _filterByExtension;

    /// <summary>
    /// Filter new files by extension
    /// </summary>
    [DefaultValue(DefaultEnvironmentSettings.SmartWatchFilterByExension)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [DefaultValue(DefaultEnvironmentSettings.SmartWatchMode)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [DefaultValue(DefaultEnvironmentSettings.SmartWatchNewTab)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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
    [DefaultValue(DefaultEnvironmentSettings.SmartWatchAutoRun)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
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

    /// <summary>
    /// Call OnPropertyChanged
    /// </summary>
    public void RaiseOnPropertyChanged() => OnPropertyChanged();
  }
}
