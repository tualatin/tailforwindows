using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Data.Messages
{
  /// <summary>
  /// Open <see cref="FilterData"/> from <see cref="TailData"/>
  /// </summary>
  public class OpenFilterDataFromTailDataMessage
  {
    /// <summary>
    /// FilterPattern
    /// </summary>
    public string FilterPattern
    {
      get;
    }

    /// <summary>
    /// <see cref="TailData"/>
    /// </summary>
    public TailData TailData
    {
      get;
    }

    /// <summary>
    /// Who is sending the message
    /// </summary>
    public object Sender
    {
      get;
    }

    /// <summary>
    /// Standarc constructor
    /// </summary>
    /// <param name="sender">Who sends the message</param>
    /// <param name="tailData"><see cref="TailData"/></param>
    /// <param name="filterPattern">Optional filter pattern</param>
    public OpenFilterDataFromTailDataMessage(object sender, TailData tailData, string filterPattern = null)
    {
      Sender = sender;
      TailData = tailData;

      if ( !string.IsNullOrWhiteSpace(filterPattern) )
        FilterPattern = filterPattern;
    }
  }
}
