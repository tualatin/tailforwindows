using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces
{
  /// <summary>
  /// FileManager controller interface
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
    /// Reads a JSON file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></returns>
    Task<ObservableCollection<TailData>> ReadJsonFileAsync(CancellationToken token);

    /// <summary>
    /// Gets a list of categories from JSON file
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="ObservableCollection{T}"/> of <see cref="string"/></returns>
    Task<ObservableCollection<string>> GetCategoriesAsync(ObservableCollection<TailData> tailData, CancellationToken token);

    /// <summary>
    /// Get <c><see cref="TailData"/></c> by certain Id
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="id">Id</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><c><see cref="TailData"/></c>, otherwise <c>Null</c></returns>
    Task<TailData> GetTailDataByIdAsync(ObservableCollection<TailData> tailData, Guid id, CancellationToken token);

    /// <summary>
    /// Updates a JSON file
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> CreateUpdateJsonFileAsync(ObservableCollection<TailData> tailData, CancellationToken token);
  }
}
