using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using TailForWin.Data.Enums;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for Spinner.xaml
  /// </summary>
  public partial class Spinner : IDisposable
  {
    private BackgroundWorker counterIncrementUp;
    private BackgroundWorker counterIncrementDown;
    private bool leftmouseButtonDown;


    public void Dispose ()
    {
      if (counterIncrementDown != null)
      {
        counterIncrementDown.Dispose ( );
        counterIncrementDown = null;
      }

      if (counterIncrementUp == null)
        return;

      counterIncrementUp.Dispose ( );
      counterIncrementUp = null;
    }

    public Spinner ()
    {
      InitializeComponent ( );

      counterIncrementUp = new BackgroundWorker { WorkerSupportsCancellation = true };
      counterIncrementUp.DoWork += IncrementUp_DoWork;

      counterIncrementDown = new BackgroundWorker { WorkerSupportsCancellation = true };
      counterIncrementDown.DoWork += IncrementDown_DoWork;

      textBoxSpinValue.DataContext = this;
    }

    #region MouseEvents

    private void btnUp_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      if (counterIncrementUp.IsBusy)
        return;

      leftmouseButtonDown = true;
      counterIncrementUp.RunWorkerAsync ( );
      btnUp.CaptureMouse ( );
    }

    private void btnUp_PreviewMouseLeftButtonUp (object sender, MouseButtonEventArgs e)
    {
      if (!counterIncrementUp.IsBusy)
        return;

      leftmouseButtonDown = false;
      counterIncrementUp.CancelAsync ( );
      btnUp.ReleaseMouseCapture ( );
    }

    private void btnUp_PreviewMouseMove (object sender, MouseEventArgs e)
    {
      if (!leftmouseButtonDown)
        return;
      if (sender.GetType ( ) != typeof (Button))
        return;
      
      Button btnIncrement = sender as Button;

      if (!btnIncrement.IsMouseCaptured)
        return;

      Point mousePoint = PointToScreen (Mouse.GetPosition (this));
      Point relativePoint = btnIncrement.PointToScreen (new Point (0, 0));
      Size sizeBtn = new Size (btnIncrement.ActualWidth, btnIncrement.ActualHeight);
      System.Drawing.Rectangle rc = new System.Drawing.Rectangle ((int) relativePoint.X, (int) relativePoint.Y, (int) sizeBtn.Width, (int) sizeBtn.Height);

      if (!rc.Contains ((int) mousePoint.X, (int) mousePoint.Y))
      {
        if (!counterIncrementUp.IsBusy)
          return;

        counterIncrementUp.CancelAsync ( );
        return;
      }

      if (counterIncrementUp.IsBusy)
        return;

      counterIncrementUp.RunWorkerAsync ( );
    }
    
    private void btnDown_PreviewMouseLeftButtonDown (object sender, MouseButtonEventArgs e)
    {
      if (counterIncrementDown.IsBusy)
        return;

      leftmouseButtonDown = true;
      counterIncrementDown.RunWorkerAsync ( );
      btnDown.CaptureMouse ( );
    }

    private void btnDown_PreviewMouseLeftButtonUp (object sender, MouseButtonEventArgs e)
    {
      if (!counterIncrementDown.IsBusy)
        return;

      leftmouseButtonDown = false;
      counterIncrementDown.CancelAsync ( );
      btnDown.ReleaseMouseCapture ( );
    }

    private void btnDown_PreviewMouseMove (object sender, MouseEventArgs e)
    {
      if (!leftmouseButtonDown)
        return;
      if (sender.GetType ( ) != typeof (Button))
        return;

      Button btnDecrement = sender as Button;

      if (!btnDecrement.IsMouseCaptured)
        return;

      Point mousePoint = PointToScreen (Mouse.GetPosition (this));
      Point relativePoint = btnDecrement.PointToScreen (new Point (0, 0));
      Size sizeBtn = new Size (btnDecrement.ActualWidth, btnDecrement.ActualHeight);
      System.Drawing.Rectangle rc = new System.Drawing.Rectangle ((int) relativePoint.X, (int) relativePoint.Y, (int) sizeBtn.Width, (int) sizeBtn.Height);

      if (!rc.Contains ((int) mousePoint.X, (int) mousePoint.Y))
      {
        if (!counterIncrementDown.IsBusy)
          return;

        counterIncrementDown.CancelAsync ( );
        return;
      }

      if (counterIncrementDown.IsBusy)
        return;

      counterIncrementDown.RunWorkerAsync ( );
    }

    #endregion

    #region Properties

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register ("MaxSpinValue", typeof (int), typeof (Spinner), new PropertyMetadata (10));
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register ("MinSpinValue", typeof (int), typeof (Spinner), new PropertyMetadata (0));
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register ("Increment", typeof (int), typeof (Spinner), new PropertyMetadata (1));
    public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register ("StartIndex", typeof (int), typeof (Spinner), new PropertyMetadata (0));

    /// <summary>
    /// Maximum spinner value
    /// </summary>
    [Category ("Spinner Settings")]
    public int MaxSpinValue
    {
      get
      {
        return ((int) GetValue (MaxValueProperty));
      }
      set
      {
        SetValue (MaxValueProperty, value);
      }
    }

    /// <summary>
    /// Minimum spinner value
    /// </summary>
    [Category ("Spinner Settings")]
    public int MinSpinValue
    {
      get
      {
        return ((int) GetValue (MinValueProperty));
      }
      set
      {
        SetValue (MinValueProperty, value);
      }
    }

    /// <summary>
    /// Increment for spinner
    /// </summary>
    [Category ("Spinner Settings")]
    public int Increment
    {
      get
      {
        return ((int) GetValue (IncrementProperty));
      }
      set
      {
        SetValue (IncrementProperty, value);
      }
    }

    /// <summary>
    /// Start value for spinner
    /// </summary>
    [Category ("Spinner Settings")]
    public int StartIndex
    {
      get
      {
        return ((int) GetValue (StartIndexProperty));
      }
      set
      {
        if (value > MaxSpinValue)
          value = MaxSpinValue;
        if (value < MinSpinValue)
          value = MinSpinValue;

        SetValue (StartIndexProperty, value);
      }
    }

    public static readonly DependencyProperty TextBoxMaskProperty = DependencyProperty.Register ("TextBoxMask", typeof (EMaskType), typeof (Spinner), 
      new PropertyMetadata (EMaskType.Integer));

    /// <summary>
    /// TextBoxMask
    /// </summary>
    [Category ("Spinner Settings")]
    public EMaskType TextBoxMask
    {
      get
      {
        return ((EMaskType) GetValue (TextBoxMaskProperty));
      }
      set
      {
        SetValue (TextBoxMaskProperty, value);
      }
    }

    public static readonly DependencyProperty TextBoxMinValueProperty = DependencyProperty.Register ("TextBoxMinValue", typeof (double), typeof (Spinner),
      new PropertyMetadata ((double) 0));

    /// <summary>
    /// TextBoxMinValue
    /// </summary>
    [Category ("Spinner Settings")]
    public double TextBoxMinValue
    {
      get
      {
        return ((double) GetValue (TextBoxMinValueProperty));
      }
      set
      {
        SetValue (TextBoxMinValueProperty, value);
      }
    }

    public static readonly DependencyProperty TextBoxMaxValueProperty = DependencyProperty.Register ("TextBoxMaxValue", typeof (double), typeof (Spinner),
      new PropertyMetadata ((double) 10));

    /// <summary>
    /// TextBoxMaxValue
    /// </summary>
    [Category ("Spinner Settings")]
    public double TextBoxMaxValue
    {
      get
      {
        return ((double) GetValue (TextBoxMaxValueProperty));
      }
      set
      {
        SetValue (TextBoxMaxValueProperty, value);
      }
    }

    #endregion

    #region Threads

    private void IncrementUp_DoWork (object sender, DoWorkEventArgs e)
    {
      while (counterIncrementUp != null && !counterIncrementUp.CancellationPending)
      {
        this.Dispatcher.Invoke (new Action (() =>
        {
          if (StartIndex <= MaxSpinValue)
            StartIndex = StartIndex + Increment;

          textBoxSpinValue.Text = StartIndex.ToString (CultureInfo.InvariantCulture);
        }), DispatcherPriority.Background);

        Thread.Sleep (100);
      }
    }

    private void IncrementDown_DoWork (object sender, DoWorkEventArgs e)
    {
      while (counterIncrementDown != null && !counterIncrementDown.CancellationPending)
      {
        this.Dispatcher.Invoke (new Action (() =>
        {
          if (StartIndex > MinSpinValue)
            StartIndex = StartIndex - Increment;

          textBoxSpinValue.Text = StartIndex.ToString (CultureInfo.InvariantCulture);
        }), DispatcherPriority.Background);

        Thread.Sleep (100);
      }
    }

    #endregion

    private void textBoxSpinValue_TextChanged (object sender, TextChangedEventArgs e)
    {
      int i;

      if (!int.TryParse (textBoxSpinValue.Text, out i))
        i = MinSpinValue;

      if (i < MinSpinValue)
        i = MinSpinValue;
      if (i > MaxSpinValue)
        i = MaxSpinValue;

      StartIndex = i;
    }

    private void textBoxSpinValue_LostFocus (object sender, RoutedEventArgs e)
    {
      textBoxSpinValue.Text = StartIndex.ToString (CultureInfo.InvariantCulture);
    }

    private void UserControl_Loaded (object sender, RoutedEventArgs e)
    {
      if (StartIndex > MaxSpinValue)
        StartIndex = MaxSpinValue;
      if (StartIndex < MinSpinValue)
        StartIndex = MinSpinValue;

      textBoxSpinValue.Text = StartIndex.ToString (CultureInfo.InvariantCulture);
    }
  }
}
