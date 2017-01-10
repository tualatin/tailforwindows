using System;
using System.ComponentModel;
using System.Windows;


namespace Org.Vs.TailForWin.Template.TextEditor.Data
{
  /// <summary>
  /// PropertyChangedBasse
  /// </summary>
  public class PropertyChangedBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      Application.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        PropertyChangedEventHandler handler = PropertyChanged;

        handler?.Invoke(this, new ProgressChangedEventArgs(propertyName));
      }));
    }
  }
}
