using System;
using System.Windows.Controls;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// ScrollDataGridRowInto view
  /// </summary>
  public class ScrollDataGridRowIntoView : Behavior<DataGrid>
  {
    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    private static void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      DataGrid datagrid = sender as DataGrid;

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

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
    }
  }
}
