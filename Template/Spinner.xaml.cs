using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for Spinner.xaml
  /// </summary>
  public partial class Spinner
  {
    public Spinner ()
    {
      InitializeComponent ( );
    }

    private void btnUp_Click (object sender, RoutedEventArgs e)
    {
      if (StartIndex <= MaxSpinValue)
        StartIndex = StartIndex + Increment;

      textBoxSpinValue.Text = StartIndex.ToString (CultureInfo.InvariantCulture);
    }

    private void btnDown_Click (object sender, RoutedEventArgs e)
    {
      if (StartIndex > MinSpinValue)
        StartIndex = StartIndex - Increment;

      textBoxSpinValue.Text = StartIndex.ToString (CultureInfo.InvariantCulture);
    }

    #region Properties

    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register ("MaxSpinValue", typeof (int), typeof (Spinner), new PropertyMetadata (10));
    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register ("MinSpinValue", typeof (int), typeof (Spinner), new PropertyMetadata (0));
    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register ("Increment", typeof (int), typeof (Spinner), new PropertyMetadata (1));
    public static readonly DependencyProperty StartIndexProperty = DependencyProperty.Register ("StartIndex", typeof (int), typeof (Spinner), new PropertyMetadata (0));

    /// <summary>
    /// Maximum spinner value
    /// </summary>
    [Bindable(true)]
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
    [Bindable(true)]
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
    [Bindable(true)]
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
    [Bindable(true)]
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

    private void textBoxSpinValue_TextChanged (object sender, TextChangedEventArgs e)
    {
      int i;

      if (!int.TryParse (textBoxSpinValue.Text, out i))
        i = MinSpinValue;

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
