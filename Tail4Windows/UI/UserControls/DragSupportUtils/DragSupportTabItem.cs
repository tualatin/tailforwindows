using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.UI.Converters;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Drag support TabItem
  /// </summary>
  public class DragSupportTabItem : TabItem, INotifyPropertyChanged
  {
    private Polygon _tabItemBusyIndicator;
    private readonly StringToWindowMediaBrushConverter _stringToWindowMediaBrushConverter;

    /// <summary>
    /// TabItem id
    /// </summary>
    public Guid TabItemId
    {
      get;
    }

    static DragSupportTabItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DragSupportTabItem), new FrameworkPropertyMetadata(typeof(DragSupportTabItem)));

    /// <summary>
    /// TabHeaderDoubleClicke event handler
    /// </summary>
    private static readonly RoutedEvent TabHeaderDoubleClickEvent = EventManager.RegisterRoutedEvent("TabHeaderDoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragSupportTabItem));

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
    private static readonly RoutedEvent CloseTabWindowEvent = EventManager.RegisterRoutedEvent("CloseTabWindow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragSupportTabItem));

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
    public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register("HeaderToolTip", typeof(string), typeof(DragSupportTabItem), new UIPropertyMetadata(null));

    /// <summary>
    /// Gets/sets HeaderToolTip
    /// </summary>
    public string HeaderToolTip
    {
      get => (string) GetValue(HeaderToolTipProperty);
      set
      {
        SetValue(HeaderToolTipProperty, value);
        OnPropertyChanged(nameof(HeaderToolTip));
      }
    }

    /// <summary>
    /// Set HeaderToolContentProperty property
    /// </summary>
    public static readonly DependencyProperty HeaderToolContentProperty = DependencyProperty.Register("HeaderContent", typeof(string), typeof(DragSupportTabItem));

    /// <summary>
    /// Gets/sets HeaderToolContent
    /// </summary>
    public string HeaderContent
    {
      get => (string) GetValue(HeaderToolContentProperty);
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
    /// Set TabItemBusyIndicator property
    /// </summary>
    public static readonly DependencyProperty TabItemBusyIndicatorProperty = DependencyProperty.Register("TabItemBusyIndicator", typeof(Visibility), typeof(DragSupportTabItem), new UIPropertyMetadata(Visibility.Collapsed, TabItemBusyIndicatorVisibilityChanged));

    private static void TabItemBusyIndicatorVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is DragSupportTabItem tabItem) )
        return;

      if ( tabItem._tabItemBusyIndicator == null )
        return;

      tabItem._tabItemBusyIndicator.Visibility = e.NewValue is Visibility visibility ? visibility : Visibility.Visible;
    }

    /// <summary>
    /// Gets/sets TabItemBusyIndicator
    /// </summary>
    public Visibility TabItemBusyIndicator
    {
      get => (Visibility) GetValue(TabItemBusyIndicatorProperty);
      set => SetValue(TabItemBusyIndicatorProperty, value);
    }

    /// <summary>
    /// Set ColorPopupIsOpen property
    /// </summary>
    public static readonly DependencyProperty ColorPopupIsOpenProperty = DependencyProperty.Register("ColorPopupIsOpenProperty", typeof(bool), typeof(DragSupportTabItem), new UIPropertyMetadata(false));

    /// <summary>
    /// Gets/sets ColorPopupIsOpen
    /// </summary>
    public bool ColorPopupIsOpen
    {
      get => (bool) GetValue(ColorPopupIsOpenProperty);
      set => SetValue(ColorPopupIsOpenProperty, value);
    }

    /// <summary>
    /// Set TabItem background color as string property
    /// </summary>
    public static readonly DependencyProperty TabItemBackgroundColorStringHexProperty = DependencyProperty.Register("TabItemBackgroundColorStringHexProperty", typeof(string), typeof(DragSupportTabItem), new UIPropertyMetadata("#FFD6DBE9", OnTabItemColorStringHexChanged));

    private static void OnTabItemColorStringHexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is DragSupportTabItem tabItem) )
        return;

      var color = (SolidColorBrush) tabItem._stringToWindowMediaBrushConverter.Convert(tabItem.TabItemBackgroundColorStringHex, typeof(Brush), null, CultureInfo.CurrentCulture);
      tabItem.TabItemBackgroundColor = color;

      tabItem.RaiseEvent(new RoutedEventArgs(TabHeaderBackgroundChangedEvent, tabItem));
    }

    /// <summary>
    /// TabHeaderBackgroundChanged event handler
    /// </summary>
    private static readonly RoutedEvent TabHeaderBackgroundChangedEvent = EventManager.RegisterRoutedEvent("TabHeaderBackgroundChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragSupportTabItem));

    /// <summary>
    /// TabHeaderBackgroundChanged
    /// </summary>
    public event RoutedEventHandler TabHeaderBackgroundChanged
    {
      add => AddHandler(TabHeaderBackgroundChangedEvent, value);
      remove => RemoveHandler(TabHeaderBackgroundChangedEvent, value);
    }

    /// <summary>
    /// Gets/sets background color as string
    /// </summary>
    public string TabItemBackgroundColorStringHex
    {
      get => (string) GetValue(TabItemBackgroundColorStringHexProperty);
      set
      {
        SetValue(TabItemBackgroundColorStringHexProperty, value);
        OnPropertyChanged(nameof(TabItemBackgroundColorStringHex));
      }
    }

    /// <summary>
    /// Set TabItem background color property
    /// </summary>
    public static readonly DependencyProperty TabItemBackgroundColorProperty = DependencyProperty.Register("TabItemBackgroundColorProperty", typeof(SolidColorBrush), typeof(DragSupportTabItem), new UIPropertyMetadata(Application.Current.TryFindResource("BrushSolidLightBlue")));

    /// <summary>
    /// Gets/sets background color
    /// </summary>
    public SolidColorBrush TabItemBackgroundColor
    {
      get => (SolidColorBrush) GetValue(TabItemBackgroundColorProperty);
      set
      {
        SetValue(TabItemBackgroundColorProperty, value);
        OnPropertyChanged(nameof(TabItemBackgroundColor));
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportTabItem()
    {
      Style = (Style) Application.Current.TryFindResource("DragSupportTabItemStyle");
      _stringToWindowMediaBrushConverter = new StringToWindowMediaBrushConverter();
      TabItemId = Guid.NewGuid();
    }

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal proc esses call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if ( GetTemplateChild("TabItemCloseButton") is Button closeButton )
        closeButton.PreviewMouseDown += CloseButtonClick;

      _tabItemBusyIndicator = GetTemplateChild("TabItemBusyIndicator") as Polygon;

      if ( _tabItemBusyIndicator != null )
        _tabItemBusyIndicator.Visibility = TabItemBusyIndicator;

      if ( !(GetTemplateChild("GridHeader") is Grid headerGrid) )
        return;

      headerGrid.MouseDown += HeaderGridMiddleMouseButtonDown;
      headerGrid.MouseLeftButtonDown += HeaderGridMouseLeftButtonDown;
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

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
