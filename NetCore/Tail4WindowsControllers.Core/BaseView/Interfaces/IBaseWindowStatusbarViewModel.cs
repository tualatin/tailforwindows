using System.Text;
using Org.Vs.Tail4Win.Controllers.BaseView.Events.Delegates;

namespace Org.Vs.Tail4Win.Controllers.BaseView.Interfaces
{
  /// <summary>
  /// BaseWindowStatusbar view model interface
  /// </summary>
  public interface IBaseWindowStatusbarViewModel
  {
    /// <summary>
    /// File encoding changed event
    /// </summary>
    event FileEncodingChangedEventHandler FileEncodingChanged;

    /// <summary>
    /// CurrentStatusBarBackground color as string
    /// </summary>
    string CurrentBusyState
    {
      get;
      set;
    }

    /// <summary>
    /// CurrentBusy state
    /// </summary>
    string CurrentStatusBarBackgroundColorHex
    {
      get;
      set;
    }

    /// <summary>
    /// Current file <see cref="Encoding"/>
    /// </summary>
    Encoding CurrentEncoding
    {
      get;
      set;
    }

    /// <summary>
    /// Lines read
    /// </summary>
    int LinesRead
    {
      get;
      set;
    }

    /// <summary>
    /// Size and refresh time
    /// </summary>
    string SizeRefreshTime
    {
      get;
      set;
    }

    /// <summary>
    /// Selected lines
    /// </summary>
    int SelectedLines
    {
      get;
      set;
    }
  }
}
