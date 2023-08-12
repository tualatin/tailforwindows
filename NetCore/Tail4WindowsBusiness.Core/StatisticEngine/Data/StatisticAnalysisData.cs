using Org.Vs.Tail4Win.Business.StatisticEngine.DbScheme;
using Org.Vs.Tail4Win.Core.Data.Base;

namespace Org.Vs.Tail4Win.Business.StatisticEngine.Data
{
  /// <summary>
  /// Statistic analysis data
  /// </summary>
  public class StatisticAnalysisData : NotifyMaster
  {
    private SessionEntity _sessionEntity;

    /// <summary>
    /// The <see cref="DbScheme.SessionEntity"/>
    /// </summary>
    public SessionEntity SessionEntity
    {
      get => _sessionEntity;
      set
      {
        _sessionEntity = value;
        OnPropertyChanged();
      }
    }

    private List<FileEntity> _files;

    /// <summary>
    /// <see cref="List{T}"/> of <see cref="FileEntity"/>
    /// </summary>
    public List<FileEntity> Files
    {
      get => _files;
      set
      {
        _files = value;
        OnPropertyChanged();
      }
    }
  }
}
