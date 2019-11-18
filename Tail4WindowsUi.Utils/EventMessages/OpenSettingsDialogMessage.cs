namespace Org.Vs.TailForWin.Ui.Utils.EventMessages
{
  /// <summary>
  /// Open settings dialog message
  /// </summary>
  public class OpenSettingsDialogMessage
  {
    /// <summary>
    /// Open settings dialog
    /// </summary>
    public bool OpenSettings
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="openSettings">Open settings</param>
    public OpenSettingsDialogMessage(bool openSettings) => OpenSettings = openSettings;
  }
}
