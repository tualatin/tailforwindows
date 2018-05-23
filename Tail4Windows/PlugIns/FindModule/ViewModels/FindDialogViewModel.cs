using System.Windows.Input;
using Org.Vs.TailForWin.Business.DbEngine.Controllers;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.FindModule.ViewModels
{
  /// <summary>
  /// FindDialog view model
  /// </summary>
  public class FindDialogViewModel : NotifyMaster
  {
    private readonly ISettingsDbController _dbController;

    #region Properties

    private double _topPosition;

    /// <summary>
    /// Top position
    /// </summary>
    public double TopPosition
    {
      get => _topPosition;
      set
      {
        _topPosition = value;
        OnPropertyChanged();
      }
    }

    private double _leftPosition;

    /// <summary>
    /// Left position
    /// </summary>
    public double LeftPosition
    {
      get => _leftPosition;
      set
      {
        _leftPosition = value;
        OnPropertyChanged();
      }
    }

    private bool _searchFieldHasFocus;

    /// <summary>
    /// SearchField has focus
    /// </summary>
    public bool SearchFieldHasFocus
    {
      get => _searchFieldHasFocus;
      set
      {
        _searchFieldHasFocus = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindDialogViewModel()
    {
      _dbController = SettingsDbController.Instance;
    }

    #region Commands

    private ICommand _loadeCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadeCommand ?? (_loadeCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    private ICommand _closingCommand;

    /// <summary>
    /// Cloasing command
    /// </summary>
    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand(p => ExecuteClosingCommand()));

    #endregion

    #region Command functions

    private void ExecuteClosingCommand()
    {
      SettingsHelperController.CurrentSettings.FindDialogPositionX = LeftPosition;
      SettingsHelperController.CurrentSettings.FindDialogPositionY = TopPosition;

      _dbController.UpdateFindDialogDbSettings();
    }

    private void ExecuteLoadedCommand()
    {
      TopPosition = SettingsHelperController.CurrentSettings.FindDialogPositionY;
      LeftPosition = SettingsHelperController.CurrentSettings.FindDialogPositionX;

      SearchFieldHasFocus = true;
    }

    #endregion
  }
}
