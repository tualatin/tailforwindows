using System;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Behaviors.Base;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// ScrollDataGridRowInto view
  /// </summary>
  public class ScrollDataGridRowIntoView : BehaviorBase<DataGrid>
  {
    /// <summary>
    /// Setup <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected override void OnSetup() => AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;

    /// <summary>
    /// Release all resource used by <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected override void OnCleanup() => AssociatedObject.SelectionChanged -= AssociatedObjectSelectionChanged;

    private static void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var datagrid = sender as DataGrid;

      if ( datagrid?.SelectedItem != null )
      {
        datagrid.Dispatcher.BeginInvoke((Action) (() =>
        {
          datagrid.UpdateLayout();

          if ( datagrid.SelectedItem != null )
            datagrid.ScrollIntoView(datagrid.SelectedItem);
        }));
      }
    }
  }
}
