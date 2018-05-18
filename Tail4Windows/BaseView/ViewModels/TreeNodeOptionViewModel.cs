using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// TreeNode view model
  /// </summary>
  public class TreeNodeOptionViewModel : NotifyMaster, ITreeNodeViewModel
  {
    private readonly ObservableCollection<TreeNodeOptionViewModel> _children;

    /// <summary>
    /// Children
    /// </summary>
    public IEnumerable<ITreeNodeViewModel> Children => _children;

    /// <summary>
    /// Name
    /// </summary>
    public string Name
    {
      get;
    }

    /// <summary>
    /// Is leaf
    /// </summary>
    public bool IsLeaf => !Children.Any();

    /// <summary>
    /// Option page
    /// </summary>
    public IOptionPage OptionPage
    {
      get;
    }

    private bool _isExpanded;

    /// <summary>
    /// Is exapnded
    /// </summary>
    public bool IsExpanded
    {
      get => _isExpanded;
      set
      {
        if ( value == _isExpanded )
          return;

        _isExpanded = value;
        OnPropertyChanged(nameof(IsExpanded));
      }
    }

    private bool _isSelected;

    /// <summary>
    /// Is selected
    /// </summary>
    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        if ( value == _isSelected )
          return;

        _isSelected = value;
        OnPropertyChanged(nameof(IsSelected));
      }
    }

    private bool _isEnabled;

    /// <summary>
    /// Is node enabled
    /// </summary>
    public bool IsEnabled
    {
      get => _isEnabled;
      set
      {
        if ( value == _isEnabled )
          return;

        _isEnabled = value;
        OnPropertyChanged(nameof(IsEnabled));
      }
    }

    private string _icon;

    /// <summary>
    /// Icon of node
    /// </summary>
    public string Icon
    {
      get => _icon;
      set
      {
        if ( value == _icon )
          return;

        _icon = value;
        OnPropertyChanged(nameof(Icon));
      }
    }

    private bool _isMatch;

    /// <summary>
    /// Is match
    /// </summary>
    public bool IsMatch
    {
      get => _isMatch;
      set
      {
        if ( value == _isMatch )
          return;

        _isMatch = value;
        OnPropertyChanged(nameof(IsMatch));
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="optionRoot">Root option</param>
    /// <param name="children">IEnumerable of <see cref="TreeNodeOptionViewModel"/></param>
    /// <param name="iconSource">Icon source</param>
    /// <param name="isEnabled">Node is enabled</param>
    public TreeNodeOptionViewModel(IOptionPage optionRoot, IEnumerable<TreeNodeOptionViewModel> children, string iconSource, bool isEnabled = true)
    {
      Name = optionRoot.PageTitle;
      OptionPage = optionRoot;
      IsEnabled = isEnabled;
      Icon = iconSource;
      _children = new ObservableCollection<TreeNodeOptionViewModel>(children);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="option">Option page</param>
    /// <param name="iconSource">Icon source</param>
    /// <param name="isEnabled">Node is enabled</param>
    public TreeNodeOptionViewModel(IOptionPage option, string iconSource, bool isEnabled = true)
      : this(option, Enumerable.Empty<TreeNodeOptionViewModel>(), iconSource, isEnabled)
    {
    }

    /// <summary>
    /// Finish
    /// </summary>
    ~TreeNodeOptionViewModel()
    {
      Dispose();
    }

    /// <summary>
    /// Apply search criteria
    /// </summary>
    /// <param name="criteria">Criteria</param>
    /// <param name="ancestors">Ancestors</param>
    public void ApplyCriteria(string criteria, Stack<ITreeNodeViewModel> ancestors)
    {
      if ( IsCriteriaMatched(criteria) )
      {
        IsMatch = true;

        foreach ( var ancestor in ancestors )
        {
          ancestor.IsMatch = true;
          ancestor.IsExpanded = !string.IsNullOrEmpty(criteria);
          CheckChildren(criteria, ancestor);
        }

        IsExpanded = false;
      }
      else
      {
        IsMatch = false;
      }

      ancestors.Push(this);

      foreach ( var child in Children )
      {
        child.ApplyCriteria(criteria, ancestors);
      }

      ancestors.Pop();
    }

    private void CheckChildren(string criteria, ITreeNodeViewModel parent)
    {
      foreach ( var treeNodeViewModel in parent.Children )
      {
        var child = (TreeNodeOptionViewModel) treeNodeViewModel;

        if ( child.IsLeaf && !child.IsCriteriaMatched(criteria) )
          child.IsMatch = false;

        CheckChildren(criteria, child);
      }
    }

    private bool IsCriteriaMatched(string criteria) => string.IsNullOrEmpty(criteria) || Name.Contains(criteria);

    /// <summary>
    /// Expand all childs
    /// </summary>
    public void Expand()
    {
      Children.Where(p => !p.IsLeaf).ToList().ForEach(
        p =>
        {
          p.IsExpanded = true;
          p.Expand();
        });
    }

    /// <summary>
    /// Collapse all childs
    /// </summary>
    public void Collapse()
    {
      Children.Where(p => p.IsExpanded).ToList().ForEach(
        p =>
        {
          p.IsExpanded = false;
          p.Collapse();
        });
    }

    /// <summary>
    /// Release all resources used by <see cref="TreeNodeOptionViewModel"/>
    /// </summary>
    public void Dispose()
    {
      if ( Children == null )
        return;

      foreach ( var child in Children )
      {
        child.Dispose();
      }
    }
  }
}
