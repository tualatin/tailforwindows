using System.Windows;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.LogWindowModule.Events.Args
{
  /// <summary>
  /// Selected lines changed event args
  /// </summary>
  public class SelectedLinesChangedArgs : RoutedEventArgs
  {
    /// <summary>
    /// Current selected lines
    /// </summary>
    public int SelectedLines
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="routedEvent"><see cref="RoutedEvent"/></param>
    /// <param name="selectedLines">Current selected lines</param>
    public SelectedLinesChangedArgs(RoutedEvent routedEvent, int selectedLines)
    : base(routedEvent) => SelectedLines = selectedLines;
  }
}
