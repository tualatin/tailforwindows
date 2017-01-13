using System.Collections.Specialized;
using System.ComponentModel;


namespace Org.Vs.TailForWin.Data.Base
{
  /// <summary>
  /// Implementation for INotifyPropertyChanged
  /// </summary>
  public class INotifyMaster : INotifyPropertyChanged
  {
    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;


    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    protected void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;

      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// ItemPropertyChanged
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected static void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
  }
}
