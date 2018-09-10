using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Async <see cref="ObservableCollection{T}"/>
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class AsyncObservableCollection<T> : ObservableCollection<T>
  {
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public AsyncObservableCollection()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="list">The collection from which the elements are copied.</param>
    public AsyncObservableCollection(IEnumerable<T> list)
      : base(list)
    {
    }

    /// <summary>
    /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the provided arguments.
    /// </summary>
    /// <param name="e">Arguments of the event being raised.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if ( SynchronizationContext.Current == _synchronizationContext )
      {
        // Execute the CollectionChanged event on the current thread
        RaiseCollectionChanged(e);
      }
      else
      {
        // Raises the CollectionChanged event on the creator thread
        _synchronizationContext.Send(RaiseCollectionChanged, e);
      }
    }

    /// <summary>
    /// We are in the creator thread, call the base implementation directly
    /// </summary>
    /// <param name="param">Parameter</param>
    private void RaiseCollectionChanged(object param) => base.OnCollectionChanged((NotifyCollectionChangedEventArgs) param);

    /// <summary>
    /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged" /> event with the provided arguments.
    /// </summary>
    /// <param name="e">Arguments of the event being raised.</param>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if ( SynchronizationContext.Current == _synchronizationContext )
      {
        // Execute the PropertyChanged event on the current thread
        RaisePropertyChanged(e);
      }
      else
      {
        // Raises the PropertyChanged event on the creator thread
        _synchronizationContext.Send(RaisePropertyChanged, e);
      }
    }

    /// <summary>
    /// We are in the creator thread, call the base implementation directly
    /// </summary>
    /// <param name="param">Parameter</param>
    private void RaisePropertyChanged(object param) => base.OnPropertyChanged((PropertyChangedEventArgs) param);
  }
}
