using System.ComponentModel;


namespace TailForWin.Data
{
  /// <summary>
  /// Implementation for INotifyPropertyChanged
  /// </summary>
  public class INotifyMaster: INotifyPropertyChanged
  {
    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;


    protected void OnPropertyChanged (string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;

      if (handler != null)
        handler (this, new PropertyChangedEventArgs (name));
    }
  }
}
