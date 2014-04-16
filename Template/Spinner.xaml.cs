using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using System.Windows.Threading;
using System.Threading;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for Spinner.xaml
  /// </summary>
  public partial class Spinner : IDisposable
  {
    private BackgroundWorker counterIncrementUp;
    private BackgroundWorker counterIncrementDown;


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
    }

    private void btnUp_PreviewMouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      counterIncrementUp.RunWorkerAsync ( );
    }

    private void btnUp_PreviewMouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (!counterIncrementUp.IsBusy)
        return;

      counterIncrementUp.CancelAsync ( );
    }

    private void btnDown_PreviewMouseLeftButtonDown (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    private void btnDown_PreviewMouseLeftButtonUp (object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }

    #region Properties

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register ("MaxSpinValue", typeof (int), typeof (Spinner), new PropertyMetadata (10));
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register ("MinSpinValue", typeof (int), typeof (Spinner), new PropertyMetadata (0));
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register ("Increment", typeof (int), typeof (Spinner), new PropertyMetadata (1));
    public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register ("StartIndex", typeof (int), typeof (Spinner), new PropertyMetadata (0));

    /// <summary>
    /// Maximum spinner value
    /// </summary>
    [Bindable (true)]
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
    [Bindable (true)]
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
    [Bindable (true)]
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
    [Bindable (true)]
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

    #endregion

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
      while (counterIncrementUp != null && !counterIncrementUp.CancellationPending)
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

    private void textBoxSpinValue_KeyDown (object sender, System.Windows.Input.KeyEventArgs e)
    {
      e.Handled = (int) e.Key >= 43 || (int) e.Key <= 34 || (int) e.Key == 3;
    }
  }
}
