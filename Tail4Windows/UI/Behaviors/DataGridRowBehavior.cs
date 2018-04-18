using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  ///  Attached Behavior
  ///  Somewhat trickier than a regular behavior because it's to be attached via a style 
  /// </summary>
  public class DataGridRowBehavior : Behavior<DataGridRow>
  {
    /// <summary>
    /// Get is <see cref="DataGridRow"/> focussed when selected
    /// </summary>
    /// <param name="dataGridRow"><see cref="DataGridRow"/></param>
    /// <returns><c>True</c> if it is selected, otherwise <c>False</c></returns>
    public static bool GetIsDataGridRowFocussedWhenSelected(DataGridRow dataGridRow) => (bool) dataGridRow.GetValue(IsDataGridRowFocussedWhenSelectedProperty);

    /// <summary>
    /// Set is <see cref="DataGridRow"/> focussed when selected
    /// </summary>
    /// <param name="dataGridRow"><see cref="DataGridRow"/></param>
    /// <param name="value">Bool value</param>
    public static void SetIsDataGridRowFocussedWhenSelected(DataGridRow dataGridRow, bool value) => dataGridRow.SetValue(IsDataGridRowFocussedWhenSelectedProperty, value);

    /// <summary>
    /// <see cref="DependencyProperty"/> IsDataGridRowFocussedWhenSelected
    /// </summary>
    public static readonly DependencyProperty IsDataGridRowFocussedWhenSelectedProperty = DependencyProperty.RegisterAttached("IsDataGridRowFocussedWhenSelected", typeof(bool),
      typeof(DataGridRowBehavior), new UIPropertyMetadata(false, OnIsDataGridRowFocussedWhenSelectedChanged));

    private static void OnIsDataGridRowFocussedWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      if ( !(depObj is DataGridRow item) )
        return;

      if ( e.NewValue is bool == false )
        return;

      if ( (bool) e.NewValue )
        item.Selected += OndataGridRowSelected;
      else
        item.Selected -= OndataGridRowSelected;
    }

    private static void OndataGridRowSelected(object sender, RoutedEventArgs e)
    {
      // If focus is already on a cell then don't focus back out of it
      if ( Keyboard.FocusedElement is DataGridCell || !(e.OriginalSource is DataGridRow row) )
        return;

      row.Focusable = true;
      Keyboard.Focus(row);
    }
  }
}
