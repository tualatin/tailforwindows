using System.Windows;
using TailForWin.Controller;
using TailForWin.Template.ColorPicker;
using System.Windows.Media;
using System;
using System.Windows.Shapes;
using System.Windows.Input;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for ColorItem.xaml
  /// </summary>
  public partial class ColorItem : ITabOptionItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;


    public ColorItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      gridColorItem.DataContext = SettingsHelper.TailSettings;
    }

    #region ClickEvents

    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (SaveSettings != null)
        SaveSettings (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    private void btnColorPickerForeground_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultForegroundColor = SetColor (SettingsHelper.TailSettings.GuiDefaultForegroundColor, foregroundColor);
    }

    private void btnColorPickerBackground_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultBackgroundColor = SetColor (SettingsHelper.TailSettings.GuiDefaultBackgroundColor, backgroundColor);
    }

    private void btnColorPickerInactiveForeground_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultInactiveForegroundColor = SetColor (SettingsHelper.TailSettings.GuiDefaultInactiveForegroundColor, inactiveForegroundColor);
    }

    private void btnColorPickerInactiveBackground_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultInactiveBackgroundColor = SetColor (SettingsHelper.TailSettings.GuiDefaultInactiveBackgroundColor, inactiveBackgroundColor);
    }

    private void btnFindHighlightColor_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultHighlightForegroundColor = SetColor (SettingsHelper.TailSettings.GuiDefaultHighlightForegroundColor, findHighlightForegroundColor);
    }

    private void btnFindHighlightBackgroundColor_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultHighlightBackgroundColor = SetColor (SettingsHelper.TailSettings.GuiDefaultHighlightBackgroundColor, findHighlightBackgroundColor);
    }

    private void btnLineNumbersColor_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultLineNumbersColor = SetColor (SettingsHelper.TailSettings.GuiDefaultLineNumbersColor, lineNumbersColor);
    }

    private void btnHighlightColor_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.DefaultHighlightColor = SetColor (SettingsHelper.TailSettings.GuiDefaultHighlightColor, highlightColor);
    }

    #endregion

    private string SetColor (Brush brushColor, Rectangle rect)
    {
      SolidColorBrush b = brushColor as SolidColorBrush;

      if (b == null)
        return (string.Empty);

      ColorDialog color = new ColorDialog (b.Color)
                          {
                            Owner = Window.GetWindow (this),
                            SelectedColor = ((SolidColorBrush) rect.Fill).Color
                          };

      return (color.ShowDialog ( ).GetValueOrDefault ( ) ? (color.SelectedColorHex) : (string.Empty));
    }

    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }
  }
}
