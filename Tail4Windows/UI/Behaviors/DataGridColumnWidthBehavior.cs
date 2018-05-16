﻿using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Org.Vs.TailForWin.UI.UserControls;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// DataGridCoumnWidthBehavior
  /// </summary>
  public class DataGridColumnWidthBehavior : Behavior<DataGridColumn>
  {
    #region Dependency properties

    /// <summary>
    /// DataGridColumnWidth property
    /// </summary>
    public static readonly DependencyProperty DataGridColumnWidthProperty = DependencyProperty.RegisterAttached("DataGridColumnWidth", typeof(DataGridLength),
      typeof(DataGridColumnWidthBehavior), new FrameworkPropertyMetadata(OnColumnWidthChanged));

    /// <summary>
    /// Get DataGridColumnWidth
    /// </summary>
    /// <param name="element"><see cref="DataGridColumn"/></param>
    /// <returns>DataGridColumnWidthProperty</returns>
    public static object GetDataGridColumnWidth(DataGridColumn element) => (DataGridLength) element.GetValue(DataGridColumnWidthProperty);

    /// <summary>
    /// Sets DataGridColumnWidth
    /// </summary>
    /// <param name="element"><see cref="DataGridColumn"/></param>
    /// <param name="value">Value</param>
    public static void SetDataGridColumnWidth(DataGridColumn element, DataGridLength value) => element.SetValue(DataGridColumnWidthProperty, value);

    #endregion

    #region RoutedEvents

    /// <summary>
    /// ColumnWidthChanged event handler
    /// </summary>
    public static readonly RoutedEvent ColumnWidthChangedRoutedEvent = EventManager.RegisterRoutedEvent(nameof(ColumnWidthChangedRoutedEvent), RoutingStrategy.Bubble,
      typeof(RoutedEventHandler), typeof(DataGridColumnWidthBehavior));

    /// <summary>
    /// Add ColumnWidthChanged event handler
    /// </summary>
    /// <param name="d"><see cref="DependencyObject"/></param>
    /// <param name="e"><see cref="RoutedEventHandler"/></param>
    public static void AddColumnWidthChangedEventHandler(DependencyObject d, RoutedEventHandler e)
    {
      if ( !(d is UIElement element) )
        return;

      element.AddHandler(ColumnWidthChangedRoutedEvent, e);
    }

    /// <summary>
    /// Remove ColumnWidthChanged event handler
    /// </summary>
    /// <param name="d"><see cref="DependencyObject"/></param>
    /// <param name="e"><see cref="RoutedEventHandler"/></param>
    public static void RemoveColumnWidthChangedEventHandler(DependencyObject d, RoutedEventHandler e)
    {
      if ( !(d is UIElement element) )
        return;

      element.RemoveHandler(ColumnWidthChangedRoutedEvent, e);
    }

    #endregion

    #region Callback functions

    private static void OnColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is DataGridColumn column) )
        return;

      if ( !column.IsFrozen )
        return;

      var propertyInfo = column.GetType().GetProperty("DataGridOwner", BindingFlags.Instance | BindingFlags.NonPublic);
      var owner = propertyInfo?.GetValue(column, null) as VsDataGrid;

      owner?.RaiseEvent(new RoutedEventArgs(ColumnWidthChangedRoutedEvent, column.Width));
    }

    #endregion
  }
}