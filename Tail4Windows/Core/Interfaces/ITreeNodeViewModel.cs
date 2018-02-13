using System;
using System.Collections.Generic;


namespace Org.Vs.TailForWin.Core.Interfaces
{
  /// <summary>
  /// Tree node view model interface
  /// </summary>
  public interface ITreeNodeViewModel : IDisposable
  {
    /// <summary>
    /// Children
    /// </summary>
    IEnumerable<ITreeNodeViewModel> Children
    {
      get;
    }

    /// <summary>
    /// Name
    /// </summary>
    string Name
    {
      get;
    }

    /// <summary>
    /// Is leaf
    /// </summary>
    bool IsLeaf
    {
      get;
    }

    /// <summary>
    /// Is exapnded
    /// </summary>
    bool IsExpanded
    {
      get; set;
    }

    /// <summary>
    /// Is selected
    /// </summary>
    bool IsSelected
    {
      get; set;
    }

    /// <summary>
    /// Is node enabled
    /// </summary>
    bool IsEnabled
    {
      get; set;
    }

    /// <summary>
    /// Expand all childs
    /// </summary>
    void Expand();

    /// <summary>
    /// Collapse all childs
    /// </summary>
    void Collapse();
  }
}
