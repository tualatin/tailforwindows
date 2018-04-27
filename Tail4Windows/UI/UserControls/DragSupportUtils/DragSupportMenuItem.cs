﻿using System.ComponentModel;
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

      if ( _menuItemBusyIndicator != null )
        _menuItemBusyIndicator.Visibility = MenuItemBusyIndicator;
    }

    private void TabItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch ( e.PropertyName )
      {
      case "HeaderFullText":

        HeaderContent = TabItem.HeaderFullText;
        break;

      case "TabItemBusyIndicator":

        MenuItemBusyIndicator = TabItem.TabItemBusyIndicator;
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
        MenuItemBusyIndicator = _tabItem.TabItemBusyIndicator;

        _tabItem.PropertyChanged -= TabItemPropertyChanged;
        _tabItem.PropertyChanged += TabItemPropertyChanged;
      }
    }

    /// <summary>
    /// Set HeaderToolContentProperty property
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
    public static readonly DependencyProperty TabItemBusyIndicatorProperty = DependencyProperty.Register("MenuItemBusyIndicator", typeof(Visibility), typeof(DragSupportMenuItem), new UIPropertyMetadata(Visibility.Collapsed, MenuItemBusyIndicatorVisibilityChanged));

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
