using System.Text;
using Org.Vs.TailForWin.BaseView.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;


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

    #endregion
  }
}
