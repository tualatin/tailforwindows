using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.ViewModels
{
  /// <summary>
  /// SmartWatch popup view model
  /// </summary>
  public class SmartWatchPopupViewModel : NotifyMaster, ISmartWatchPopupViewModel
  {
    #region Properties

    private bool _buttonHasFocus;

    /// <summary>
    /// Button has focus
    /// </summary>
    public bool ButtonHasFocus
    {
      get => _buttonHasFocus;
      set
      {
        _buttonHasFocus = value;
        OnPropertyChanged();
      }
    }

    private string _smartWatchText;

    /// <summary>
    /// Current SmartWatch text
    /// </summary>
    public string SmartWatchText
    {
      get => _smartWatchText;
      set
      {
        if ( Equals(value, _smartWatchText) )
          return;

        _smartWatchText = value;
        OnPropertyChanged();
      }
    }

    private TailData _currenTailData;

    /// <summary>
    /// Gets / sets current <see cref="TailData"/>
    /// </summary>
    public TailData CurrenTailData
    {
      get => _currenTailData;
      set
      {
        _currenTailData = value;
        OnPropertyChanged();
      }
    }

    #endregion

    #region Commands

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand((Window) p)));

    private ICommand _ignoreSmartWatchCommand;

    /// <summary>
    /// Ignore SmartWatch command
    /// </summary>
    public ICommand IgnoreSmartWatchCommand => _ignoreSmartWatchCommand ?? (_ignoreSmartWatchCommand = new RelayCommand(p => ExecuteIgnoreSmartWatchCommand((Window) p)));

    private ICommand _openSmartWatchInNewTabCommand;

    /// <summary>
    /// Open SmartWatch in new tab command
    /// </summary>
    public ICommand OpenSmartWatchInNewTabCommand => _openSmartWatchInNewTabCommand ?? (_openSmartWatchInNewTabCommand = new RelayCommand(p => ExecuteOpenSmartWatchInNewTab((Window) p)));

    private ICommand _openSmartWatchInSameTabCommand;

    /// <summary>
    /// Open SmartWatch in same tab command
    /// </summary>
    public ICommand OpenSmartWatchInSameTabCommand => _openSmartWatchInSameTabCommand ?? (_openSmartWatchInSameTabCommand = new RelayCommand(p => ExecuteOpenSmartWatchInSameTab((Window) p)));

    private ICommand _autorunCommand;

    /// <summary>
    /// Autorun command
    /// </summary>
    public ICommand AutorunCommand => _autorunCommand ?? (_autorunCommand = new RelayCommand(p => ExecuteAutorunCommand()));

    #endregion

    #region Command functions

    private void ExecuteAutorunCommand() => ButtonHasFocus = false;

    private void ExecuteLoadedCommand(Window window)
    {
      window.Activate();
      window.Focus();

      ButtonHasFocus = true;
    }

    private void ExecuteIgnoreSmartWatchCommand(Window window) => window.Close();

    private void ExecuteOpenSmartWatchInNewTab(Window window)
    {
      ExecuteIgnoreSmartWatchCommand(window);
    }

    private void ExecuteOpenSmartWatchInSameTab(Window window)
    {
      ButtonHasFocus = false;
      ExecuteIgnoreSmartWatchCommand(window);
    }

    #endregion
  }
}
