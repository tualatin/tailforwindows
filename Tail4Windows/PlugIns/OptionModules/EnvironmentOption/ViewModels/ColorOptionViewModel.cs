using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Data;
using Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.UI.Interfaces;

namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// ColorOption view model
  /// </summary>
  public class ColorOptionViewModel : NotifyMaster, IColorOptionViewModel
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
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => throw new NotImplementedException();

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

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

      case "SelectionBackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.SelectionBackgroundColorHex = mydata.Color;
        break;

      case "FindHighlightForegroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightForegroundColorHex = mydata.Color;
        break;

      case "FindHighlightBackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex = mydata.Color;
        break;

      case "LineNumberColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.LineNumberColorHex = mydata.Color;
        break;

      case "LineNumberHighlightColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.LineNumberHighlightColorHex = mydata.Color;
        break;

      case "SplitterBackgroundColorHex":

        SettingsHelperController.CurrentSettings.ColorSettings.SplitterBackgroundColorHex = mydata.Color;
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

      default:

        throw new NotImplementedException();
      }
    }

    #region HelperFunctions

    private void ResetData(ResetDataMessage args)
    {
      if ( !(args.Sender is ImportExportOptionViewModel) )
        return;

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
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "SelectionBackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionSelectionColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.SelectionBackgroundColorHex
        });
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "FindHighlightForegroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionHighlightFontColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightForegroundColorHex
        });
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "FindHighlightBackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionHighlightBackgroundColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.FindHighlightBackgroundColorHex
        });
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "LineNumberColorHex",
          Name = Application.Current.TryFindResource("ColorOptionLineNumberColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.LineNumberColorHex
        });
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "LineNumberHighlightColorHex",
          Name = Application.Current.TryFindResource("ColorOptionLineNumberHighlightColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.LineNumberHighlightColorHex
        });
      LogViewerColorData.Add(
        new ControlColorData
        {
          ConfigurationName = "SplitterBackgroundColorHex",
          Name = Application.Current.TryFindResource("ColorOptionSplitterBackgroundColor").ToString(),
          Color = SettingsHelperController.CurrentSettings.ColorSettings.SplitterBackgroundColorHex
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
