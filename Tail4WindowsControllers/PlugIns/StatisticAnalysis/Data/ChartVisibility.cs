using System.Windows;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Controllers.PlugIns.StatisticAnalysis.Data
{
  /// <summary>
  /// Chart visibility
  /// </summary>
  public class ChartVisibility : NotifyMaster
  {
    private Visibility _visibility;

    /// <summary>
    /// <see cref="System.Windows.Visibility"/>
    /// </summary>
    public Visibility Visibility
    {
      get => _visibility;
      set
      {
        if ( value == _visibility )
          return;

        _visibility = value;
        OnPropertyChanged();
      }
    }

    private string _title;

    /// <summary>
    /// Title
    /// </summary>
    public string Title
    {
      get => _title;
      set
      {
        if (Equals(value, _title))
          return;

        _title = value;
        OnPropertyChanged();
      }
    }
  }
}
