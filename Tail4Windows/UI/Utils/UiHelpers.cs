using System.Windows;
using System.Windows.Media;


namespace Org.Vs.TailForWin.UI.Utils
{
  /// <summary>
  /// UI helpers
  /// </summary>
  public static class UiHelpers
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

    /// <summary>
    /// Get child of type
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="depObj"><see cref="DependencyObject"/></param>
    /// <returns>Type</returns>
    public static T GetChildOfType<T>(DependencyObject depObj)
      where T : DependencyObject
    {
      if ( depObj == null )
        return null;

      for ( int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++ )
      {
        var child = VisualTreeHelper.GetChild(depObj, i);

        var result = child as T ?? GetChildOfType<T>(child);

        if ( result != null )
          return result;
      }
      return null;
    }
  }
}
