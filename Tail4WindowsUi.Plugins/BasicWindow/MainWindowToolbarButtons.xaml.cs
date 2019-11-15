using System;
using System.Windows;


namespace Org.Vs.TailForWin.Ui.PlugIns.BasicWindow
{
  /// <summary>
  /// Interaction logic for MainWindowToolbarButtons.xaml
  /// </summary>
  public partial class MainWindowToolbarButtons
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public MainWindowToolbarButtons() => InitializeComponent();

    #region DependencyProperties

    /// <summary>
    /// MaximizeButton visibility property
    /// </summary>
    public static readonly DependencyProperty MaximizeButtonVisibilityProperty = DependencyProperty.RegisterAttached("MaximizeButtonVisibility", typeof(Visibility), typeof(MainWindowToolbarButtons),
      new UIPropertyMetadata(Visibility.Visible, OnButtonVisibilityPropertyChanged));


    /// <summary>
    /// Get MaximizeButton visibility
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Visibility</returns>
    public static Visibility GetMaximizeButtonVisibility(DependencyObject obj) => (Visibility) obj.GetValue(MaximizeButtonVisibilityProperty);

    /// <summary>
    /// Set MaximizeButton visibility
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="value">Visibility value</param>
    public static void SetMaximizeButtonVisibility(DependencyObject obj, Visibility value) => obj.SetValue(MaximizeButtonVisibilityProperty, value);

    /// <summary>
    /// RestoreButton visibility property
    /// </summary>
    public static readonly DependencyProperty RestoreButtonVisibilityProperty = DependencyProperty.RegisterAttached("RestoreButtonVisibility", typeof(Visibility), typeof(MainWindowToolbarButtons),
      new UIPropertyMetadata(Visibility.Visible, OnButtonVisibilityPropertyChanged));


    /// <summary>
    /// Get RestoreButton visibility
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Visibility</returns>
    public static Visibility GetRestoreButtonVisibility(DependencyObject obj) => (Visibility) obj.GetValue(RestoreButtonVisibilityProperty);

    /// <summary>
    /// Set RestoreButton visibility
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="value">Visibility value</param>
    public static void SetRestoreButtonVisibility(DependencyObject obj, Visibility value) => obj.SetValue(RestoreButtonVisibilityProperty, value);

    /// <summary>
    /// MinimizeButton visibility property
    /// </summary>
    public static readonly DependencyProperty MinimizeButtonVisibilityProperty = DependencyProperty.RegisterAttached("MinimizeButtonVisibility", typeof(Visibility), typeof(MainWindowToolbarButtons),
      new UIPropertyMetadata(Visibility.Visible, OnButtonVisibilityPropertyChanged));


    /// <summary>
    /// Get MinimizeButton visibility
    /// </summary>
    /// <param name="obj">Object</param>
    /// <returns>Visibility</returns>
    public static Visibility GetMinimizeButtonVisibility(DependencyObject obj) => (Visibility) obj.GetValue(MinimizeButtonVisibilityProperty);

    /// <summary>
    /// Set MinimizeButton visibility
    /// </summary>
    /// <param name="obj">Object</param>
    /// <param name="value">Visibility value</param>
    public static void SetMinimizeButtonVisibility(DependencyObject obj, Visibility value) => obj.SetValue(MinimizeButtonVisibilityProperty, value);

    private static void OnButtonVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is MainWindowToolbarButtons toolbarButtons) )
        return;

      if ( e.Property.Name.Equals("MaximizeButtonVisibility") )
      {
        Enum.TryParse<Visibility>(e.NewValue.ToString(), out var visibility);
        toolbarButtons.MaximizeButton.Visibility = visibility;
      }

      if ( e.Property.Name.Equals("RestoreButtonVisibility") )
      {
        Enum.TryParse<Visibility>(e.NewValue.ToString(), out var visibility);
        toolbarButtons.RestoreButton.Visibility = visibility;
      }

      if ( e.Property.Name.Equals("MinimizeButtonVisibility") )
      {
        Enum.TryParse<Visibility>(e.NewValue.ToString(), out var visibility);
        toolbarButtons.MinimizeButton.Visibility = visibility;
      }
    }

    #endregion
  }
}
