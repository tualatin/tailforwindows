using System.Collections.ObjectModel;
using Org.Vs.Tail4Win.Core.Data;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// FileManager (for JSON use only) controller interface
  /// </summary>
  public interface IFileManagerController
  {
    /// <summary>
    /// Converts old XML config file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ConvertXmlToJsonConfigAsync(CancellationToken token);

    /// <summary>
    /// Add new tailData JSON file
    /// </summary>
    /// <param name="item"><see cref="TailData"/> to add</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> AddTailDataAsync(TailData item, CancellationToken token, ObservableCollection<TailData> tailData = null);

    /// <summary>
    /// Reads a JSON file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></returns>
    Task<ObservableCollection<TailData>> ReadJsonFileAsync(CancellationToken token);

    /// <summary>
    /// Gets a list of categories from JSON file
    /// <para>Optional you can insert a list of <see cref="IReadOnlyCollection{T}"/> of <see cref="TailData"/></para>
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="IReadOnlyCollection{T}"/> of <see cref="TailData"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="string"/></returns>
    Task<ObservableCollection<string>> GetCategoriesAsync(CancellationToken token, IReadOnlyCollection<TailData> tailData = null);

    /// <summary>
    /// Get <c><see cref="TailData"/></c> by certain Id
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="id">Id</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><c><see cref="TailData"/></c>, otherwise <c>Null</c></returns>
    Task<TailData> GetTailDataByIdAsync(ObservableCollection<TailData> tailData, Guid id, CancellationToken token);

    /// <summary>
    /// Updates a <see cref="TailData"/> item
    /// </summary>
    /// <param name="item"><see cref="TailData"/> to update</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="ObservableCollection{T}"/> if <see cref="TailData"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> UpdateTailDataAsync(TailData item, CancellationToken token, ObservableCollection<TailData> tailData = null);
 
    /// <summary>
    /// Updates a JSON file
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> CreateUpdateJsonFileAsync(ObservableCollection<TailData> tailData, CancellationToken token);

    /// <summary>
    /// Deletes a <see cref="TailData"/> item by his Id
    /// </summary>
    /// <param name="id">Id of item to delete</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="tailData">Optional <see cref="ObservableCollection{T}"/> if <see cref="TailData"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> DeleteTailDataByIdAsync(Guid id, CancellationToken token, ObservableCollection<TailData> tailData = null);

    /// <summary>
    /// Deletes a filter entry <see cref="FilterData"/>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <param name="tailData"></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> DeleteFilterDataByIdAsync(Guid id, CancellationToken token, ObservableCollection<TailData> tailData = null);
  }
}
