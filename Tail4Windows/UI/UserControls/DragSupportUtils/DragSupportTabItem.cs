using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Extensions;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Drag support TabItem
  /// </summary>
  public class DragSupportTabItem : TabItem
  {
    static DragSupportTabItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DragSupportTabItem), new FrameworkPropertyMetadata(typeof(DragSupportTabItem)));

    /// <summary>
    /// TabHeaderDoubleClicke event handler
    /// </summary>
    public static readonly RoutedEvent TabHeaderDoubleClickEvent = EventManager.RegisterRoutedEvent("TabHeaderDoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragSupportTabItem));

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
    public static readonly RoutedEvent CloseTabWindowEvent = EventManager.RegisterRoutedEvent("CloseTabWindow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragSupportTabItem));

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
    public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register("HeaderToolTip", typeof(object), typeof(DragSupportTabItem), new UIPropertyMetadata(null));

    /// <summary>
    /// Set HeaderToolTip
    /// </summary>
    public object HeaderToolTip
    {
      private get => GetValue(HeaderToolTipProperty);
      set => SetValue(HeaderToolTipProperty, value);
    }

    /// <summary>
    /// Set HeaderToolContentProperty property
    /// </summary>
    public static readonly DependencyProperty HeaderToolContentProperty = DependencyProperty.Register("HeaderContent", typeof(string), typeof(DragSupportTabItem));

    /// <summary>
    /// Set HeaderToolContent
    /// </summary>
    public string HeaderContent
    {
      private get => (string) GetValue(HeaderToolContentProperty);
      set
      {
        if ( string.IsNullOrWhiteSpace(value) )
          return;

        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        string item = value.MeasureTextAndCutIt(typeface, FontSize, 160);

        SetValue(HeaderToolContentProperty, item);
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportTabItem() => Style = (Style) Application.Current.TryFindResource("DragSupportTabItemStyle");

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal proc esses call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if ( GetTemplateChild("TabItemCloseButton") is Button closeButton )
        closeButton.PreviewMouseDown += CloseButtonClick;

      if ( !(GetTemplateChild("GridHeader") is Grid headerGrid) )
        return;

      headerGrid.MouseDown += HeaderGridMiddleMouseButtonDown;
      headerGrid.MouseLeftButtonDown += HeaderGridMouseLeftButtonDown;

      // set special ToolTip for TabItemHeader
      var myToolTip = new ToolTip()
      {
        Style = (Style) FindResource("TabItemToolTipStyle"),
        Content = HeaderToolTip
      };

      if ( HeaderToolTip != null )
        ToolTipService.SetToolTip(headerGrid, myToolTip);
    }

    private void CloseButtonClick(object sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(CloseTabWindowEvent, this));

    private void HeaderGridMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( e.ClickCount == 2 )
        RaiseEvent(new RoutedEventArgs(TabHeaderDoubleClickEvent, this));
    }

    private void HeaderGridMiddleMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( e.MiddleButton == MouseButtonState.Pressed )
        RaiseEvent(new RoutedEventArgs(CloseTabWindowEvent, this));
    }
  }
}
