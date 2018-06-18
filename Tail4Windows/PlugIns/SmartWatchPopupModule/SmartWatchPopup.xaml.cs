using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule
{
  /// <summary>
  /// Interaction logic for SmartWatchPopup.xaml
  /// </summary>
  public partial class SmartWatchPopup
  {
    private readonly ISmartWatchPopupViewModel _smartWatchPopupViewModel;

    #region Properties

    /// <summary>
    /// Gets / sets current <see cref="TailData"/>
    /// </summary>
    public TailData CurrenTailData
    {
      get => _smartWatchPopupViewModel?.CurrenTailData;
      set
      {
        if ( _smartWatchPopupViewModel == null )
          return;

        _smartWatchPopupViewModel.CurrenTailData = value;
      }
    }

    /// <summary>
    /// Gets / sets current SmartWatch text
    /// </summary>
    public string SmartWatchText
    {
      get => _smartWatchPopupViewModel?.SmartWatchText;
      set
      {
        if ( _smartWatchPopupViewModel == null )
          return;

        _smartWatchPopupViewModel.SmartWatchText = value;
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchPopup()
    {
      InitializeComponent();

      _smartWatchPopupViewModel = (SmartWatchPopupViewModel) DataContext;
    }
  }
}
