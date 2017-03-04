using System;
using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Template.TabOptions.Interfaces;


namespace Org.Vs.TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for ExtraItem.xaml
  /// </summary>
  public partial class ExtraItem : ITabOptionItems
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(ExtraItem));

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExtraItem()
    {
      InitializeComponent();

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


    public void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        btnCancel_Click(sender, e);
    }

    public void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if(SaveSettings != null)
        CloseDialog(this, EventArgs.Empty);
    }

    public void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      CloseDialog?.Invoke(this, EventArgs.Empty);
    }

    #endregion
  }
}
