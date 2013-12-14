using System.Windows;
using System.Windows.Controls;
using System;
using TailForWin.Data;
using TailForWin.Controller;
using System.Windows.Input;


namespace TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for OptionsItem.xaml
  /// </summary>
  public partial class OptionsItem: UserControl, ITabOptionItems
  {
    /// <summary>
    /// Close dialog event handler
    /// </summary>
    public event EventHandler CloseDialog;

    /// <summary>
    /// Save application settings event handler
    /// </summary>
    public event EventHandler SaveSettings;

    private bool isInit = false;


    public OptionsItem ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      SetComboBoxes ( );
      SetControls ( );

      isInit = true;
    }
    
    #region ClickEvents
    
    public void btnSave_Click (object sender, RoutedEventArgs e)
    {
      SettingsHelper.TailSettings.LinesRead = spinnerNLines.StartIndex;

      if (SaveSettings != null)
        SaveSettings (this, EventArgs.Empty);
    }

    public void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      if (CloseDialog != null)
        CloseDialog (this, EventArgs.Empty);
    }

    private void btnReset_Click (object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show (Application.Current.FindResource ("QResetSettings") as string, Application.Current.FindResource ("Question") as string, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
      {
        SettingsHelper.SetToDefault ( );
        SetControls ( );
      }
    }

    private void btnProxy_Click (object sender, RoutedEventArgs e)
    {
      Window wnd = Window.GetWindow (this);
      ProxyServer ps = new ProxyServer ( ) { Owner = wnd };

      ps.ShowDialog ( );
    }

    #endregion

    #region Events

    private void UserControl_Loaded (object sender, RoutedEventArgs e)
    {
      gridOptions.DataContext = SettingsHelper.TailSettings;
    }

    public void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click (sender, e);
    }

    private void comboBoxThreadPriority_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultThreadPriority = (System.Threading.ThreadPriority) Enum.Parse (typeof (System.Threading.ThreadPriority), comboBoxThreadPriority.SelectedItem as string);
    }

    private void comboBoxThreadRefreshRate_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultRefreshRate = (SettingsData.ETailRefreshRate) Enum.Parse (typeof (SettingsData.ETailRefreshRate), comboBoxThreadRefreshRate.SelectedItem as string);
    }

    private void comboBoxTimeFormat_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultTimeFormat = SettingsData.GetDescriptionEnum<SettingsData.ETimeFormat> (comboBoxTimeFormat.SelectedItem as string);
    }

    private void comboBoxDateFormat_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (isInit)
        SettingsHelper.TailSettings.DefaultDateFormat = SettingsData.GetDescriptionEnum<SettingsData.EDateFormat> (comboBoxDateFormat.SelectedItem as string);
    }

    #endregion

    #region HelperFunctions

    private void SetControls ()
    {
      comboBoxThreadPriority.SelectedItem = SettingsHelper.TailSettings.DefaultThreadPriority.ToString ( );
      comboBoxThreadRefreshRate.SelectedItem = SettingsHelper.TailSettings.DefaultRefreshRate.ToString ( );
      comboBoxTimeFormat.SelectedItem = SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultTimeFormat);
      comboBoxDateFormat.SelectedItem = SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultDateFormat);

      spinnerNLines.StartIndex = SettingsHelper.TailSettings.LinesRead;
    }

    private void SetComboBoxes ()
    {
      foreach (string priorityName in Enum.GetNames (typeof (System.Threading.ThreadPriority)))
      {
        comboBoxThreadPriority.Items.Add (priorityName);
      }

      comboBoxThreadPriority.SelectedIndex = 0;

      foreach (string refreshName in Enum.GetNames (typeof (SettingsData.ETailRefreshRate)))
      {
        comboBoxThreadRefreshRate.Items.Add (refreshName);
      }

      comboBoxThreadRefreshRate.SelectedIndex = 0;

      foreach (SettingsData.ETimeFormat timeFormat in Enum.GetValues (typeof (SettingsData.ETimeFormat)))
      {
        string item = SettingsData.GetEnumDescription (timeFormat);
        comboBoxTimeFormat.Items.Add (item);
      }

      comboBoxTimeFormat.SelectedIndex = 0;

      foreach (SettingsData.EDateFormat dateFormat in Enum.GetValues (typeof (SettingsData.EDateFormat)))
      {
        string item = SettingsData.GetEnumDescription (dateFormat);
        comboBoxDateFormat.Items.Add (item);
      }

      comboBoxDateFormat.SelectedIndex = 0;
    }

    #endregion
  }
}
