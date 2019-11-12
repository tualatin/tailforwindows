using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;


namespace Org.Vs.TailForWin.Ui.Utils.Extensions
{
  /// <summary>
  /// VisualTreeEnumeration extension
  /// </summary>
  public static class VisualTreeEnumerationExtension
  {
    /// <summary>
    /// Get descendents
    /// </summary>
    /// <param name="root">Root object</param>
    /// <param name="depth">Search depth</param>
    /// <returns>List of <see cref="DependencyObject"/></returns>
    public static IEnumerable<DependencyObject> Descendents(this DependencyObject root, int depth)
    {
      int count = VisualTreeHelper.GetChildrenCount(root);

      for ( int i = 0; i < count; i++ )
      {
        var child = VisualTreeHelper.GetChild(root, i);
        yield return child;

        if ( depth <= 0 )
          continue;

        foreach ( var descendent in Descendents(child, --depth) )
        {
          yield return descendent;
        }
      }
    }

    /// <summary>
    /// Get descendents
    /// </summary>
    /// <param name="root">Root object</param>
    /// <returns>List of <see cref="DependencyObject"/></returns>
    public static IEnumerable<DependencyObject> Descendents(this DependencyObject root) => Descendents(root, int.MaxValue);

    /// <summary>
    /// Get ancestors
    /// </summary>
    /// <param name="root">Root object</param>
    /// <returns>List of <see cref="DependencyObject"/></returns>
    public static IEnumerable<DependencyObject> Ancestors(this DependencyObject root)
    {
      var current = VisualTreeHelper.GetParent(root);

      while ( current != null )
      {
        yield return current;
        current = VisualTreeHelper.GetParent(current);
      }
    }
  }
}
