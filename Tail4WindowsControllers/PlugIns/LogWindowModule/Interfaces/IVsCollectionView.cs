using System.Collections.ObjectModel;
using System.ComponentModel;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.TailForWin.Controllers.PlugIns.LogWindowModule.Interfaces
{
  /// <summary>
  /// Virtual Studios Collection view interface
  /// </summary>
  public interface IVsCollectionView<T> : INotifyPropertyChanged
  {
    #region Properties

    /// <summary>
    /// <see cref="ObservableCollection{T}"/>
    /// </summary>
    AsyncObservableCollection<T> Collection
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Clears collections
    /// </summary>
    void Clear();

    /// <summary>
    /// Release all resources used by current collection
    /// </summary>
    void Dispose();
  }
}
