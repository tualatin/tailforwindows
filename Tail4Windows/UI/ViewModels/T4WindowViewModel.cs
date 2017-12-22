using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.UI.ViewModels
{
  /// <inheritdoc />
  /// <summary>
  /// T4Window view model
  /// </summary>
  public class T4WindowViewModel : NotifyMaster
  {
    /// <summary>
    /// Window title
    /// </summary>
    public string WindowTitle
    {
      get;
      set;
    }

    /// <summary>
    /// Width of main window
    /// </summary>
    public double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Height of main window
    /// </summary>
    public double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4WindowViewModel()
    {
    }

  }
}
