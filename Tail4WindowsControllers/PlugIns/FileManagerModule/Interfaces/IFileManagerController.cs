using System;
using System.Collections.Generic;
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
    /// Add new tailData JSON file
    /// </summary>
    /// <param name="tailData"><see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> AddTailDataAsync(TailData tailData, CancellationToken token);

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
    /// <param name="tailData"><see cref="IReadOnlyCollection{T}"/> of <see cref="TailData"/></param>
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
    /// Updates a JSON file
    /// </summary>
    /// <param name="tailData"><see cref="ObservableCollection{T}"/> of <see cref="TailData"/></param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> CreateUpdateJsonFileAsync(ObservableCollection<TailData> tailData, CancellationToken token);
  }
}
