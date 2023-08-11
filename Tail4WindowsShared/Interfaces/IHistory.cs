namespace Org.Vs.Tail4Win.Shared.Interfaces
{
  /// <summary>
  /// JSON history interface
  /// </summary>
  /// <typeparam name="T">Type of interface</typeparam>
  public interface IHistory<T> where T : class
  {
    /// <summary>
    /// Converts old XML history file to JSON
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> ConvertXmlToJsonFileAsync(CancellationToken token);

    /// <summary>
    /// Reads history file
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="HistoryData"/></returns>
    Task<T> ReadHistoryAsync(CancellationToken token);

    /// <summary>
    /// Updates history file
    /// </summary>
    /// <param name="data">Data to update</param>
    /// <param name="searchText">Text to save</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> UpdateHistoryAsync(T data, string searchText, CancellationToken token);

    /// <summary>
    /// Deletes the history
    /// </summary>
    /// <param name="data">Data to delete</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    Task<bool> DeleteHistoryAsync(T data, CancellationToken token);
  }
}
