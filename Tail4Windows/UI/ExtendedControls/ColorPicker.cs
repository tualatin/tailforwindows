﻿using System.Windows;
using Xceed.Wpf.Toolkit;


namespace Org.Vs.TailForWin.UI.ExtendedControls
{
  /// <summary>
  /// Extended ColorPicker
  /// </summary>
  public class ColorPicker : Xceed.Wpf.Toolkit.ColorPicker
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorPicker()
    {
      UsingAlphaChannel = false;
      AvailableColorsSortingMode = ColorSortingMode.HueSaturationBrightness;
      AvailableColorsHeader = Application.Current.TryFindResource("ColorPickerAvailableColorsHeader").ToString();
      StandardColorsHeader = Application.Current.TryFindResource("ColorPickerStandardColorsHeader").ToString();
      AdvancedButtonHeader = Application.Current.TryFindResource("ColorPickerAdvancedButtonHeader").ToString();
      StandardButtonHeader = Application.Current.TryFindResource("ColorPickerStandardButtonHeader").ToString();

      DisplayColorAndName = true;
      DisplayColorTooltip = true;
    }
  }
}
