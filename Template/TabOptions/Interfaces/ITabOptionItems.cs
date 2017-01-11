using System;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Template.TabOptions.Interfaces
{
  /// <summary>
  /// Interface tab option items
  /// </summary>
  public interface ITabOptionItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    event EventHandler SaveSettings;

    /// <summary>
    /// Escape key event function
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">KeyEventArgs</param>
    void HandleEsc (object sender, KeyEventArgs e);

    /// <summary>
    /// Save key event function
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    void btnSave_Click (object sender, System.Windows.RoutedEventArgs e);

    /// <summary>
    /// Cancel key event function
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    void btnCancel_Click (object sender, System.Windows.RoutedEventArgs e);
  }
}
