using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Org.Vs.TailForWin.Ui.PlugIns.VsControls.ExtendedControls;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  ///  Attached Behavior
  ///  Somewhat trickier than a regular behavior because it's to be attached via a style 
  /// </summary>
  public class DataGridRowBehavior : Behavior<DataGridRow>
  {
    /// <summary>
    /// Get is <see cref="DataGridRow"/> focused when selected
    /// </summary>
    /// <param name="dataGridRow"><see cref="DataGridRow"/></param>
    /// <returns><c>True</c> if it is selected, otherwise <c>False</c></returns>
    public static bool GetIsDataGridRowFocusedWhenSelected(DataGridRow dataGridRow) => (bool) dataGridRow.GetValue(IsDataGridRowFocusedWhenSelectedProperty);

    /// <summary>
    /// Set is <see cref="DataGridRow"/> focused when selected
    /// </summary>
    /// <param name="dataGridRow"><see cref="DataGridRow"/></param>
    /// <param name="value">Bool value</param>
    public static void SetIsDataGridRowFocusedWhenSelected(DataGridRow dataGridRow, bool value) => dataGridRow.SetValue(IsDataGridRowFocusedWhenSelectedProperty, value);

    /// <summary>
    /// <see cref="DependencyProperty"/> IsDataGridRowFocusedWhenSelected
    /// </summary>
    public static readonly DependencyProperty IsDataGridRowFocusedWhenSelectedProperty = DependencyProperty.RegisterAttached("IsDataGridRowFocusedWhenSelected", typeof(bool),
      typeof(DataGridRowBehavior), new UIPropertyMetadata(false, OnIsDataGridRowFocusedWhenSelectedChanged));

    private static void OnIsDataGridRowFocusedWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      if ( !(depObj is DataGridRow item) )
        return;

      //if ( e.NewValue is bool == false )
      //  return;

      if ( (bool) e.NewValue )
        item.Selected += OnDataGridRowSelected;
      else
        item.Selected -= OnDataGridRowSelected;
    }

    private static void OnDataGridRowSelected(object sender, RoutedEventArgs e)
    {
      // If focus is already on a cell then don't focus back out of it
      if ( Keyboard.FocusedElement is DataGridCell || Keyboard.FocusedElement is VsWatermarkTextBox || !(e.OriginalSource is DataGridRow row) )
        return;

      row.Focusable = true;
      Keyboard.Focus(row);
    }
  }
}
