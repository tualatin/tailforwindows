using System;


namespace Org.Vs.TailForWin.Business.StatisticEngine.DbScheme
{
  /// <summary>
  /// File date entity
  /// </summary>
  public class FileDateEntity
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
    /// <see cref="FileEntity"/>
    /// </summary>
    public FileEntity File
    {
      get;
      set;
    }

    /// <summary>
    /// Date
    /// </summary>
    public DateTime Date
    {
      get;
      set;
    }
  }
}
