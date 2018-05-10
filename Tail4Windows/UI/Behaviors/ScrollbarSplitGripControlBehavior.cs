using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  public class ScrollbarSplitGripControlBehavior
  {
    /// <summary>
    /// Get IsFocused
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsFocused(DependencyObject obj) => (bool) obj.GetValue(IsFocusedProperty);

    /// <summary>
    /// Set IsFocused
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

    /// <summary>
    /// IsFocused property
    /// </summary>
    public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(ScrollbarSplitGripControlBehavior), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

    private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is LogWindowListBox lb) )
        return;

      var test = lb.Descendents().OfType<ScrollViewer>();

      foreach ( var blubb in test )
      {

      }
    }
  }
}
