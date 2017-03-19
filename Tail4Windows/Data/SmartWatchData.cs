using Org.Vs.TailForWin.Data.Base;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// SmartWatch object
  /// </summary>
  public class SmartWatchData : INotifyMaster
  {
    private bool filterByExtension;

    /// <summary>
    /// Filter new files by extension
    /// </summary>
    public bool FilterByExtension
    {
      get => filterByExtension;
      set
      {
        filterByExtension = value;
        OnPropertyChanged("FilterByExtension");
      }
    }

    private ESmartWatchMode mode;

    /// <summary>
    /// SmartWatch mode
    /// </summary>
    public ESmartWatchMode Mode
    {
      get => mode;
      set
      {
        mode = value;
        OnPropertyChanged("Mode");
      }
    }

    private bool newTab;

    /// <summary>
    /// Open in new tab
    /// </summary>
    public bool NewTab
    {
      get => newTab;
      set
      {
        newTab = value;
        OnPropertyChanged("NewTab");
      }
    }
  }
}
