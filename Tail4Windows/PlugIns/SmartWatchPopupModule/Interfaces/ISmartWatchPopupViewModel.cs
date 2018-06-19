using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.PlugIns.SmartWatchPopupModule.Interfaces
{
  /// <summary>
  /// SmartWatch popup view model inteface
  /// </summary>
  public interface ISmartWatchPopupViewModel
  {
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
    TailData CurrenTailData
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
