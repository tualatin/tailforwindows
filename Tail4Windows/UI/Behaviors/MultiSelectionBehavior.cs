using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// MultiSelectionBehavior
  /// </summary>
  public class MultiSelectionBehavior : Behavior<DataGrid>
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public MultiSelectionBehavior() => SelectedItems = new ObservableCollection<object>();

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached() => AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    protected override void OnDetaching() => AssociatedObject.SelectionChanged -= AssociatedObjectSelectionChanged;

    private void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedItems = AssociatedObject.SelectedItems;

      if ( e.RemovedItems != null )
      {
        foreach ( var item in e.RemovedItems )
        {
          if ( selectedItems.Contains(item))
            selectedItems.Remove(item);
        }
      }

      if ( e.AddedItems != null )
      {
        foreach ( var item in e.AddedItems )
        {
          selectedItems.Add(item);
        }
      }

      SelectedItems = selectedItems;
    }

    /// <summary>
    /// SelectedItems property
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectionBehavior),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// SelectedItems
    /// </summary>
    public IList SelectedItems
    {
      get => (IList) GetValue(SelectedItemsProperty);
      set => SetValue(SelectedItemsProperty, value);
    }
  }
}
