using System.Windows;
using Xceed.Wpf.Toolkit;


namespace Org.Vs.TailForWin.Ui.PlugIns.VsControls.ExtendedControls
{
  /// <summary>
  /// Virtual Studios Extended ColorPicker
  /// </summary>
  public class VsColorPickerEx : ColorPicker
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public VsColorPickerEx()
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
