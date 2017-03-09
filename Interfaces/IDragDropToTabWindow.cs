using System.Windows;


namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// Drag and drop to tab window interface
  /// </summary>
  public interface IDragDropToTabWindow
  {
    /// <summary>
    /// Drag enter
    /// </summary>
    void OnDragEnter();

    /// <summary>
    /// Drag leave
    /// </summary>
    void OnDrageLeave();

    /// <summary>
    /// Is drag mouse over
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    bool IsDragMouseOver(Point mousePosition);

    /// <summary>
    /// Is mouse over tab zone
    /// </summary>
    /// <param name="mousePosition">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    bool IsDragMouseOverTabZone(Point mousePosition);
  }
}
