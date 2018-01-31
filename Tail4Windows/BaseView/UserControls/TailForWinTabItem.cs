using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.BaseView.UserControls
{
  /// <summary>
  /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
  ///
  /// Step 1a) Using this custom control in a XAML file that exists in the current project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:Org.Vs.TailForWin.BaseView.UserControls"
  ///
  ///
  /// Step 1b) Using this custom control in a XAML file that exists in a different project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:Org.Vs.TailForWin.BaseView.UserControls;assembly=Org.Vs.TailForWin.BaseView.UserControls"
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
    static TailForWinTabItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TailForWinTabItem), new FrameworkPropertyMetadata(typeof(TailForWinTabItem)));

    /// <summary>
    /// TabHeaderDoubleClicke event handler
    /// </summary>
    public static readonly RoutedEvent TabHeaderDoubleClickEvent = EventManager.RegisterRoutedEvent("TabHeaderDoubleClick", RoutingStrategy.Bubble,
                                                                      typeof(RoutedEventHandler), typeof(TailForWinTabItem));

    /// <summary>
    /// TabHeaderDoubleClick
    /// </summary>
    public event RoutedEventHandler TabHeaderDoubleClick
    {
      add => AddHandler(TabHeaderDoubleClickEvent, value);
      remove => RemoveHandler(TabHeaderDoubleClickEvent, value);
    }

    /// <summary>
    /// CloseTabWindow event handler
    /// </summary>
    public static readonly RoutedEvent CloseTabWindowEvent = EventManager.RegisterRoutedEvent("CloseTabWindow", RoutingStrategy.Bubble,
                                                                  typeof(RoutedEventHandler), typeof(TailForWinTabItem));

    /// <summary>
    /// Close tab window when user press the close button in TabHeader
    /// </summary>
    public event RoutedEventHandler CloseTabWindow
    {
      add => AddHandler(CloseTabWindowEvent, value);
      remove => RemoveHandler(CloseTabWindowEvent, value);
    }

    /// <summary>
    /// Set HeaderToolTipProperty property
    /// </summary>
    public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register("HeaderToolTip", typeof(object), typeof(TailForWinTabItem), new UIPropertyMetadata(null));

    /// <summary>
    /// Set HeaderToolTip
    /// </summary>
    public object HeaderToolTip
    {
      private get => GetValue(HeaderToolTipProperty);
      set => SetValue(HeaderToolTipProperty, value);
    }


    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal proc esses call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if ( GetTemplateChild("tabItemCloseButton") is Button closeButton )
        closeButton.Click += CloseButtonClick;

      if ( !(GetTemplateChild("gridHeader") is Grid headerGrid) )
        return;

      headerGrid.PreviewMouseDown += HeaderGridMiddleMouseButtonDown;
      headerGrid.MouseLeftButtonDown += HeaderGridMouseLeftButtonDown;

      // set special ToolTip for TabItemHeader
      ToolTip myToolTip = new ToolTip()
      {
        Style = (Style) FindResource("TabItemToolTipStyle"),
        Content = HeaderToolTip
      };

      if ( HeaderToolTip != null )
        ToolTipService.SetToolTip(headerGrid, myToolTip);
    }

    private void CloseButtonClick(object sender, RoutedEventArgs e)
    {
      if ( Parent is TabControl )
        RaiseEvent(new RoutedEventArgs(CloseTabWindowEvent, this));
    }

    private void HeaderGridMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( e.ClickCount == 2 )
        RaiseEvent(new RoutedEventArgs(TabHeaderDoubleClickEvent, this));
    }

    private void HeaderGridMiddleMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( !(Parent is TabControl) )
        return;

      if ( e.MiddleButton == MouseButtonState.Pressed )
        RaiseEvent(new RoutedEventArgs(CloseTabWindowEvent, this));
    }
  }
}
