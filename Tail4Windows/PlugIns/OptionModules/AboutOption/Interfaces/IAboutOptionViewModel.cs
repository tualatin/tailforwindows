using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.AboutOption.Data;
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
    /// Donate command
    /// </summary>
    ICommand DonateCommand
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

    /// <summary>
    /// ThirdPartyComponents view
    /// </summary>
    ListCollectionView ThirdPartyComponentsView
    {
      get;
    }

    /// <summary>
    /// Third party components
    /// </summary>
    ObservableCollection<ThirdPartyComponentData> ThirdPartyComponents
    {
      get;
    }
  }
}
