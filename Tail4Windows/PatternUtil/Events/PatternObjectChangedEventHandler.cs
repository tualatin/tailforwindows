﻿namespace Org.Vs.TailForWin.PatternUtil.Events
{
  /// <summary>
  /// Pattern object changed event handler
  /// </summary>
  /// <param name="sender">Sender</param>
  /// <param name="pattern">Current pattern</param>
  /// <param name="isRegex">Is regex pattern</param>
  public delegate void PatternObjectChangedEventHandler (object sender, string pattern, bool isRegex);
}