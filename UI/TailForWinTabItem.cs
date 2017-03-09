using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.UI
{
  /// <summary>
  /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
  ///
  /// Step 1a) Using this custom control in a XAML file that exists in the current project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:Org.Vs.TailForWin.UI"
  ///
  ///
  /// Step 1b) Using this custom control in a XAML file that exists in a different project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:Org.Vs.TailForWin.UserControls;assembly=Org.Vs.TailForWin.UserControls"
  ///
  /// You will also need to add a project reference from the project where the XAML file lives
  /// to this project and Rebuild to avoid compilation errors:
  ///
  ///     Right click on the target project in the Solution Explorer and
  ///     "Add Reference"->"Projects"->[Browse to and select this project]
  ///
  ///
  /// Step 2)
  /// Go ahead and use your control in the XAML file.
  ///
  ///     <MyNamespace:TailForWinTabItem/>
  ///
  /// </summary>
  public class TailForWinTabItem : TabItem
  {
    static TailForWinTabItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TailForWinTabItem), new FrameworkPropertyMetadata(typeof(TailForWinTabItem)));
    }

    /// <summary>
    /// TabHeaderDoubleClicke event
    /// </summary>
    public static readonly RoutedEvent TabHeaderDoubleClickEvent = EventManager.RegisterRoutedEvent("TabHeaderDoubleClick", RoutingStrategy.Bubble,
                                                                      typeof(RoutedEventHandler), typeof(TailForWinTabItem));

    /// <summary>
    /// TabHeaderDoubleClick
    /// </summary>
    public event RoutedEventHandler TabHeaderDoubleClick
    {
      add
      {
        AddHandler(TabHeaderDoubleClickEvent, value);
      }
      remove
      {
        RemoveHandler(TabHeaderDoubleClickEvent, value);
      }
    }


    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal proc esses call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if(GetTemplateChild("tabItemCloseButton") is Button closeButton)
        closeButton.Click += new RoutedEventHandler(CloseButton_Click);

      if(GetTemplateChild("gridHeader") is Grid headerGrid)
        headerGrid.MouseLeftButtonDown += new MouseButtonEventHandler(HeaderGrid_MouseLeftButtonDown);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      if(Parent is TabControl tabCtrl)
      {
        if(tabCtrl.Items.Count <= 2)
        {
          MessageBox.Show("Is last tabitem, ca not remove!");
          return;
        }

        tabCtrl.Items.Remove(this);
      }
    }

    private void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if(e.ClickCount == 2)
        RaiseEvent(new RoutedEventArgs(TabHeaderDoubleClickEvent, this));
    }
  }
}
