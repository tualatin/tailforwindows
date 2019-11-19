using System;
using System.Collections;
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
using Org.Vs.TailForWin.Ui.Utils.Extensions;


namespace Org.Vs.TailForWin.Ui.PlugIns.VsControls.ExtendedControls
{
  /// <summary>
  /// Virtual Studios <see cref="DataGrid"/>
  /// </summary>
  public class VsDataGrid : DataGrid
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(VsDataGrid));

    #region Constants

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

    #region Color keys

    /// <summary>
    /// Resource 'DataGridSelectAllButtonStyle'
    /// </summary>
    public static  ComponentResourceKey DataGridSelectAllButtonStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "DataGridSelectAllButtonStyle");

    /// <summary>
    /// Resource 'DataGridCellStyle'
    /// </summary>
    public static ComponentResourceKey DataGridCellStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "DataGridCellStyle");

    /// <summary>
    /// Resource 'RowHeaderGripperStyle'
    /// </summary>
    public static ComponentResourceKey RowHeaderGripperStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "RowHeaderGripperStyle");

    /// <summary>
    /// Resource 'DataGridRowHeaderStyle'
    /// </summary>
    public static ComponentResourceKey DataGridRowHeaderStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "DataGridRowHeaderStyle");

    /// <summary>
    /// Resource 'DataGridRowStyle'
    /// </summary>
    public static ComponentResourceKey DataGridRowStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "DataGridRowStyle");

    /// <summary>
    /// Resource 'ColumnHeaderGripperStyle'
    /// </summary>
    public static ComponentResourceKey ColumnHeaderGripperStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "ColumnHeaderGripperStyle");

    /// <summary>
    /// Resource 'DataGridColumnHeaderStyle'
    /// </summary>
    public static  ComponentResourceKey DataGridColumnHeaderStyleKey => new ComponentResourceKey(typeof(VsDataGrid), "DataGridColumnHeaderStyle");

    #endregion

    /// <summary>
    /// ColumnHeader <see cref="ContextMenu"/>
    /// </summary>
    private readonly ContextMenu _columnHeaderContextMenu;

    private bool _oneTime;

    /// <summary>
    /// Current attached columns
    /// </summary>
    private readonly Dictionary<DataGridColumn, DataGrid> _attachedDataGridColumns;

    private Grid _horizontalScrollbarGrid;
    private ScrollViewer _scrollViewer;
    private string _userDataGridSettingsFile;

    private string AllColumnsHeaders
    {
      get;
      set;
    }

    /// <summary>
    /// SelectedItemsList property
    /// </summary>
    public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register(nameof(SelectedItemsList), typeof(IList), typeof(VsDataGrid),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// SelectedItemsList
    /// </summary>
    public IList SelectedItemsList
    {
      get => (IList) GetValue(SelectedItemsListProperty);
      set => SetValue(SelectedItemsListProperty, value);
    }

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
    /// Visible columns property
    /// </summary>
    public static readonly DependencyProperty VisibleColumnsProperty = DependencyProperty.Register(nameof(VisibleColumns), typeof(string), typeof(VsDataGrid),
      new PropertyMetadata(string.Empty, OnVisibleColumnsChanged));

    /// <summary>
    /// Gets or sets a value indicating the names of columns (as they appear in the column header) to be visible, separated by a semicolon.
    /// columns that whose name is not here will be hidden.
    /// </summary>
    public string VisibleColumns
    {
      get => (string) GetValue(VisibleColumnsProperty);
      set => SetValue(VisibleColumnsProperty, value);
    }

    private static void OnVisibleColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is VsDataGrid dg) )
        return;

      dg.VisibleColumnsChanged(e);
    }

    private void VisibleColumnsChanged(DependencyPropertyChangedEventArgs e)
    {
      if ( e.NewValue == null )
        return;

      string[] showTheseColumns = e.NewValue.ToString().Split(';');
      string colName;

      // update the columns visibility.
      foreach ( var col in Columns )
      {
        colName = col.Header.ToString();
        col.Visibility = showTheseColumns.Contains(colName) ? Visibility.Visible : Visibility.Collapsed;
      }

      // update the context menu items.
      if ( _columnHeaderContextMenu == null )
        return;

      foreach ( MenuItem menuItem in _columnHeaderContextMenu.Items )
      {
        colName = menuItem.Header.ToString();
        menuItem.IsChecked = showTheseColumns.Contains(colName);
      }
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
      _columnHeaderContextMenu = new ContextMenu();
      _oneTime = true;

      SelectionChanged += OnVsDataGridSelectionChanged;
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

      var index = 0;

      foreach ( var column in Columns )
      {
        if ( index >= columns.Tables[0].Rows.Count )
          continue;

        var row = columns.Tables[0].Rows[index];

        try
        {
          int displayIndex = Convert.ToInt32(row[VsColumnDisplayIndex]);
          column.DisplayIndex = displayIndex;

          double.TryParse(row[VsColumnWidth].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double width);
          column.Width = width;

          int visibility = Convert.ToInt32(row[VsColumnVisibility]);
          var splited = string.IsNullOrWhiteSpace(VisibleColumns) ? AllColumnsHeaders.Split(';').ToList() : VisibleColumns.Split(';').ToList();

          switch ( visibility )
          {
          case 0:

            column.Visibility = Visibility.Visible;
            break;

          case 2:

            if ( splited.Contains(column.Header) )
            {
              splited.Remove(column.Header.ToString());
              BuildVisibleColumns(splited);
            }

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

        foreach ( var column in Columns )
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

      foreach ( var scrollBar in scrollBars )
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
      _userDataGridSettingsFile = CoreEnvironment.UserSettingsPath + $@"\{Name}_Layout.xml";
      _scrollViewer = this.Descendents().OfType<ScrollViewer>().FirstOrDefault();

      if ( _scrollViewer == null )
        return;

      _scrollViewer.ScrollChanged += OnScrollChanged;

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

    private void OnVsDataGridSelectionChanged(object sender, SelectionChangedEventArgs e) => SelectedItemsList = SelectedItems;

    private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if ( e.NewItems != null )
      {
        if ( e.NewItems[0] is DataGridColumn col )
        {
          // keep a list of all columns headers for later use.
          AllColumnsHeaders = $"{col.Header};{AllColumnsHeaders}";

          // make a new menu item and add it to the context menu.
          MenuItem menuItem = new MenuItem();
          menuItem.Click += ColumnMenuItemClick;
          menuItem.Header = col.Header.ToString();
          menuItem.IsCheckable = true;

          _columnHeaderContextMenu.Items.Add(menuItem);
        }

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

    /// <summary>
    /// When overridden in a derived class, positions child elements and determines a size for a FrameworkElement derived class. 
    /// </summary>
    /// <param name="arrangeBounds">The final area within the parent that this element should use to arrange itself and its children.</param>
    /// <returns>The actual size used.</returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      if ( !_oneTime )
        return base.ArrangeOverride(arrangeBounds);

      _oneTime = false;
      var headersPresenter = this.Descendents().OfType<DataGridColumnHeadersPresenter>().FirstOrDefault();

      if ( headersPresenter == null )
        return base.ArrangeOverride(arrangeBounds);

      ContextMenuService.SetContextMenu(headersPresenter, _columnHeaderContextMenu);

      if ( string.IsNullOrWhiteSpace(VisibleColumns) )
      {
        VisibleColumns = AllColumnsHeaders;
      }
      else
      {
        string s = VisibleColumns;
        VisibleColumns = null;
        VisibleColumns = s;
      }
      return base.ArrangeOverride(arrangeBounds);
    }

    private void ColumnMenuItemClick(object sender, RoutedEventArgs e)
    {
      if ( !(sender is MenuItem mi) )
        return;

      var splited = VisibleColumns.Split(';').ToList();
      string columnName = mi.Header.ToString();

      if ( !mi.IsChecked )
        splited.Remove(columnName);
      else
        splited.Add(columnName);

      BuildVisibleColumns(splited);
    }

    private void BuildVisibleColumns(List<string> splited)
    {
      string build = string.Empty;

      foreach ( var name in splited )
      {
        if ( string.IsNullOrWhiteSpace(name) )
          continue;

        build = $"{name};{build}";
      }

      VisibleColumns = build;
    }
  }
}
