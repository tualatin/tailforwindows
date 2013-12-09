﻿using System.ComponentModel;
using System.Collections.Specialized;


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

    protected void ItemPropertyChanged (object sender, PropertyChangedEventArgs e)
    {
      NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset);
    }
  }
}
