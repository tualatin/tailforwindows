using Org.Vs.TailForWin.Core.Data.Base;

namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Update data object
  /// </summary>
  public class UpdateData : NotifyMaster
  {
    private Version _applicationVersion;

    /// <summary>
    /// Current application version
    /// </summary>
    public Version ApplicationVersion
    {
      get => _applicationVersion;
      set
      {
        if ( value == _applicationVersion )
          return;

        _applicationVersion = value;
        OnPropertyChanged();
      }
    }

    private Version _webVersion;

    /// <summary>
    /// WebVersion
    /// </summary>
    public Version WebVersion
    {
      get => _webVersion;
      set
      {
        if (value == _webVersion)
          return;

        _webVersion = value;
        OnPropertyChanged();
      }
    }

    private bool _update;

    /// <summary>
    /// Update
    /// </summary>
    public bool Update
    {
      get => _update;
      set
      {
        if ( value == _update )
          return;

        _update = value;
        OnPropertyChanged();
      }
    }
  }
}
