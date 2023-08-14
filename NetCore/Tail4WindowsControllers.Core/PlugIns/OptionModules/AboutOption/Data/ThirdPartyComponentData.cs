using Org.Vs.TailForWin.Core.Data.Base;

namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Data
{
  /// <summary>
  /// Third party component data
  /// </summary>
  public class ThirdPartyComponentData : NotifyMaster
  {
    private string _componentName;

    /// <summary>
    /// Component name
    /// </summary>
    public string ComponentName
    {
      get => _componentName;
      set
      {
        if ( Equals(value, _componentName) )
          return;

        _componentName = value;
        OnPropertyChanged();
      }
    }

    private string _componentVersion;

    /// <summary>
    /// Version
    /// </summary>
    public string ComponentVersion
    {
      get => _componentVersion;
      set
      {
        if ( Equals(value, _componentVersion) )
          return;

        _componentVersion = value;
        OnPropertyChanged();
      }
    }

    private string _description;

    /// <summary>
    /// Description
    /// </summary>
    public string Description
    {
      get => _description;
      set
      {
        if (Equals(value, _description))
          return;

        _description = value;
        OnPropertyChanged();
      }
    }
  }
}
