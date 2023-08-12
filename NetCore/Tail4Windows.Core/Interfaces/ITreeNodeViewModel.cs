namespace Org.Vs.Tail4Win.Core.Interfaces
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
      get;
      set;
    }

    /// <summary>
    /// Is selected
    /// </summary>
    bool IsSelected
    {
      get;
      set;
    }

    /// <summary>
    /// Is node enabled
    /// </summary>
    bool IsEnabled
    {
      get;
      set;
    }

    /// <summary>
    /// Icon of node
    /// </summary>
    string Icon
    {
      get;
      set;
    }

    /// <summary>
    /// Is match
    /// </summary>
    bool IsMatch
    {
      get;
      set;
    }

    /// <summary>
    /// Apply search criteria
    /// </summary>
    /// <param name="criteria">Criteria</param>
    /// <param name="ancestors">Ancestors</param>
    void ApplyCriteria(string criteria, Stack<ITreeNodeViewModel> ancestors);

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
