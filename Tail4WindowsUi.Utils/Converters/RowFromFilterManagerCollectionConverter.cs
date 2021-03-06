﻿using System;
using System.Windows.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Data;


namespace Org.Vs.TailForWin.Ui.Utils.Converters
{
  /// <summary>
  /// Row from object converter
  /// </summary>
  public class RowFromFilterManagerCollectionConverter : IValueConverter
  {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted type</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      int rn = 0;

      if ( FilterManagerCollectionViewHolder.Cv == null || value == null )
        return rn;

      rn = FilterManagerCollectionViewHolder.Cv.IndexOf(value);

      return rn + 1;
    }

    /// <summary>
    /// Convert back
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="targetType">Target type</param>
    /// <param name="parameter">Parameter</param>
    /// <param name="culture">Culture</param>
    /// <returns>Converted type</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => 1;
  }
}
