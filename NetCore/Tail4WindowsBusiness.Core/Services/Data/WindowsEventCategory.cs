using Org.Vs.TailForWin.Core.Data.Base;

namespace Org.Vs.TailForWin.Business.Services.Data
{
  /// <summary>
  /// Windows event category data object
  /// </summary>
  public class WindowsEventCategory : NotifyMaster
  {
    private string _category;

    /// <summary>
    /// Category
    /// </summary>
    public string Category
    {
      get => _category;
      set
      {
        if (Equals(value, _category))
          return;

        _category = value;
        OnPropertyChanged();
      }
    }

    private string _log;

    /// <summary>
    /// Log
    /// </summary>
    public string Log
    {
      get => _log;
      set
      {
        if (Equals(value, _log))
          return;

        _log = value;
        OnPropertyChanged();
      }
    }

    private string _logDisplayName;

    /// <summary>
    /// Log display name
    /// </summary>
    public string LogDisplayName
    {
      get => _logDisplayName;
      set
      {
        if (Equals(value, _logDisplayName))
          return;

        _logDisplayName = value;
        OnPropertyChanged();
      }
    }
  }
}
