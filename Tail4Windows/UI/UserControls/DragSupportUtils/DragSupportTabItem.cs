using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Ui.Utils.Converters;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Drag support <see cref="TabItem"/>
  /// </summary>
  public class DragSupportTabItem : TabItem, INotifyPropertyChanged
  {
    private Polygon _tabItemBusyIndicator;
    private Ellipse _itemChangedIndictor;
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
    private static readonly RoutedEvent TabHeaderDoubleClickEvent = EventManager.RegisterRoutedEvent(nameof(TabHeaderDoubleClick), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
      typeof(DragSupportTabItem));

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
    private static readonly RoutedEvent CloseTabWindowEvent = EventManager.RegisterRoutedEvent(nameof(CloseTabWindow), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
      typeof(DragSupportTabItem));

    /// <summary>
    /// Close tab window when user press the close button in TabHeader
    /// </summary>
    public event RoutedEventHandler CloseTabWindow
    {
      add => AddHandler(CloseTabWindowEvent, value);
      remove => RemoveHandler(CloseTabWindowEvent, value);
    }

    /// <summary>
    /// CloseLeft tabs event handler
    /// </summary>
    private static readonly RoutedEvent CloseLeftTabsEvent = EventManager.RegisterRoutedEvent(nameof(CloseLeftTabs), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
      typeof(DragSupportTabItem));

    /// <summary>
    /// Close left tabs when user press the close button in TabHeader
    /// </summary>
    public event RoutedEventHandler CloseLeftTabs
    {
      add => AddHandler(CloseLeftTabsEvent, value);
      remove => RemoveHandler(CloseLeftTabsEvent, value);
    }

    /// <summary>
    /// CloseRightTabs event handler
    /// </summary>
    private static readonly RoutedEvent CloseRightTabsEvent = EventManager.RegisterRoutedEvent(nameof(CloseRightTabs), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
      typeof(DragSupportTabItem));

    /// <summary>
    /// Close right tabs when user press the close button in TabHeader
    /// </summary>
    public event RoutedEventHandler CloseRightTabs
    {
      add => AddHandler(CloseRightTabsEvent, value);
      remove => RemoveHandler(CloseRightTabsEvent, value);
    }

    /// <summary>
    /// Set HeaderToolTipProperty property
    /// </summary>
    public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register(nameof(HeaderToolTip), typeof(string), typeof(DragSupportTabItem),
      new UIPropertyMetadata(null));

    /// <summary>
    /// Gets/sets HeaderToolTip
    /// </summary>
    public string HeaderToolTip
    {
      get => (string) GetValue(HeaderToolTipProperty);
      set
      {
        SetValue(HeaderToolTipProperty, value);
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Set HeaderToolContentProperty property
    /// </summary>
    public static readonly DependencyProperty HeaderToolContentProperty = DependencyProperty.Register(nameof(HeaderContent), typeof(string), typeof(DragSupportTabItem));

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

        HeaderFullText = value;
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        string item = value.MeasureTextAndCutIt(typeface, FontSize, 160);

        SetValue(HeaderToolContentProperty, item);
        OnPropertyChanged(nameof(HeaderFullText));
      }
    }

    /// <summary>
    /// TabHeader full text
    /// </summary>
    public string HeaderFullText
    {
      get;
      private set;
    }

    /// <summary>
    /// Set TabItemBusyIndicator property
    /// </summary>
    public static readonly DependencyProperty TabItemBusyIndicatorProperty = DependencyProperty.Register(nameof(TabItemBusyIndicator), typeof(Visibility), typeof(DragSupportTabItem),
      new UIPropertyMetadata(Visibility.Collapsed, TabItemBusyIndicatorVisibilityChanged));

    private static void TabItemBusyIndicatorVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is DragSupportTabItem tabItem) )
        return;

      if ( tabItem._tabItemBusyIndicator == null )
        return;

      tabItem._tabItemBusyIndicator.Visibility = e.NewValue is Visibility visibility ? visibility : Visibility.Visible;
      tabItem.OnPropertyChanged(nameof(tabItem.TabItemBusyIndicator));
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
    public static readonly DependencyProperty ColorPopupIsOpenProperty = DependencyProperty.Register(nameof(ColorPopupIsOpen), typeof(bool), typeof(DragSupportTabItem),
      new UIPropertyMetadata(false));

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
    public static readonly DependencyProperty TabItemBackgroundColorStringHexProperty = DependencyProperty.Register("TabItemBackgroundColorStringHexProperty", typeof(string),
      typeof(DragSupportTabItem), new UIPropertyMetadata(DefaultEnvironmentSettings.TabItemHeaderBackgroundColor, OnTabItemColorStringHexChanged));

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
    private static readonly RoutedEvent TabHeaderBackgroundChangedEvent = EventManager.RegisterRoutedEvent(nameof(TabHeaderBackgroundChanged), RoutingStrategy.Bubble,
      typeof(RoutedEventHandler), typeof(DragSupportTabItem));

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
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Set TabItem background color property
    /// </summary>
    public static readonly DependencyProperty TabItemBackgroundColorProperty = DependencyProperty.Register(nameof(TabItemBackgroundColor), typeof(SolidColorBrush),
      typeof(DragSupportTabItem), new UIPropertyMetadata(Application.Current.TryFindResource("BrushSolidLightBlue")));

    /// <summary>
    /// Gets/sets background color
    /// </summary>
    public SolidColorBrush TabItemBackgroundColor
    {
      get => (SolidColorBrush) GetValue(TabItemBackgroundColorProperty);
      set
      {
        SetValue(TabItemBackgroundColorProperty, value);
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Set ItemChangedIndicator property
    /// </summary>
    public static readonly DependencyProperty ItemChangedIndicatorProperty = DependencyProperty.Register(nameof(ItemChangedIndicator), typeof(Visibility), typeof(DragSupportTabItem),
      new UIPropertyMetadata(Visibility.Collapsed, ItemChangedIndicatorVisibilityChanged));

    private static void ItemChangedIndicatorVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is DragSupportTabItem tabItem) )
        return;

      if ( tabItem._itemChangedIndictor == null )
        return;

      tabItem._itemChangedIndictor.Visibility = e.NewValue is Visibility visibility ? visibility : Visibility.Visible;
      tabItem.OnPropertyChanged(nameof(tabItem.ItemChangedIndicator));
    }

    /// <summary>
    /// Gets/sets ItemChangedIndicator
    /// </summary>
    public Visibility ItemChangedIndicator
    {
      get => (Visibility) GetValue(ItemChangedIndicatorProperty);
      set => SetValue(ItemChangedIndicatorProperty, value);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportTabItem()
    {
      Style = (Style) Application.Current.TryFindResource("DragSupportTabItemStyle");
      _stringToWindowMediaBrushConverter = new StringToWindowMediaBrushConverter();
      TabItemId = Guid.NewGuid();

      ContextMenu = new ContextMenu();

      ContextMenu.Items.Add(new MenuItem { Header = Application.Current.TryFindResource("DragSupportTabItemCloseTab"), Command = CloseCurrentTabItemCommand });
      ContextMenu.Items.Add(new MenuItem { Header = Application.Current.TryFindResource("DragSupportTabItemCloseLeftTabs"), Command = CloseLeftTabsCommand });
      ContextMenu.Items.Add(new MenuItem { Header = Application.Current.TryFindResource("DragSupportTabItemCloseRightTabs"), Command = CloseRightTabsCommand });
    }

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if ( GetTemplateChild("TabItemCloseButton") is Button closeButton )
        closeButton.PreviewMouseDown += CloseButtonClick;

      _tabItemBusyIndicator = GetTemplateChild("TabItemBusyIndicator") as Polygon;

      if ( _tabItemBusyIndicator != null )
        _tabItemBusyIndicator.Visibility = TabItemBusyIndicator;

      _itemChangedIndictor = GetTemplateChild("ItemChangedIndicator") as Ellipse;

      if ( _itemChangedIndictor != null )
        _itemChangedIndictor.Visibility = ItemChangedIndicator;

      if ( !(GetTemplateChild("GridHeader") is Grid headerGrid) )
        return;

      headerGrid.MouseDown += HeaderGridMiddleMouseButtonDown;
      headerGrid.MouseLeftButtonDown += HeaderGridMouseLeftButtonDown;
    }

    private ICommand _closeCurrentTabItemCommand;

    /// <summary>
    /// Close current tab item command
    /// </summary>
    public ICommand CloseCurrentTabItemCommand => _closeCurrentTabItemCommand ?? (_closeCurrentTabItemCommand = new RelayCommand(p => ExecuteCloseCurrentTabItemCommand()));

    private void ExecuteCloseCurrentTabItemCommand() => RaiseEvent(new RoutedEventArgs(CloseTabWindowEvent, this));

    private ICommand _closeLeftTabsCommand;

    /// <summary>
    /// Close left tabs command
    /// </summary>
    public ICommand CloseLeftTabsCommand => _closeLeftTabsCommand ?? (_closeLeftTabsCommand = new RelayCommand(p => ExecuteCloseLeftTabsCommand()));

    private void ExecuteCloseLeftTabsCommand() => RaiseEvent(new RoutedEventArgs(CloseLeftTabsEvent, this));

    private ICommand _closeRightTabsCommand;

    /// <summary>
    /// Close right tabs command
    /// </summary>
    public ICommand CloseRightTabsCommand => _closeRightTabsCommand ?? (_closeRightTabsCommand = new RelayCommand(p => ExecuteCloseRightTabsCommand()));

    private void ExecuteCloseRightTabsCommand() => RaiseEvent(new RoutedEventArgs(CloseRightTabsEvent, this));

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
