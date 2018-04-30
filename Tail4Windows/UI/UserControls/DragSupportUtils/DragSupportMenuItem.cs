using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// DragSupport <see cref="MenuItem"/>
  /// </summary>
  public class DragSupportMenuItem : MenuItem, INotifyPropertyChanged
  {
    private Polygon _menuItemBusyIndicator;
    private Path _menuItemPauseIndicator;

    static DragSupportMenuItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DragSupportMenuItem), new FrameworkPropertyMetadata(typeof(DragSupportMenuItem)));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragSupportMenuItem() => Style = (Style) Application.Current.TryFindResource("DragSupportMenuItemStyle");

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal proc esses call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _menuItemBusyIndicator = GetTemplateChild("MenuItemBusyIndicator") as Polygon;
      _menuItemPauseIndicator = GetTemplateChild("MenuItemPauseIndicator") as Path;

      if ( _menuItemBusyIndicator != null )
        _menuItemBusyIndicator.Visibility = MenuItemBusyIndicator;

      if ( _menuItemPauseIndicator != null )
        _menuItemPauseIndicator.Visibility = Visibility.Collapsed;
    }

    private void TabItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch ( e.PropertyName )
      {
      case "HeaderFullText":

        HeaderContent = TabItem.HeaderFullText;
        SetBusyIndicator();
        break;

      case "TabItemBusyIndicator":

        SetBusyIndicator();
        break;
      }
    }

    private DragSupportTabItem _tabItem;

    /// <summary>
    /// TabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem TabItem
    {
      get => _tabItem;
      set
      {
        if ( Equals(value, _tabItem) )
          return;

        _tabItem = value;
        HeaderContent = _tabItem.HeaderFullText;

        SetBusyIndicator();

        _tabItem.PropertyChanged -= TabItemPropertyChanged;
        _tabItem.PropertyChanged += TabItemPropertyChanged;
      }
    }

    private void SetBusyIndicator()
    {
      if ( Equals(HeaderContent, Application.Current.TryFindResource("NoFile").ToString()) )
      {
        MenuItemBusyIndicator = Visibility.Hidden;
        MenuItemPauseIndicator = Visibility.Collapsed;
      }
      else
      {
        if ( _tabItem.TabItemBusyIndicator == Visibility.Collapsed )
        {
          MenuItemBusyIndicator = Visibility.Collapsed;
          MenuItemPauseIndicator = Visibility.Visible;
        }
        else
        {
          MenuItemBusyIndicator = _tabItem.TabItemBusyIndicator == Visibility.Collapsed ? Visibility.Hidden : _tabItem.TabItemBusyIndicator;
          MenuItemPauseIndicator = Visibility.Collapsed;
        }
      }
    }

    /// <summary>
    /// HeaderToolContentProperty property
    /// </summary>
    public static readonly DependencyProperty HeaderToolContentProperty = DependencyProperty.Register("HeaderContent", typeof(string), typeof(DragSupportMenuItem));

    /// <summary>
    /// Gets/sets HeaderToolContent
    /// </summary>
    public string HeaderContent
    {
      get => (string) GetValue(HeaderToolContentProperty);
      set => SetValue(HeaderToolContentProperty, value);
    }

    /// <summary>
    /// Set MenuItemBusyIndicator property
    /// </summary>
    public static readonly DependencyProperty TabItemBusyIndicatorProperty = DependencyProperty.Register("MenuItemBusyIndicator", typeof(Visibility), typeof(DragSupportMenuItem), new UIPropertyMetadata(Visibility.Hidden, MenuItemBusyIndicatorVisibilityChanged));

    private static void MenuItemBusyIndicatorVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is DragSupportMenuItem menuItem) )
        return;

      if ( menuItem._menuItemBusyIndicator == null )
        return;

      menuItem._menuItemBusyIndicator.Visibility = e.NewValue is Visibility visibility ? visibility : Visibility.Visible;
    }

    /// <summary>
    /// Gets/sets MenuItemBusyIndicator
    /// </summary>
    public Visibility MenuItemBusyIndicator
    {
      get => (Visibility) GetValue(TabItemBusyIndicatorProperty);
      set => SetValue(TabItemBusyIndicatorProperty, value);
    }

    /// <summary>
    /// Set MenuItemPauseIndicator property
    /// </summary>
    public static readonly DependencyProperty TabItemPauseIndicatorProperty = DependencyProperty.Register("MenuItemPauseIndicator", typeof(Visibility), typeof(DragSupportMenuItem), new UIPropertyMetadata(Visibility.Collapsed, MenuItemPauseIndicatorVisibilityChanged));

    private static void MenuItemPauseIndicatorVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(sender is DragSupportMenuItem menuItem) )
        return;

      if ( menuItem._menuItemPauseIndicator == null )
        return;

      menuItem._menuItemPauseIndicator.Visibility = e.NewValue is Visibility visibility ? visibility : Visibility.Visible;
    }

    /// <summary>
    /// Gets/sets MenuItemPauseIndicator
    /// </summary>
    public Visibility MenuItemPauseIndicator
    {
      get => (Visibility) GetValue(TabItemPauseIndicatorProperty);
      set => SetValue(TabItemPauseIndicatorProperty, value);
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
