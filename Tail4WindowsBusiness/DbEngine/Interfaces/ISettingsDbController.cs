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
    void ReadDbSettings();

    /// <summary>
    /// Updates FindResult DataBase settings
    /// </summary>
    void UpdateFindResultDbSettings();

    /// <summary>
    /// Updates FindDialog DataBase settings
    /// </summary>
    void UpdateFindDialogDbSettings();
  }
}
