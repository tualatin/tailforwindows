using System.Windows.Input;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.AboutOption.Interfaces
{
  /// <summary>
  /// About option view model interface
  /// </summary>
  public interface IAboutOptionViewModel : IViewModelBase
  {
    /// <summary>
    /// Author
    /// </summary>
    string Author
    {
      get;
      set;
    }

    /// <summary>
    /// Build date
    /// </summary>
    string BuildDate
    {
      get;
      set;
    }

    /// <summary>
    /// Request navigate command
    /// </summary>
    ICommand RequestNavigateCommand
    {
      get;
    }

    /// <summary>
    /// Uptime
    /// </summary>
    string UpTime
    {
      get;
      set;
    }

    /// <summary>
    /// Current version
    /// </summary>
    string Version
    {
      get;
      set;
    }
  }
}
