using System;


namespace Org.Vs.TailForWin.Business.BookmarkEngine.Events.Args
{
  /// <summary>
  /// IdChanged event args
  /// </summary>
  public class IdChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Current Window Id <see cref="Guid"/>
    /// </summary>
    public Guid WindowId
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="id"><see cref="Guid"/></param>
    public IdChangedEventArgs(Guid id) => WindowId = id;
  }
}
