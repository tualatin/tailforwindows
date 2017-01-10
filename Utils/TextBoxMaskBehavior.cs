using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using log4net;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// TextBoxMaskBehavior
  /// </summary>
  public class TextBoxMaskBehavior
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(TextBoxMaskBehavior));

    #region Control Properties

    public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.RegisterAttached("MinimumValue", typeof(double), typeof(TextBoxMaskBehavior),
      new FrameworkPropertyMetadata(double.NaN, MinimumValueChangedCallback));

    public static double GetMinimumValue(DependencyObject obj)
    {
      return ((double)obj.GetValue(MinimumValueProperty));
    }

    public static void SetMinimumValue(DependencyObject obj, double value)
    {
      obj.SetValue(MinimumValueProperty, value);
    }

    private static void MinimumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBox tb = (d as TextBox);
      ValidateTextBox(tb);
    }

    public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.RegisterAttached("MaximumValue", typeof(double), typeof(TextBoxMaskBehavior),
      new FrameworkPropertyMetadata(double.NaN, MaximumValueChangedCallback));

    public static double GetMaximumValue(DependencyObject obj)
    {
      return ((double)obj.GetValue(MaximumValueProperty));
    }

    public static void SetMaximumValue(DependencyObject obj, double value)
    {
      obj.SetValue(MaximumValueProperty, value);
    }

    private static void MaximumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBox tb = (d as TextBox);
      ValidateTextBox(tb);
    }

    public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(EMaskType), typeof(TextBoxMaskBehavior),
      new FrameworkPropertyMetadata(MaskChangedCallback));

    public static EMaskType GetMask(DependencyObject obj)
    {
      return ((EMaskType)obj.GetValue(MaskProperty));
    }

    public static void SetMask(DependencyObject obj, EMaskType value)
    {
      obj.SetValue(MaskProperty, value);
    }

    private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textBox = e.OldValue as TextBox;

      if (textBox != null)
      {
        textBox.PreviewTextInput -= TextBox_PreviewTextInput;
        DataObject.RemovePastingHandler(textBox, TextBoxPastingEventHandler);
      }

      TextBox tb = (d as TextBox);

      if (tb == null)
        return;

      if ((EMaskType)e.NewValue != EMaskType.Any)
      {
        tb.PreviewTextInput += TextBox_PreviewTextInput;
        DataObject.AddPastingHandler(tb, TextBoxPastingEventHandler);
      }

      ValidateTextBox(tb);
    }

    #endregion

    #region Helper Functions

    private static void ValidateTextBox(TextBox tb)
    {
      if (GetMask(tb) != EMaskType.Any)
        tb.Text = ValidateValue(GetMask(tb), tb.Text);
    }

    private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
    {
      TextBox tb = (sender as TextBox);
      string clipboard = e.DataObject.GetData(typeof(string)) as string;
      clipboard = ValidateValue(GetMask(tb), clipboard);

      if (!string.IsNullOrEmpty(clipboard))
      {
        if (tb != null)
          tb.Text = clipboard;
      }

      e.CancelCommand();
      e.Handled = true;
    }

    private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      TextBox tb = (sender as TextBox);
      bool isValid = IsSymbolValid(GetMask(tb), e.Text);
      e.Handled = !isValid;

      if (!isValid)
        return;

      if (tb != null)
      {
        int caret = tb.CaretIndex;
        string text = tb.Text;
        bool textInserted = false;
        int selectionLength = 0;

        if (tb.SelectionLength > 0)
        {
          text = text.Substring(0, tb.SelectionStart) + text.Substring(tb.SelectionStart + tb.SelectionLength);
          caret = tb.SelectionStart;
        }

        if (string.CompareOrdinal(e.Text, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) == 0)
        {
          while (true)
          {
            int ind = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, StringComparison.Ordinal);

            if (ind == -1)
              break;

            text = text.Substring(0, ind) + text.Substring(ind + 1);

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
            if (caret == 1 && string.CompareOrdinal(string.Empty + text[0], NumberFormatInfo.CurrentInfo.NegativeSign) == 0)
            {
              text = $"{NumberFormatInfo.CurrentInfo.NegativeSign}0{text.Substring(1)}";
              caret++;
            }
          }

          if (caret == text.Length)
          {
            selectionLength = 1;
            textInserted = true;
            text = $"{text}{NumberFormatInfo.CurrentInfo.NumberDecimalSeparator}0";
            caret++;
          }
        }
        else if (string.CompareOrdinal(e.Text, NumberFormatInfo.CurrentInfo.NegativeSign) == 0)
        {
          textInserted = true;

          if (tb.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
          {
            text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);

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
          text = text.Substring(0, caret) + e.Text + ((caret < tb.Text.Length) ? text.Substring(caret) : string.Empty);

          caret++;
        }

        SetCaretPosition(tb, ref text, ref caret);

        tb.Text = text;
        tb.CaretIndex = caret;
        tb.SelectionStart = caret;
        tb.SelectionLength = selectionLength;
      }
      e.Handled = true;
    }

    private static void SetCaretPosition(TextBox tb, ref string text, ref int caret)
    {
      try
      {
        double val = Convert.ToDouble(text);
        double newVal = ValidateLimits(GetMinimumValue(tb), GetMaximumValue(tb), val);

        if (val != newVal)
          text = newVal.ToString();
        else if (val == 0)
          if (!text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
            text = "0";
      }
      catch
      {
        text = "0";
      }

      while (text.Length > 1 && String.CompareOrdinal(text[0].ToString(), '0'.ToString()) == 0 &&
             String.CompareOrdinal(string.Empty + text[1], NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) != 0)
      {
        text = text.Substring(1);

        if (caret > 0)
          caret--;
      }

      while (text.Length > 2 && String.CompareOrdinal(string.Empty + text[0], NumberFormatInfo.CurrentInfo.NegativeSign) == 0 &&
             String.CompareOrdinal(text[1].ToString(), '0'.ToString()) == 0 &&
             String.CompareOrdinal(string.Empty + text[2], NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) != 0)
      {
        text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);

        if (caret > 1)
          caret--;
      }

      if (caret > text.Length)
        caret = text.Length;
    }

    private static string ValidateValue(EMaskType mask, string value)
    {
      if (string.IsNullOrEmpty(value))
        return (string.Empty);

      value = value.Trim();

      switch (mask)
      {
      case EMaskType.Integer:

        try
        {
          Convert.ToInt64(value);

          return (value);
        }
        catch
        {
          LOG.Error("{0} can not convert value '{1}' to integer", System.Reflection.MethodBase.GetCurrentMethod().Name, value);
        }
        return (string.Empty);

      case EMaskType.Decimal:

        try
        {
          Convert.ToDouble(value);

          return (value);
        }
        catch
        {
          LOG.Error("{0} can not convert value '{1}' to decimal", System.Reflection.MethodBase.GetCurrentMethod().Name, value);
        }
        return (string.Empty);
      }
      return (value);
    }

    private static double ValidateLimits(double min, double max, double value)
    {
      if (!min.Equals(double.NaN))
      {
        if (value < min)
          return (min);
      }

      if (max.Equals(double.NaN))
        return (value);

      return (value > max ? (max) : (value));
    }

    private static bool IsSymbolValid(EMaskType mask, string str)
    {
      switch (mask)
      {
      case EMaskType.Any:

      return (true);

      case EMaskType.Integer:

      if (String.CompareOrdinal(str, NumberFormatInfo.CurrentInfo.NegativeSign) == 0)
        return (true);
      break;

      case EMaskType.Decimal:

      if (String.CompareOrdinal(str, NumberFormatInfo.CurrentInfo.NumberDecimalSeparator) == 0 ||
          String.CompareOrdinal(str, NumberFormatInfo.CurrentInfo.NegativeSign) == 0)
        return (true);
      break;
      }

      if (!(mask.Equals(EMaskType.Integer) || mask.Equals(EMaskType.Decimal)))
        return (false);

      return (str.All(Char.IsDigit));
    }

    #endregion
  }
}
