using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Behaviors;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Virtual Studios <see cref="DataGrid"/>
  /// </summary>
  public class VsDataGrid : DataGrid
  {
    private Grid _horizontalScrollbarGrid;


    static VsDataGrid() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VsDataGrid), new FrameworkPropertyMetadata(typeof(VsDataGrid)));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsDataGrid()
    {
      Loaded += OnLoaded;
      Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      DataGridColumnWidthBehavior.AddColumnWidthChangedEventHandler(this, OnWidthChanged);

      var scrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();
      _horizontalScrollbarGrid = UiHelpers.GetHorizontalScrollBarGrid(scrollViewer);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) => DataGridColumnWidthBehavior.RemoveColumnWidthChangedEventHandler(this, OnWidthChanged);


    private void OnWidthChanged(object sender, RoutedEventArgs e)
    {
      if ( _horizontalScrollbarGrid == null )
      {
        var scrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();
        _horizontalScrollbarGrid = UiHelpers.GetHorizontalScrollBarGrid(scrollViewer);
      }

      if ( _horizontalScrollbarGrid == null )
        return;

      double width = ((DataGridLength) e.OriginalSource).Value;
      _horizontalScrollbarGrid.Margin = new Thickness(width, 0, 0, 0);
    }
  }
}
