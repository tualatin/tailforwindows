using System.Windows;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces
{
  /// <summary>
  /// DragDrop to TabWindow interface
  /// </summary>
  public interface IDragDropToTabWindow
  {
    /// <summary>
    /// Is parent window
    /// </summary>
    bool IsParent
    {
      get;
    }

    /// <summary>
    /// On Drag enter
    /// </summary>
    void OnDragEnter();

    /// <summary>
    /// On Drag leave
    /// </summary>
    void OnDrageLeave();

    /// <summary>
    /// Is drag mouse ober
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over<c>True</c> otherwise <c>False</c></returns>
    bool IsDragMouseOver(Point mousePosition);

    /// <summary>
    /// Is drag mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If it is over <c>True</c> otherwise <c>False</c></returns>
    bool IsDragMouseOverTabZone(Point mousePosition);
  }
}
