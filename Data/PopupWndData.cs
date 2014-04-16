namespace TailForWin.Data
{
  public class PopupWndData : INotifyMaster
  {
    private int openingMilliseconds;

    /// <summary>
    /// Popup window opening milliseconds
    /// </summary>
    public int OpeningMilliseconds
    {
      get
      {
        return (openingMilliseconds);
      }
      set
      {
        openingMilliseconds = value;
        OnPropertyChanged ("OpeningMilliseconds");
      }
    }

    private int stayOpenMilliseconds;

    /// <summary>
    /// Popup window stayOpen milliseconds
    /// </summary>
    public int StayOpenMilliseconds
    {
      get
      {
        return (stayOpenMilliseconds);
      }
      set
      {
        stayOpenMilliseconds = value;
        OnPropertyChanged ("StayOpenMilliseconds");
      }
    }

    private int hidingMilliseconds;

    /// <summary>
    /// Popup window hiding milliseconds
    /// </summary>
    public int HidingMilliseconds
    {
      get
      {
        return (hidingMilliseconds);
      }
      set
      {
        hidingMilliseconds = value;
        OnPropertyChanged ("HidingMilliseconds");
      }
    }
  }
}
