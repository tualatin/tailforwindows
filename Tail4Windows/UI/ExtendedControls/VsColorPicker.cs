using System.Windows;
using Xceed.Wpf.Toolkit;


namespace Org.Vs.TailForWin.UI.ExtendedControls
{
  /// <summary>
  /// Virtual Studios Extended ColorPicker
  /// </summary>
  public class VsColorPicker : ColorPicker
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsColorPicker()
    {
      UsingAlphaChannel = false;
      AvailableColorsSortingMode = ColorSortingMode.HueSaturationBrightness;
      AvailableColorsHeader = Application.Current.TryFindResource("ColorPickerAvailableColorsHeader").ToString();
      StandardColorsHeader = Application.Current.TryFindResource("ColorPickerStandardColorsHeader").ToString();
      AdvancedTabHeader = Application.Current.TryFindResource("ColorPickerAdvancedButtonHeader").ToString();
      StandardTabHeader = Application.Current.TryFindResource("ColorPickerStandardButtonHeader").ToString();

      DisplayColorAndName = true;
      DisplayColorTooltip = true;
    }
  }
}
