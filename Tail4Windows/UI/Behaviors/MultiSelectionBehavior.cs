using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Behaviors.Base;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// MultiSelectionBehavior
  /// </summary>
  public class MultiSelectionBehavior : BehaviorBase<DataGrid>
  {
    /// <summary>
    /// Setup <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected override void OnSetup() => AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;

    /// <summary>
    /// Release all resource used by <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected override void OnCleanup() => AssociatedObject.SelectionChanged -= AssociatedObjectSelectionChanged;

    private void AssociatedObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedItems = AssociatedObject.SelectedItems;

      if ( e.RemovedItems != null )
      {
        foreach ( var item in e.RemovedItems )
        {
          if ( selectedItems.Contains(item) )
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

      SelectedItems = new ObservableCollection<object>();

      foreach ( object item in selectedItems )
      {
        SelectedItems.Add(item);
      }
    }

    /// <summary>
    /// SelectedItems property
    /// </summary>
    public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectionBehavior),
      new FrameworkPropertyMetadata(new ObservableCollection<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
