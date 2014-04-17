using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;


namespace TailForWin.Utils
{
  #region Enum MaskType

  /// <summary>
  /// TextBox mask types
  /// </summary>
  public enum MaskType
  {
    /// <summary>
    /// Any type
    /// </summary>
    Any,

    /// <summary>
    /// Integer
    /// </summary>
    Integer,

    /// <summary>
    /// Decimal
    /// </summary>
    Decimal
  }

  #endregion

  public class TextBoxMaskBehavior
  {
    #region Control Properties

    public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.RegisterAttached ("MinimumValue", typeof (double), typeof (TextBoxMaskBehavior),
      new FrameworkPropertyMetadata (double.NaN, MinimumValueChangedCallback));

    public static double GetMinimumValue (DependencyObject obj)
    {
      return ((double) obj.GetValue (MinimumValueProperty));
    }

    public static void SetMinimumValue (DependencyObject obj, double value)
    {
      obj.SetValue (MinimumValueProperty, value);
    }

    private static void MinimumValueChangedCallback (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBox tb = (d as TextBox);
      ValidateTextBox (tb);
    }

    public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.RegisterAttached ("MaximumValue", typeof (double), typeof (TextBoxMaskBehavior),
      new FrameworkPropertyMetadata (double.NaN, MaximumValueChangedCallback));

    public static double GetMaximumValue (DependencyObject obj)
    {
      return ((double) obj.GetValue (MaximumValueProperty));
    }

    public static void SetMaximumValue (DependencyObject obj, double value)
    {
      obj.SetValue (MaximumValueProperty, value);
    }

    private static void MaximumValueChangedCallback (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBox tb = (d as TextBox);
      ValidateTextBox (tb);
    }

    public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached ("Mask", typeof (MaskType), typeof (TextBoxMaskBehavior),
      new FrameworkPropertyMetadata (MaskChangedCallback));

    public static MaskType GetMask (DependencyObject obj)
    {
      return ((MaskType) obj.GetValue (MaskProperty));
    }

    public static void SetMask (DependencyObject obj, MaskType value)
    {
      obj.SetValue (MaskProperty, value);
    }

    private static void MaskChangedCallback (DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is TextBox)
      {
        (e.OldValue as TextBox).PreviewTextInput -= TextBox_PreviewTextInput;
        DataObject.RemovePastingHandler ((e.OldValue as TextBox), (DataObjectPastingEventHandler) TextBoxPastingEventHandler);
      }

      TextBox tb = (d as TextBox);

      if (tb == null)
        return;

      if ((MaskType) e.NewValue != MaskType.Any)
      {
        tb.PreviewTextInput += TextBox_PreviewTextInput;
        DataObject.AddPastingHandler (tb, (DataObjectPastingEventHandler) TextBoxPastingEventHandler);
      }

      ValidateTextBox (tb);
    }

    #endregion

    #region Helper Functions

    private static void ValidateTextBox (TextBox tb)
    {
      if (GetMask (tb) != MaskType.Any)
        tb.Text = ValidateValue (GetMask (tb), tb.Text);
    }

    private static void TextBoxPastingEventHandler (object sender, DataObjectPastingEventArgs e)
    {
      TextBox tb = (sender as TextBox);
      string clipboard = e.DataObject.GetData (typeof (string)) as string;
      clipboard = ValidateValue (GetMask (tb), clipboard);

      if (!string.IsNullOrEmpty (clipboard))
        tb.Text = clipboard;

      e.CancelCommand ( );
      e.Handled = true;
    }

    private static void TextBox_PreviewTextInput (object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      TextBox tb = (sender as TextBox);
      bool isValid = IsSymbolValid (GetMask (tb), e.Text);
      e.Handled = !isValid;

      if (isValid)
      {
        int caret = tb.CaretIndex;
        string text = tb.Text;
        bool textInserted = false;
        int selectionLength = 0;

        if (tb.SelectionLength > 0)
        {
          text = text.Substring (0, tb.SelectionStart) + text.Substring (tb.SelectionStart + tb.SelectionLength);
          caret = tb.SelectionStart;
        }

        if (String.Compare (e.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, false) == 0)
        {
          while (true)
          {
            int ind = text.IndexOf (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            if (ind == -1)
              break;

            text = text.Substring (0, ind) + text.Substring (ind + 1);

            if (caret > ind)
              caret--;
          }

          if (caret == 0)
          {
            text = "0" + text;
            caret++;
          }
          else
          {
            if (caret == 1 && String.Compare (string.Empty + text[0], NumberFormatInfo.CurrentInfo.NegativeSign, false) == 0)
            {
              text = String.Format ("{0}0{1}", NumberFormatInfo.CurrentInfo.NegativeSign, text.Substring (1));
              caret++;
            }
          }

          if (caret == text.Length)
          {
            selectionLength = 1;
            textInserted = true;
            text = String.Format ("{0}{1}0", text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            caret++;
          }
        }
        else if (String.Compare (e.Text, NumberFormatInfo.CurrentInfo.NegativeSign, false) == 0)
        {
          textInserted = true;

          if (tb.Text.Contains (NumberFormatInfo.CurrentInfo.NegativeSign))
          {
            text = text.Replace (NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);

            if (caret != 0)
              caret--;
          }
          else
          {
            text = NumberFormatInfo.CurrentInfo.NegativeSign + tb.Text;
            caret++;
          }
        }

        if (!textInserted)
        {
          text = text.Substring (0, caret) + e.Text +
              ((caret < tb.Text.Length) ? text.Substring (caret) : string.Empty);

          caret++;
        }

        SetCaretPosition (tb, ref text, ref caret);

        tb.Text = text;
        tb.CaretIndex = caret;
        tb.SelectionStart = caret;
        tb.SelectionLength = selectionLength;
        e.Handled = true;
      }
    }

    private static void SetCaretPosition (TextBox tb, ref string text, ref int caret)
    {
      try
      {
        double val = Convert.ToDouble (text);
        double newVal = ValidateLimits (GetMinimumValue (tb), GetMaximumValue (tb), val);

        if (val != newVal)
          text = newVal.ToString ( );
        else if (val == 0)
          if (!text.Contains (NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
            text = "0";
      }
      catch
      {
        text = "0";
      }

      while (text.Length > 1 && String.Compare (text[0].ToString ( ), '0'.ToString ( ), false) == 0 &&
             String.Compare (string.Empty + text[1], NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, false) != 0)
      {
        text = text.Substring (1);

        if (caret > 0)
          caret--;
      }

      while (text.Length > 2 && String.Compare (string.Empty + text[0], NumberFormatInfo.CurrentInfo.NegativeSign, false) == 0 &&
             String.Compare (text[1].ToString ( ), '0'.ToString ( ), false) == 0 &&
             String.Compare (string.Empty + text[2], NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, false) != 0)
      {
        text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring (2);

        if (caret > 1)
          caret--;
      }

      if (caret > text.Length)
        caret = text.Length;
    }

    private static string ValidateValue (MaskType mask, string value)
    {
      if (string.IsNullOrEmpty (value))
        return (string.Empty);

      value = value.Trim ( );

      switch (mask)
      {
      case MaskType.Integer:

        try
        {
          Convert.ToInt64 (value);

          return (value);
        }
        catch
        {
        }
        return (string.Empty);

      case MaskType.Decimal:

        try
        {
          Convert.ToDouble (value);

          return (value);
        }
        catch
        {
        }
        return (string.Empty);
      }
      return (value);
    }

    private static double ValidateLimits (double min, double max, double value)
    {
      if (!min.Equals (double.NaN))
      {
        if (value < min)
          return (min);
      }

      if (!max.Equals (double.NaN))
      {
        if (value > max)
          return (max);
      }
      return (value);
    }

    private static bool IsSymbolValid (MaskType mask, string str)
    {
      switch (mask)
      {
      case MaskType.Any:

        return (true);

      case MaskType.Integer:

        if (String.Compare (str, NumberFormatInfo.CurrentInfo.NegativeSign, false) == 0)
          return (true);
        break;

      case MaskType.Decimal:

        if (String.Compare (str, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, false) == 0 ||
            String.Compare (str, NumberFormatInfo.CurrentInfo.NegativeSign, false) == 0)
          return (true);
        break;
      }

      if (!(mask.Equals (MaskType.Integer) || mask.Equals (MaskType.Decimal)))
        return (false);

      foreach (char ch in str)
      {
        if (!Char.IsDigit (ch))
          return (false);
      }
      return (true);
    }

    #endregion
  }
}
