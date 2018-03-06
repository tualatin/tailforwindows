using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.UserControls.Interfaces;


namespace Org.Vs.TailForWin.UI.ViewModels
{
  /// <summary>
  /// Auto update view model
  /// </summary>
  public class AutoUpdateViewModel : NotifyMaster, IUpdateControlViewModel
  {
    private string _applicationVersion;

    /// <summary>
    /// Current application version
    /// </summary>
    public string ApplicationVersion
    {
      get => _applicationVersion;
      set
      {
        if ( Equals(_applicationVersion, value) )
          return;

        _applicationVersion = value;
        OnPropertyChanged(nameof(ApplicationVersion));
      }
    }

    private string _webVersion;

    /// <summary>
    /// Current web version
    /// </summary>
    public string WebVersion
    {
      get => _webVersion;
      set
      {
        if ( Equals(_webVersion, value) )
          return;

        _webVersion = value;
        OnPropertyChanged(nameof(WebVersion));
      }
    }

    private string _updateHint;

    /// <summary>
    /// Update hint text
    /// </summary>
    public string UpdateHint
    {
      get => _updateHint;
      set
      {
        if ( Equals(_updateHint, value) )
          return;

        _updateHint = value;
        OnPropertyChanged(nameof(UpdateHint));
      }
    }
  }
}
