namespace Org.Vs.TailForWin.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// Key value object entity
  /// </summary>
  public class KeyValueObjectEntity
  {
    /// <summary>
    /// Id
    /// </summary>
    public int Id
    {
      get;
      set;
    }

    /// <summary>
    /// <see cref="KeyValueEntity"/>
    /// </summary>
    public KeyValueEntity KeyValue
    {
      get;
      set;
    }

    /// <summary>
    /// Dummy value
    /// </summary>
    public object Object
    {
      get;
      set;
    }
  }
}
