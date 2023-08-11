using Org.Vs.Tail4Win.Shared.Data;

namespace Org.Vs.Tail4Win.Shared.Interfaces
{
  /// <summary>
  /// Update interface
  /// </summary>
  public interface IUpdater
  {
    /// <summary>
    /// Do check if main application needs to update
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <param name="version">Current main application version</param>
    /// <returns>Should update <c>True</c> otherwise <c>False</c></returns>
    Task<UpdateData> UpdateNecessaryAsync(CancellationToken token, Version version);
  }
}
