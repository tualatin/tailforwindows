using System.Threading;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Business.DbEngine.Interfaces
{
  /// <summary>
  /// Settings DataBase controller interface
  /// </summary>
  public interface ISettingsDbController
  {
    /// <summary>
    /// Read current DataBase settings
    /// </summary>
    /// <returns>Task</returns>
    Task ReadDbSettingsAsync();

    /// <summary>
    /// Updates FindResult DataBase settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task UpdateFindResultDbSettingsAsync(CancellationToken token);

    /// <summary>
    /// Updates FindDialog DataBase settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task UpdateFindDialogDbSettingsAsync(CancellationToken token);

    /// <summary>
    /// Updates password settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task UpdatePasswordSettingsAsync(CancellationToken token);

    /// <summary>
    /// Updates Bookmark overview DataBase settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    Task UpdateBookmarkOverviewDbSettingsAsync(CancellationToken token);

    /// <summary>
    /// Resets DataBase settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    Task ResetDbSettingsAsync(CancellationToken token);
  }
}
