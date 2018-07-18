using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Extensions;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Virtual Studios <see cref="DataGrid"/>
  /// </summary>
  public class VsDataGrid : DataGrid
  {
    /// <summary>
    /// Current attached columns
    /// </summary>
    private readonly Dictionary<DataGridColumn, DataGrid> _attachedDataGridColumns;

    private Grid _horizontalScrollbarGrid;
    private ScrollViewer _scrollViewer;

    /// <summary>
    /// ActualColumnWidth property descriptor
    /// </summary>
    public PropertyDescriptor ActualColumnWidthDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));

    static VsDataGrid() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VsDataGrid), new FrameworkPropertyMetadata(typeof(VsDataGrid)));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsDataGrid()
    {
      _attachedDataGridColumns = new Dictionary<DataGridColumn, DataGrid>();

      Columns.CollectionChanged += OnColumnsCollectionChanged;
      Loaded += OnLoaded;
      Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
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

      _horizontalScrollbarGrid = BusinessHelper.GetHorizontalScrollBarGrid(_scrollViewer);
      OnActualWidthChanged(this, EventArgs.Empty);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      if ( _scrollViewer != null )
      {
        _scrollViewer.ScrollChanged -= OnScrollChanged;
        _scrollViewer = null;
      }

      _horizontalScrollbarGrid = null;
    }

    private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if ( e.NewItems != null )
      {
        foreach ( object item in e.NewItems )
        {
          if ( !(item is DataGridColumn column) )
            continue;

          if ( _attachedDataGridColumns.ContainsKey(column) )
            continue;

          _attachedDataGridColumns.Add(column, this);
          ActualColumnWidthDescriptor.AddValueChanged(column, OnActualWidthChanged);
        }
      }

      if ( e.OldItems == null )
        return;

      foreach ( object item in e.OldItems )
      {
        if ( !(item is DataGridColumn column) )
          continue;

        if ( !_attachedDataGridColumns.ContainsKey(column) )
          continue;

        _attachedDataGridColumns.Remove(column);
        ActualColumnWidthDescriptor.RemoveValueChanged(column, OnActualWidthChanged);
      }
    }

    private void OnActualWidthChanged(object sender, EventArgs e)
    {
      if ( _horizontalScrollbarGrid == null )
        return;

      double width = 0;

      foreach ( var column in _attachedDataGridColumns.Where(p => p.Key.IsFrozen).ToList() )
      {
        width += column.Key.ActualWidth;
      }

      _horizontalScrollbarGrid.Margin = new Thickness(width, 0, 0, 0);
    }
  }
}
