using System.IO;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.SmartWatchPopupModule.Events.Args;
using Org.Vs.TailForWin.Controllers.PlugIns.SmartWatchPopupModule.Events.Delegates;
using Org.Vs.TailForWin.Controllers.PlugIns.SmartWatchPopupModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.ViewModels
{
  /// <summary>
  /// SmartWatch popup view model
  /// </summary>
  public class SmartWatchPopupViewModel : NotifyMaster, ISmartWatchPopupViewModel
  {
    #region Events

    /// <summary>
    /// SmartWatch window closed event
    /// </summary>
    public event SmartWatchWindowClosedEventHandler SmartWatchWindowClosed;

    #endregion

    #region Properties

    private string _fileName;

    /// <summary>
    /// FileName
    /// </summary>
    public string FileName
    {
      get => _fileName;
      set
      {
        if ( Equals(value, _fileName) )
          return;

        _fileName = value;
        OnPropertyChanged();
      }
    }

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

    private string _title;

    /// <summary>
    /// Gets window title
    /// </summary>
    public string Title
    {
      get => _title;
      set
      {
        if ( Equals(value, _title) )
          return;

        _title = value;
        OnPropertyChanged();
      }
    }

    private TailData _currentTailData;

    /// <summary>
    /// Gets / sets current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _currentTailData;
      set
      {
        _currentTailData = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// MainWindow
    /// </summary>
    public Window MainWindow
    {
      get;
      set;
    }

    private double _top;

    /// <summary>
    /// Top window position
    /// </summary>
    public double Top
    {
      get => _top;
      set
      {
        _top = value;
        OnPropertyChanged();
      }
    }

    private double _left;

    /// <summary>
    /// Left window position
    /// </summary>
    public double Left
    {
      get => _left;
      set
      {
        _left = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Height
    /// </summary>
    public double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Width
    /// </summary>
    public double Width
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchPopupViewModel()
    {
      Height = 135;
      OnPropertyChanged(nameof(Height));
      Width = 420;
      OnPropertyChanged(nameof(Width));

      Title = "SmartWatch";
    }

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
      SetWindowPosition();
      window.Activate();
      window.Focus();

      string message = Application.Current.TryFindResource("SmartWatchHint").ToString();
      SmartWatchText = string.Format(message, Path.GetFileName(FileName), CoreEnvironment.ApplicationTitle);
      Title = $"{Title} - {CurrentTailData.File}";
      ButtonHasFocus = true;
    }

    private void ExecuteIgnoreSmartWatchCommand(Window window) => window.Close();

    private void ExecuteOpenSmartWatchInNewTab(Window window)
    {
      SmartWatchWindowClosed?.Invoke(this, new SmartWatchWindowClosedEventArgs(true, FileName));
      ExecuteIgnoreSmartWatchCommand(window);
    }

    private void ExecuteOpenSmartWatchInSameTab(Window window)
    {
      ButtonHasFocus = false;

      SmartWatchWindowClosed?.Invoke(this, new SmartWatchWindowClosedEventArgs(false, FileName));
      ExecuteIgnoreSmartWatchCommand(window);
    }

    #endregion

    private void SetWindowPosition()
    {
      var mainHeight = MainWindow?.Height ?? Application.Current.MainWindow?.Height;
      var mainWidth = MainWindow?.Width ?? Application.Current.MainWindow?.Width;
      Window mainWindow = MainWindow ?? Application.Current.MainWindow;

      if ( !mainWidth.HasValue || !mainHeight.HasValue )
        return;

      double top = mainHeight.Value / 2 - Height / 2;
      double left = mainWidth.Value / 2 - Width / 2;

      if ( mainWindow?.WindowState == WindowState.Maximized )
      {
        Top = top;
        Left = left;
      }
      else
      {
        var mainLeft = mainWindow?.Left;
        var mainTop = mainWindow?.Top;

        Top = mainTop.Value + top;
        Left = mainLeft.Value + left;
      }
    }
  }
}
