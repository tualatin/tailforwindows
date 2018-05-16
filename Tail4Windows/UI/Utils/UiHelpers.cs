using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// UI helpers
  /// </summary>
  public static class UiHelpers
  {
    /// <summary>
    /// Get horizontal scrollbar grid
    /// </summary>
    /// <param name="scrollViewer"><see cref="DependencyObject"/></param>
    /// <returns><see cref="Grid"/> horizontal scrollbar grid</returns>
    public static Grid GetHorizontalScrollBarGrid(DependencyObject scrollViewer)
    {
      if ( scrollViewer == null )
        return null;

      var scrollBars = scrollViewer.Descendents().OfType<ScrollBar>().Where(p => p.Visibility == Visibility.Visible);

      foreach ( var scrollBar in scrollBars )
      {
        var grid = scrollBar.Descendents().OfType<Grid>().FirstOrDefault(p => p.Name == "GridHorizontalScrollBar");

        if ( grid == null )
          continue;

        return grid;
      }

      return null;
    }
  }
}
