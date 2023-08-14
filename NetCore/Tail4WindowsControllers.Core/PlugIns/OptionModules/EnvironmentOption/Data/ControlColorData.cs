using Org.Vs.TailForWin.Core.Data.Base;

namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Data
{
  /// <summary>
  /// Control color data
  /// </summary>
  public class ControlColorData : NotifyMaster
  {
    /// <summary>
    /// Configuration name
    /// </summary>
    public string ConfigurationName
    {
      get;
      set;
    }

    private string _name;

    /// <summary>
    /// Name of control
    /// </summary>
    public string Name
    {
      get => _name;
      set
      {
        if ( Equals(value, _name) )
          return;

        _name = value;
        OnPropertyChanged();
      }
    }

    private string _color;

    /// <summary>
    /// Color of control
    /// </summary>
    public string Color
    {
      get => _color;
      set
      {
        if ( Equals(value, _color) )
          return;

        _color = value;
        OnPropertyChanged();
      }
    }
  }
}
