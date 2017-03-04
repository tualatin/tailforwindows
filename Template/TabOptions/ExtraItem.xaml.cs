using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
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

      AddItemsToFileSortComboBox();
    }

    private void UCExtraItem_Loaded(object sender, RoutedEventArgs e)
    {
      GridExtraOptions.DataContext = SettingsHelper.TailSettings;

      SetControls();
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


    /// <summary>
    /// Handles Escape button blick
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        btnCancel_Click(sender, e);
    }

    /// <summary>
    /// Save click event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void btnSave_Click(object sender, RoutedEventArgs e)
    {
      SaveSettings?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Cancel click event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    public void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      CloseDialog?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    private void CombBoxFileSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      SettingsHelper.TailSettings.DefaultFileSort = SettingsData.GetDescriptionEnum<EFileSort>(ComboBoxFileSort.SelectedItem as string);
      e.Handled = true;
    }

    #region HelperFunctions

    private void SetControls()
    {
      ComboBoxFileSort.SelectedItem = SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultFileSort);
    }

    private void AddItemsToFileSortComboBox()
    {
      foreach(EFileSort sort in Enum.GetValues(typeof(EFileSort)))
      {
        var item = SettingsData.GetEnumDescription(sort);
        ComboBoxFileSort.Items.Add(item);
      }
    }

    #endregion
  }
}
