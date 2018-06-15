using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.PatternModule.ViewModels
{
  /// <summary>
  /// PatternControl view model
  /// </summary>
  public class PatternControlViewModel : NotifyMaster
  {
    #region Properties

    private string _logFile;

    /// <summary>
    /// Current log file
    /// </summary>
    public string LogFile
    {
      get => _logFile;
      set
      {
        if ( Equals(value, _logFile) )
          return;

        _logFile = value;
        OnPropertyChanged();
      }
    }


    #endregion
  }
}
