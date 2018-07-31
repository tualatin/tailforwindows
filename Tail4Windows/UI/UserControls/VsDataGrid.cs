using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using log4net;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Virtual Studios <see cref="DataGrid"/>
  /// </summary>
  public class VsDataGrid : DataGrid
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(VsDataGrid));

    #region constants

    /// <summary>
    /// DisplayIndex
    /// </summary>
    private const string VsColumnDisplayIndex = "DisplayIndex";

    /// <summary>
    /// Width
    /// </summary>
    private const string VsColumnWidth = "Width";

    /// <summary>
    /// Visibility
    /// </summary>
    private const string VsColumnVisibility = "Visibility";

    /// <summary>
    /// SortDirection
    /// </summary>
    private const string VsColumnSortDirection = "SortDirection";

    #endregion

    /// <summary>
    /// Current attached columns
    /// </summary>
    private readonly Dictionary<DataGridColumn, DataGrid> _attachedDataGridColumns;

    private Grid _horizontalScrollbarGrid;
    private ScrollViewer _scrollViewer;
    private string _userDataGridSettingsFile;

    /// <summary>
    /// Save DataGrid layout property
    /// </summary>
    public static readonly DependencyProperty SaveDataGridLayoutProperty = DependencyProperty.Register(nameof(SaveDataGridLayout), typeof(bool), typeof(VsDataGrid),
      new PropertyMetadata(true));

    /// <summary>
    /// Save DataGrid layout
    /// </summary>
    public bool SaveDataGridLayout
    {
      get => (bool) GetValue(SaveDataGridLayoutProperty);
      set => SetValue(SaveDataGridLayoutProperty, value);
    }

    /// <summary>
    /// ActualColumnWidth property descriptor
    /// </summary>
    private readonly PropertyDescriptor _actualColumnWidthDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));

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

    /// <summary>
    /// Loads current <see cref="VsDataGrid"/> options
    /// </summary>
    private void LoadDataGridOptions()
    {
      if ( !File.Exists(_userDataGridSettingsFile) )
        return;

      LOG.Trace("Read DataGrid options");

      var columns = new DataSet(Name);
      columns.ReadXml(_userDataGridSettingsFile);

      int index = 0;

      foreach ( DataGridColumn column in Columns )
      {
        DataRow row = columns.Tables[0].Rows[index];

        try
        {
          int displayIndex = Convert.ToInt32(row[VsColumnDisplayIndex]);
          column.DisplayIndex = displayIndex;

          double.TryParse(row[VsColumnWidth].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double width);
          column.Width = width;

          int visibility = Convert.ToInt32(row[VsColumnVisibility]);

          switch ( visibility )
          {
          case 0:

            column.Visibility = Visibility.Visible;
            break;

          case 2:

            column.Visibility = Visibility.Collapsed;
            break;
          }

          try
          {
            int sortDirection = Convert.ToInt32(row[VsColumnSortDirection]);

            switch ( sortDirection )
            {
            case 0:

              column.SortDirection = ListSortDirection.Ascending;
              break;

            case 1:

              column.SortDirection = ListSortDirection.Descending;
              break;
            }

            if ( column.SortDirection != null )
              Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, column.SortDirection.Value));

            Items.Refresh();
          }
          catch
          {
            // Nothing
          }
        }
        finally
        {
          index++;
        }
      }
    }

    /// <summary>
    /// Save current <see cref="VsDataGrid"/> options
    /// </summary>
    public void SaveDataGridOptions()
    {
      if ( string.IsNullOrWhiteSpace(_userDataGridSettingsFile) || !SaveDataGridLayout )
        return;

      LOG.Trace("Save DataGrid options");

      var columns = new DataSet(Name);
      var columnTable = new DataTable("columns");

      try
      {
        columnTable.Columns.Add(VsColumnDisplayIndex, Type.GetType("System.Int32") ?? throw new InvalidOperationException());
        columnTable.Columns.Add(VsColumnWidth, Type.GetType("System.Double") ?? throw new InvalidOperationException());
        columnTable.Columns.Add(VsColumnVisibility, Type.GetType("System.Int32") ?? throw new InvalidOperationException());
        columnTable.Columns.Add(VsColumnSortDirection, Type.GetType("System.Int32") ?? throw new InvalidOperationException());

        columns.Tables.Add(columnTable);

        foreach ( DataGridColumn column in Columns )
        {
          columnTable.Rows.Add(column.DisplayIndex, column.Width.DisplayValue, column.Visibility, column.SortDirection);
        }

        columns.WriteXml(_userDataGridSettingsFile);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

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

      foreach ( ScrollBar scrollBar in scrollBars )
      {
        var grid = scrollBar.Descendents().OfType<Grid>().FirstOrDefault(p => p.Name == "GridHorizontalScrollBar");

        if ( grid == null )
          continue;

        return grid;
      }
      return null;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      _userDataGridSettingsFile = EnvironmentContainer.UserSettingsPath + $@"\{Name}_Layout.xml";
      _scrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      if ( _scrollViewer == null )
        return;

      _scrollViewer.ScrollChanged += OnScrollChanged;

      SetupColumnHeaders();

      if ( SaveDataGridLayout )
        LoadDataGridOptions();

      OnScrollChanged(this, null);
    }

    private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( _scrollViewer == null )
        return;

      if ( _horizontalScrollbarGrid != null )
        return;

      _horizontalScrollbarGrid = GetHorizontalScrollBarGrid(_scrollViewer);
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
          _actualColumnWidthDescriptor.AddValueChanged(column, OnActualWidthChanged);
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
        _actualColumnWidthDescriptor.RemoveValueChanged(column, OnActualWidthChanged);
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

    private void SetupColumnHeaders()
    {
      var columnHeaders = GetColumnHeaders();

      if ( columnHeaders == null || columnHeaders.Length == 0 )
        return;

      foreach ( DataGridColumnHeader header in columnHeaders )
      {
        SetupColumnHeader(columnHeaders, header);
      }
    }

    private void SetupColumnHeader(DataGridColumnHeader[] columns, DataGridColumnHeader columnHeader)
    {
      if ( columnHeader.ContextMenu == null )
        columnHeader.ContextMenu = new ContextMenu();

      foreach ( DataGridColumnHeader column in columns )
      {
        if ( string.IsNullOrWhiteSpace(GetColumnName(column.Column)) )
          continue;

        var item = new MenuItem
        {
          Header = GetColumnName(column.Column),
          IsCheckable = true,
          IsChecked = columnHeader.Column.Visibility == Visibility.Visible
        };

        item.Checked += delegate
        {
          column.Column.Visibility = Visibility.Visible;
        };

        item.Unchecked += delegate
        {
          column.Column.Visibility = Visibility.Hidden;
        };

        columnHeader.ContextMenu?.Items.Add(item);
      }
    }

    private string GetColumnName(DataGridColumn column)
    {
      if ( column == null )
        return string.Empty;

      return column.Header != null ? column.Header.ToString() : $"Column {column.DisplayIndex}";
    }

    private DataGridColumnHeader[] GetColumnHeaders()
    {
      UpdateLayout();
      var columnHeaders = this.Descendents().OfType<DataGridColumnHeader>().ToList();
      return columnHeaders.Where(p => p?.Column != null).ToArray();
    }
  }
}
