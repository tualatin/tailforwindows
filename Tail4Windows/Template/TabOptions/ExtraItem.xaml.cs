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

    private bool changedWndStyle;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExtraItem()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;

      AddItemsToFileSortComboBox();
      AddItemsToWindowStyleComboBox();
      AddItemToSmartWatchModeComboBox();
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
      if(changedWndStyle)
      {
        var hintText = FindResource("WndStyleChanged");
        MessageBox.Show(string.Format(hintText.ToString(), LogFile.APPLICATION_CAPTION), LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
      }

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

      if(SettingsHelper.TailSettings.DefaultFileSort != SettingsData.GetDescriptionEnum<EFileSort>(ComboBoxFileSort.SelectedItem as string))
        SettingsHelper.TailSettings.DefaultFileSort = SettingsData.GetDescriptionEnum<EFileSort>(ComboBoxFileSort.SelectedItem as string);

      e.Handled = true;
    }

    private void CombBoxWindowStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      if(SettingsHelper.TailSettings.CurrentWindowStyle != SettingsData.GetDescriptionEnum<EWindowStyle>(ComboBoxWindowStyle.SelectedItem as string))
      {
        SettingsHelper.TailSettings.CurrentWindowStyle = SettingsData.GetDescriptionEnum<EWindowStyle>(ComboBoxWindowStyle.SelectedItem as string);
        changedWndStyle = true;
      }

      e.Handled = true;
    }

    private void ComboBoxSmartWatchMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      if(SettingsHelper.TailSettings.SmartWatchData.Mode != SettingsData.GetDescriptionEnum<ESmartWatchMode>(ComboBoxSmartWatchMode.SelectedItem as string))
        SettingsHelper.TailSettings.SmartWatchData.Mode = SettingsData.GetDescriptionEnum<ESmartWatchMode>(ComboBoxSmartWatchMode.SelectedItem as string);

      e.Handled = true;
    }

    #region HelperFunctions

    private void SetControls()
    {
      ComboBoxFileSort.SelectedItem = SettingsData.GetEnumDescription(SettingsHelper.TailSettings.DefaultFileSort);
      ComboBoxWindowStyle.SelectedItem = SettingsData.GetEnumDescription(SettingsHelper.TailSettings.CurrentWindowStyle);
      ComboBoxSmartWatchMode.SelectedItem = SettingsData.GetEnumDescription(SettingsHelper.TailSettings.SmartWatchData.Mode);
    }

    private void AddItemsToFileSortComboBox()
    {
      foreach(EFileSort sort in Enum.GetValues(typeof(EFileSort)))
      {
        var item = SettingsData.GetEnumDescription(sort);
        ComboBoxFileSort.Items.Add(item);
      }
    }

    private void AddItemsToWindowStyleComboBox()
    {
      foreach(EWindowStyle wndStyle in Enum.GetValues(typeof(EWindowStyle)))
      {
        var item = SettingsData.GetEnumDescription(wndStyle);
        ComboBoxWindowStyle.Items.Add(item);
      }
    }

    private void AddItemToSmartWatchModeComboBox()
    {
      foreach(ESmartWatchMode mode in Enum.GetValues(typeof(ESmartWatchMode)))
      {
        var item = SettingsData.GetEnumDescription(mode);
        ComboBoxSmartWatchMode.Items.Add(item);
      }
    }

    #endregion
  }
}
