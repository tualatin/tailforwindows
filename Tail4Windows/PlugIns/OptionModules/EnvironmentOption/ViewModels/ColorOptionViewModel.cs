using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Data;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// ColorOption view model
  /// </summary>
  public class ColorOptionViewModel : NotifyMaster
  {
    /// <summary>
    /// Log viewer color data collection
    /// </summary>
    public ObservableCollection<ControlColorData> LogViewerColorData
    {
      get;
    }

    /// <summary>
    /// Statusbar color data collection
    /// </summary>
    public ObservableCollection<ControlColorData> StatusbarColorData
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorOptionViewModel()
    {
      EnvironmentContainer.Instance.CurrentEventManager.UnregisterHandler<ResetDataMessage>(ResetData);
      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<ResetDataMessage>(ResetData);

      LogViewerColorData = new ObservableCollection<ControlColorData>();
      LogViewerColorData.CollectionChanged += ColorDataCollectionChanged;

      StatusbarColorData = new ObservableCollection<ControlColorData>();
      StatusbarColorData.CollectionChanged += ColorDataCollectionChanged;

      AddLogViewerColorOptions();
      AddStatusbarColorOptions();
    }

    private void ColorDataCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if ( e.NewItems != null )
      {
        foreach ( ControlColorData controlColor in e.NewItems )
        {
          controlColor.PropertyChanged += ControlColorPropertyChanged;
        }
      }

      if ( e.OldItems == null )
        return;

      foreach ( ControlColorData controlColor in e.OldItems )
      {
        controlColor.PropertyChanged -= ControlColorPropertyChanged;
      }
    }

    private void ControlColorPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !(sender is ControlColorData mydata) )
        return;

      switch ( mydata.ConfigurationName )
      {
      case "ForegroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.ForegroundColorHex = mydata.Color;
        break;

      case "BackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.BackgroundColorHex = mydata.Color;
        break;

      case "StatusBarInactiveBackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex = mydata.Color;
        break;

      case "StatusBarFileLoadedBackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex = mydata.Color;
        break;

      case "StatusBarTailBackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex = mydata.Color;
        break;
      }
    }

    #region Commands

    #endregion

    #region Command functions

    #endregion

    #region HelperFunctions

    private void ResetData(object sender)
    {
      LogViewerColorData.Clear();
      StatusbarColorData.Clear();

      AddLogViewerColorOptions();
      AddStatusbarColorOptions();
    }

    private void AddLogViewerColorOptions()
    {
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "ForegroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionFontColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.ForegroundColorHex
        });
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "BackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionBackgroundColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.BackgroundColorHex
        });
    }

    private void AddStatusbarColorOptions()
    {
      StatusbarColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "StatusBarInactiveBackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionStatusBarInactiveColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarInactiveBackgroundColorHex
        });
      StatusbarColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "StatusBarFileLoadedBackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionStatusBarLoadedColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarFileLoadedBackgroundColorHex
        });
      StatusbarColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "StatusBarTailBackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionStatusBarBusyColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.StatusBarTailBackgroundColorHex
        });
    }

    #endregion
  }
}
