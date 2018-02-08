using System.Windows;


namespace Org.Vs.TailForWin.BaseView.UserControls
{
  /// <summary>
  /// Interaction logic for MainWindowQuickSearchBar.xaml
  /// </summary>
  public partial class MainWindowQuickSearchBar
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public MainWindowQuickSearchBar() => InitializeComponent();

    #region DependencyProperties

    /// <summary>
    /// Is focused property
    /// </summary>
    public static readonly DependencyProperty IsQuickSearchTextBoxFocusedProperty = DependencyProperty.RegisterAttached("IsQuickSearchTextBoxFocused", typeof(bool), typeof(MainWindowQuickSearchBar),
      new UIPropertyMetadata(false, OnIsQuickSearchTextBoxFocusedPropertyChanged));


    /// <summary>
    /// Get is quick search textbox focused
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Flag</returns>
    public static bool GetIsQuickSearchTextBoxFocused(DependencyObject obj) => (bool) obj.GetValue(IsQuickSearchTextBoxFocusedProperty);

    /// <summary>
    /// Set is quick search textbox focused
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="value">Flag value</param>
    public static void SetIsQuickSearchTextBoxFocused(DependencyObject obj, bool value) => obj.SetValue(IsQuickSearchTextBoxFocusedProperty, value);

    private static void OnIsQuickSearchTextBoxFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uie = (UIElement) d;

      if ( (bool) e.NewValue )
        uie.Focus();
    }

    #endregion
  }
}
