using System.Collections.ObjectModel;
using Org.Vs.Tail4Win.Core.Data;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.OptionModules.GlobalHighlightModule.Interfaces
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
    /// <returns><c>True</c> if successfully deleted, otherwise <c>False</c></returns>
    Task<bool> UpdateGlobalFilterAsync(ObservableCollection<FilterData> items);

    /// <summary>
    /// Loads filters async
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>A <see cref="ObservableCollection{T}"/> of <see cref="FilterData"/></returns>
    Task<ObservableCollection<FilterData>> ReadGlobalFiltersAsync(CancellationToken token);

    /// <summary>
    /// Deletes a filter async
    /// </summary>
    /// <param name="id">ID of filter to delete</param>
    /// <returns><c>True</c> if successfully deleted, otherwise <c>False</c></returns>
    Task<bool> DeleteGlobalFilterAsync(Guid id);
  }
}
