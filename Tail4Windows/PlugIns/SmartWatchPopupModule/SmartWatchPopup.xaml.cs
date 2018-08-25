using System.Windows;
using Org.Vs.TailForWin.Controllers.PlugIns.SmartWatchPopupModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule
{
  /// <summary>
  /// Interaction logic for SmartWatchPopup.xaml
  /// </summary>
  public partial class SmartWatchPopup
  {
    #region Properties

    /// <summary>
    /// Gets / sets current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => SmartWatchPopupViewModel?.CurrentTailData;
      set
      {
        if ( SmartWatchPopupViewModel == null )
          return;

        SmartWatchPopupViewModel.CurrentTailData = value;
      }
    }

    /// <summary>
    /// Gets / sets filename
    /// </summary>
    public string FileName
    {
      get => SmartWatchPopupViewModel?.FileName;
      set
      {
        if ( SmartWatchPopupViewModel == null )
          return;

        SmartWatchPopupViewModel.FileName = value;
      }
    }

    /// <summary>
    /// Main window handle
    /// </summary>
    public Window MainWindow
    {
      get => SmartWatchPopupViewModel?.MainWindow;
      set
      {
        if ( SmartWatchPopupViewModel == null )
          return;

        SmartWatchPopupViewModel.MainWindow = value;
      }
    }

    /// <summary>
    /// Gets SmartWatchPopupViewModel
    /// </summary>
    public ISmartWatchPopupViewModel SmartWatchPopupViewModel
    {
      get;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchPopup()
    {
      InitializeComponent();

      SmartWatchPopupViewModel = (SmartWatchPopupViewModel) DataContext;
    }
  }
}
