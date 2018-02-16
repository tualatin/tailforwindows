using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Interaction logic for Spinner.xaml
  /// </summary>
  public partial class Spinner : INotifyPropertyChanged
  {
    private CancellationTokenSource _cts;
    private bool _leftmouseButtonDown;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public Spinner()
    {
      InitializeComponent();
      _cts = new CancellationTokenSource();
    }

    private void UserControlLoaded(object sender, RoutedEventArgs e)
    {
      if ( StartIndex > MaxSpinValue )
        StartIndex = MaxSpinValue;
      if ( StartIndex < MinSpinValue )
        StartIndex = MinSpinValue;
    }

    #region MouseEvents

    private void BtnUpPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _leftmouseButtonDown = true;

      _cts.Dispose();
      _cts = new CancellationTokenSource();

      NotifyTaskCompletion.Create(UpValueAsync());
      BtnUp.CaptureMouse();
    }

    private void BtnUpPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      _leftmouseButtonDown = false;

      _cts.Cancel(false);
      BtnUp.ReleaseMouseCapture();
    }

    private void BtnUpPreviewMouseMove(object sender, MouseEventArgs e)
    {
      if ( !_leftmouseButtonDown )
        return;

      if ( !(sender is Button btnIncrement) )
        return;

      if ( !btnIncrement.IsMouseCaptured )
        return;

      Point mousePoint = PointToScreen(Mouse.GetPosition(this));
      Point relativePoint = btnIncrement.PointToScreen(new Point(0, 0));
      Size sizeBtn = new Size(btnIncrement.ActualWidth, btnIncrement.ActualHeight);
      System.Drawing.Rectangle rc = new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) sizeBtn.Width, (int) sizeBtn.Height);

      if ( !rc.Contains((int) mousePoint.X, (int) mousePoint.Y) )
        _cts.Cancel(false);
    }

    private void BtnDownPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _leftmouseButtonDown = true;

      _cts.Dispose();
      _cts = new CancellationTokenSource();

      NotifyTaskCompletion.Create(DownValueAsync());
      BtnDown.CaptureMouse();
    }

    private void BtnDownPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      _leftmouseButtonDown = false;

      _cts.Cancel(false);
      BtnDown.ReleaseMouseCapture();
    }

    private void BtnDownPreviewMouseMove(object sender, MouseEventArgs e)
    {
      if ( !_leftmouseButtonDown )
        return;

      if ( !(sender is Button btnDecrement) )
        return;

      if ( !btnDecrement.IsMouseCaptured )
        return;

      Point mousePoint = PointToScreen(Mouse.GetPosition(this));
      Point relativePoint = btnDecrement.PointToScreen(new Point(0, 0));
      Size sizeBtn = new Size(btnDecrement.ActualWidth, btnDecrement.ActualHeight);
      System.Drawing.Rectangle rc = new System.Drawing.Rectangle((int) relativePoint.X, (int) relativePoint.Y, (int) sizeBtn.Width, (int) sizeBtn.Height);

      if ( !rc.Contains((int) mousePoint.X, (int) mousePoint.Y) )
        _cts.Cancel(false);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Maximum spinner value 
    /// </summary>
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxSpinValue", typeof(int), typeof(Spinner), new PropertyMetadata(10));

    /// <summary>
    /// Minimum spinner value
    /// </summary>
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinSpinValue", typeof(int), typeof(Spinner), new PropertyMetadata(0));

    /// <summary>
    /// Step value
    /// </summary>
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(int), typeof(Spinner), new PropertyMetadata(1));

    /// <summary>
    /// Start index
    /// </summary>
    public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register("StartIndex", typeof(int), typeof(Spinner), new PropertyMetadata(0, StartIndexChanged));

    private static void StartIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(e.NewValue is int value) || !(sender is Spinner spinner) )
        return;

      spinner.Value = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Maximum spinner value
    /// </summary>
    [Category("Spinner Settings")]
    public int MaxSpinValue
    {
      get => (int) GetValue(MaxValueProperty);
      set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Minimum spinner value
    /// </summary>
    [Category("Spinner Settings")]
    public int MinSpinValue
    {
      get => (int) GetValue(MinValueProperty);
      set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Increment for spinner
    /// </summary>
    [Category("Spinner Settings")]
    public int Increment
    {
      get => (int) GetValue(IncrementProperty);
      set => SetValue(IncrementProperty, value);
    }

    /// <summary>
    /// Start value for spinner
    /// </summary>
    [Category("Spinner Settings")]
    public int StartIndex
    {
      get => (int) GetValue(StartIndexProperty);
      set
      {
        if ( value > MaxSpinValue )
          value = MaxSpinValue;
        if ( value < MinSpinValue )
          value = MinSpinValue;

        SetValue(StartIndexProperty, value);
      }
    }

    /// <summary>
    /// Text box mask
    /// </summary>
    public static readonly DependencyProperty TextBoxMaskProperty = DependencyProperty.Register("TextBoxMask", typeof(EMaskType), typeof(Spinner), new PropertyMetadata(EMaskType.Integer));

    /// <summary>
    /// Text box mask
    /// </summary>
    [Category("Spinner Settings")]
    public EMaskType TextBoxMask
    {
      get => (EMaskType) GetValue(TextBoxMaskProperty);
      set => SetValue(TextBoxMaskProperty, value);
    }

    /// <summary>
    /// Text box min value
    /// </summary>
    public static readonly DependencyProperty TextBoxMinValueProperty = DependencyProperty.Register("TextBoxMinValue", typeof(double), typeof(Spinner), new PropertyMetadata((double) 0));

    /// <summary>
    /// Text box min value
    /// </summary>
    [Category("Spinner Settings")]
    public double TextBoxMinValue
    {
      get => (double) GetValue(TextBoxMinValueProperty);
      set => SetValue(TextBoxMinValueProperty, value);
    }

    /// <summary>
    /// Text box max value
    /// </summary>
    public static readonly DependencyProperty TextBoxMaxValueProperty = DependencyProperty.Register("TextBoxMaxValue", typeof(double), typeof(Spinner), new PropertyMetadata((double) 10));

    /// <summary>
    /// Text box max value
    /// </summary>
    [Category("Spinner Settings")]
    public double TextBoxMaxValue
    {
      get => (double) GetValue(TextBoxMaxValueProperty);
      set => SetValue(TextBoxMaxValueProperty, value);
    }

    private string _value;

    /// <summary>
    /// Value
    /// </summary>
    public string Value
    {
      get => _value;
      set
      {
        _value = value;
        OnPropertyChanged(nameof(Value));
      }
    }

    #endregion

    private void TextBoxSpinValueTextChanged(object sender, TextChangedEventArgs e)
    {
      if ( !int.TryParse(Value, out int i) )
        i = MinSpinValue;
      if ( i < MinSpinValue )
        i = MinSpinValue;
      if ( i > MaxSpinValue )
        i = MaxSpinValue;

      StartIndex = i;
    }

    private void TextBoxSpinValueLostFocus(object sender, RoutedEventArgs e) => Value = StartIndex.ToString(CultureInfo.InvariantCulture);

    private void TextBoxSpinValuePreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if ( !(sender is TextBox) )
        return;

      if ( e.Delta > 0 )
        UpValue();
      else if ( e.Delta < 0 )
        DownValue();
    }

    #region HelperFunctions

    private async Task UpValueAsync()
    {
      try
      {
        while ( !_cts.IsCancellationRequested )
        {
          UpValue();
          await Task.Delay(TimeSpan.FromMilliseconds(120), _cts.Token).ConfigureAwait(false);
        }
      }
      catch
      {
        // Nothing
      }
    }

    private void UpValue()
    {
      Dispatcher.InvokeAsync(
        () =>
        {
          if ( StartIndex <= MaxSpinValue )
            StartIndex += Increment;
        });
    }

    private async Task DownValueAsync()
    {
      try
      {
        while ( !_cts.IsCancellationRequested )
        {
          DownValue();
          await Task.Delay(TimeSpan.FromMilliseconds(120), _cts.Token).ConfigureAwait(false);
        }
      }
      catch
      {
        // Nothing
      }
    }

    private void DownValue()
    {
      Dispatcher.InvokeAsync(
        () =>
        {
          if ( StartIndex > MinSpinValue )
            StartIndex -= Increment;
        });
    }

    #endregion

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
