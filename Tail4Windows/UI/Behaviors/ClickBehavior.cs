using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// Click behavior
  /// </summary>
  public class ClickBehavior : Behavior<Control>
  {
    private readonly DispatcherTimer _timer = new DispatcherTimer();

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ClickBehavior()
    {
      _timer.Interval = TimeSpan.FromSeconds(0.2);
      _timer.Tick += Timer_Tick;
    }

    /// <summary>
    /// Click command property
    /// </summary>
    public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(ClickBehavior));

    /// <summary>
    /// Click command
    /// </summary>
    public ICommand ClickCommand
    {
      get => (ICommand) GetValue(ClickCommandProperty);
      set => SetValue(ClickCommandProperty, value);
    }

    /// <summary>
    /// Double click command property
    /// </summary>
    public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register(nameof(DoubleClickCommand), typeof(ICommand), typeof(ClickBehavior));

    /// <summary>
    /// Double click command
    /// </summary>
    public ICommand DoubleClickCommand
    {
      get => (ICommand) GetValue(DoubleClickCommandProperty);
      set => SetValue(DoubleClickCommandProperty, value);
    }

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.Loaded += AssociatedObject_Loaded;
      AssociatedObject.Unloaded += AssociatedObject_Unloaded;
    }

    private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
    {
      AssociatedObject.Loaded -= AssociatedObject_Loaded;
      AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
    }

    private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
    {
      AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
      AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      _timer.Stop();
      ClickCommand?.Execute(null);
    }

    private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if ( e.ClickCount == 2 )
      {
        _timer.Stop();
        DoubleClickCommand?.Execute(null);
      }
      else
      {
        _timer.Start();
      }
    }
  }
}
