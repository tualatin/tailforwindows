using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for ImportExport.xaml
  /// </summary>
  public partial class ImportExportItem: UserControl, ITabOptionItems
  {
    public ImportExportItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;  
    }

    #region ITabOptionItems Members

    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;


    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }

    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (SaveSettings != null)
        CloseDialog (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    #endregion
  }
}
