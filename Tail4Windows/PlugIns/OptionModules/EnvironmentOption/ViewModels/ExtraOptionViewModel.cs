using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Extras option view model
  /// </summary>
  public class ExtraOptionViewModel : NotifyMaster
  {
    #region Properties

    private string _logLineLimit;

    /// <summary>
    /// LogLineLimit
    /// </summary>
    public string LogLineLimit
    {
      get => _logLineLimit;
      set
      {
        if ( Equals(_logLineLimit, value) )
          return;

        _logLineLimit = value;
        OnPropertyChanged(nameof(LogLineLimit));
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExtraOptionViewModel() => LogLineLimit = SettingsHelperController.CurrentSettings.LogLineLimit == -1 ?
      Application.Current.TryFindResource("ExtrasLogLineUnlimited").ToString() :
      $"{SettingsHelperController.CurrentSettings.LogLineLimit:N0} {Application.Current.TryFindResource("ExtrasLogLineLines")}";

    #region Commands

    private ICommand _slideValueChangedCommand;

    /// <summary>
    /// SlideValueChangedCommand
    /// </summary>
    public ICommand SliderValueChangedCommand => _slideValueChangedCommand ?? (_slideValueChangedCommand = new RelayCommand(ExecuteSliderValueChangedCommand));

    #endregion

    #region Command functions

    private void ExecuteSliderValueChangedCommand(object parameter)
    {
      if ( !(parameter is RoutedPropertyChangedEventArgs<double> args) )
        return;

      var value = Equals((int) args.NewValue, EnvironmentContainer.UnlimitedLogLineValue) ?
        Application.Current.TryFindResource("ExtrasLogLineUnlimited").ToString() : $"{args.NewValue:N0} {Application.Current.TryFindResource("ExtrasLogLineLines")}";

      LogLineLimit = value;
      args.Handled = true;
    }

    #endregion
  }
}
