using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Global filter controller interface
  /// </summary>
  public interface IGlobalFilterController
  {
    /// <summary>
    /// Save filter async
    /// </summary>
    /// <param name="items">Global filters to save</param>
    Task SaveFilterAsync(ObservableCollection<FilterData> items);

    /// <summary>
    /// Loads filters async
    /// </summary>
    /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="FilterData"/></returns>
    Task<ObservableCollection<FilterData>> LoadFiltersAsync();

    /// <summary>
    /// Deletes a filter async
    /// </summary>
    /// <param name="id">ID of filter to delete</param>
    /// <returns><c>True</c> if successfully deleted, otherwise <c>False</c></returns>
    Task<bool> DeleteFilterAsync(Guid id);
  }
}
