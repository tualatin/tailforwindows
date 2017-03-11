using System.Windows;
using System.Windows.Controls;


namespace Org.Vs.TailForWin.Interfaces
{
  /// <summary>
  /// Drag and drop to tab window interface
  /// </summary>
  public interface IDragDropToTabWindow
  {
    /// <summary>
    /// Add TabItem
    /// </summary>
    TabItem TabAdd
    {
      get;
    }

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
