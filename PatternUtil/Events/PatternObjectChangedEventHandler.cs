using System.Collections.Generic;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.PatternUtil.Events
{
  /// <summary>
  /// Pattern object changed event handler
  /// </summary>
  /// <param name="sender">Sender</param>
  /// <param name="pattern">Current pattern</param>
  public delegate void PatternObjectChangedEventHandler (object sender, List<SearchPatter> pattern);
}
