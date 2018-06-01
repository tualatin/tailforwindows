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
    private ScrollViewer _scrollViewer;


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

      _scrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      if ( _scrollViewer == null )
        return;

      _scrollViewer.ScrollChanged += OnScrollChanged;
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( _scrollViewer == null )
        return;

      if ( _horizontalScrollbarGrid != null )
        return;

      _horizontalScrollbarGrid = UiHelpers.GetHorizontalScrollBarGrid(_scrollViewer);
      OnWidthChanged(this, null);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      if ( _scrollViewer != null )
      {
        _scrollViewer.ScrollChanged -= OnScrollChanged;
        _scrollViewer = null;
      }

      _horizontalScrollbarGrid = null;

      DataGridColumnWidthBehavior.RemoveColumnWidthChangedEventHandler(this, OnWidthChanged);
      var control = DataGridColumnWidthBehavior.AttacheDataGridColumns.Where(p => Equals(this, p.Value)).ToList();

      if ( control.Count == 0 )
        return;

      control.ForEach(p =>
      {
        DataGridColumnWidthBehavior.AttacheDataGridColumns.Remove(p.Key);
      });
    }

    private void OnWidthChanged(object sender, RoutedEventArgs e)
    {
      if ( _horizontalScrollbarGrid == null )
        return;

      if ( !(sender is VsDataGrid dg) )
        return;

      double width = 0;

      foreach ( var column in DataGridColumnWidthBehavior.AttacheDataGridColumns.Where(p => Equals(p.Value, dg)).ToList() )
      {
        width += column.Key.ActualWidth;
      }

      _horizontalScrollbarGrid.Margin = new Thickness(width, 0, 0, 0);
    }
  }
}
