﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Opacity converter
  /// </summary>
  public class OpacityConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted value</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => value != null && (Visibility) value == Visibility.Visible ? 1d : .2d;

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">TargetType</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Current culture</param>
    /// <returns>Back converted value</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
  }
}
