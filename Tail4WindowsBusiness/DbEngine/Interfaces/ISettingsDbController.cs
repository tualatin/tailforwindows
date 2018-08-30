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
    void UpdateFindResultDbSettings();

    /// <summary>
    /// Updates FindDialog DataBase settings
    /// </summary>
    void UpdateFindDialogDbSettings();

    /// <summary>
    /// Updates password settings
    /// </summary>
    void UpdatePasswordSettings();

    /// <summary>
    /// Updates Bookmark overview DataBase settings
    /// </summary>
    void UpdateBookmarkOverviewDbSettings();

    /// <summary>
    /// Resets DataBase settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    Task ResetDbSettingsAsync(CancellationToken token);
  }
}
