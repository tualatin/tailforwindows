using System.Windows;
using System.Windows.Input;
using Org.Vs.Tail4Win.Controllers.PlugIns.SmartWatchPopupModule.Events.Delegates;
using Org.Vs.Tail4Win.Core.Data;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.SmartWatchPopupModule.Interfaces
{
  /// <summary>
  /// SmartWatch popup view model interface
  /// </summary>
  public interface ISmartWatchPopupViewModel
  {
    #region Events

    /// <summary>
    /// SmartWatch window closed event
    /// </summary>
    event SmartWatchWindowClosedEventHandler SmartWatchWindowClosed;

    #endregion

    /// <summary>
    /// Button has focus
    /// </summary>
    bool ButtonHasFocus
    {
      get;
    }

    /// <summary>
    /// Gets / sets current <see cref="TailData"/>
    /// </summary>
    TailData CurrentTailData
    {
      get;
      set;
    }

    /// <summary>
    /// Gets / sets current SmartWatch text
    /// </summary>
    string SmartWatchText
    {
      get;
      set;
    }

    /// <summary>
    /// Gets window title
    /// </summary>
    string Title
    {
      get;
    }

    /// <summary>
    /// FileName
    /// </summary>
    string FileName
    {
      get;
      set;
    }

    /// <summary>
    /// MainWindow
    /// </summary>
    Window MainWindow
    {
      get;
      set;
    }

    /// <summary>
    /// Top window position
    /// </summary>
    double Top
    {
      get;
    }

    /// <summary>
    /// Left window position
    /// </summary>
    double Left
    {
      get;
    }

    /// <summary>
    /// Height
    /// </summary>
    double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Width
    /// </summary>
    double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Loaded command
    /// </summary>
    ICommand LoadedCommand
    {
      get;
    }

    /// <summary>
    /// Ignore SmartWatch command
    /// </summary>
    ICommand IgnoreSmartWatchCommand
    {
      get;
    }

    /// <summary>
    /// Open SmartWatch in new tab command
    /// </summary>
    ICommand OpenSmartWatchInNewTabCommand
    {
      get;
    }

    /// <summary>
    /// Open SmartWatch in same tab command
    /// </summary>
    ICommand OpenSmartWatchInSameTabCommand
    {
      get;
    }

    /// <summary>
    /// Autorun command
    /// </summary>
    ICommand AutorunCommand
    {
      get;
    }
  }
}
