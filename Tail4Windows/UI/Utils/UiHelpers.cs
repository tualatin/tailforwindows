using System.Windows;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// UI helpers
  /// </summary>
  public class UiHelpers
  {
    /// <summary>
    /// Finds visual childs
    /// </summary>
    /// <typeparam name="T">Typeof control to find</typeparam>
    /// <param name="rootObject">Root object as <see cref="DependencyObject"/></param>
    /// <returns><see cref="DependencyObject"/></returns>
    public static DependencyObject RecursiveVisualChildFinder<T>(DependencyObject rootObject)
    {
      var child = VisualTreeHelper.GetChild(rootObject, 0);
      return child.GetType() == typeof(T) ? child : RecursiveVisualChildFinder<T>(child);
    }
  }
}
