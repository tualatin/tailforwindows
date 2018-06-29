using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.BaseView.Interfaces;
using Org.Vs.TailForWin.Controllers.BaseView.Events.Args;
using Org.Vs.TailForWin.Controllers.BaseView.Events.Delegates;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// BaseWindowStatusbar view model
  /// </summary>
  public class BaseWindowStatusbarViewModel : NotifyMaster, IBaseWindowStatusbarViewModel
  {
    private static BaseWindowStatusbarViewModel instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static BaseWindowStatusbarViewModel Instance => instance ?? (instance = new BaseWindowStatusbarViewModel());

    private BaseWindowStatusbarViewModel()
    {
    }

    #region Events

    /// <summary>
    /// File encoding changed event
    /// </summary>
    public event FileEncodingChangedEventHandler FileEncodingChanged;

    #endregion

    #region Statusbar properties

    private string _currentStatusBarBackgroundColorHex;

    /// <summary>
    /// CurrentStatusBarBackground color as string
    /// </summary>
    public string CurrentStatusBarBackgroundColorHex
    {
      get => _currentStatusBarBackgroundColorHex;
      set
      {
        if ( Equals(value, _currentStatusBarBackgroundColorHex) )
          return;

        _currentStatusBarBackgroundColorHex = value;
        OnPropertyChanged(nameof(CurrentStatusBarBackgroundColorHex));
      }
    }

    private string _currentBusyState;

    /// <summary>
    /// CurrentBusy state
    /// </summary>
    public string CurrentBusyState
    {
      get => _currentBusyState;
      set
      {
        if ( Equals(value, _currentBusyState) )
          return;

        _currentBusyState = value;
        OnPropertyChanged(nameof(CurrentBusyState));
      }
    }

    private Encoding _currentEncoding;

    /// <summary>
    /// Current file <see cref="Encoding"/>
    /// </summary>
    public Encoding CurrentEncoding
    {
      get => _currentEncoding;
      set
      {
        _currentEncoding = value;
        OnPropertyChanged();
      }
    }

    private int _linesRead;

    /// <summary>
    /// Lines read
    /// </summary>
    public int LinesRead
    {
      get => _linesRead;
      set
      {
        _linesRead = value;
        OnPropertyChanged();
      }
    }

    private string _sizeRefreshTime;

    /// <summary>
    /// Size and refresh time
    /// </summary>
    public string SizeRefreshTime
    {
      get => _sizeRefreshTime;
      set
      {
        _sizeRefreshTime = value;
        OnPropertyChanged();
      }
    }

    #endregion

    #region Commands

    private ICommand _selectionChangedCommand;

    /// <summary>
    /// SelectionChanged command
    /// </summary>
    public ICommand SelectionChangedCommand => _selectionChangedCommand ?? (_selectionChangedCommand = new RelayCommand(ExecuteSelectionChangedCommand));

    #endregion

    #region Command functions

    private void ExecuteSelectionChangedCommand(object param)
    {
      if ( !(param is SelectionChangedEventArgs e) )
        return;

      var cb = e.OriginalSource as ComboBox;
      FileEncodingChanged?.Invoke(this, new FileEncodingArgs(cb?.SelectedItem as Encoding));
      _currentEncoding = cb?.SelectedItem as Encoding;
    }

    #endregion
  }
}
