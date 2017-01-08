using System;
using System.ComponentModel;
using System.Windows;


namespace Org.Vs.TailForWin.Template.TextEditor.Data
{
  public class PropertyChangedBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      Application.Current.Dispatcher.BeginInvoke((Action)(() =>
    {
      PropertyChangedEventHandler handler = PropertyChanged;

      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }));
    }
  }
}
