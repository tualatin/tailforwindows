using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.BaseView.ViewModels
{
  /// <summary>
  /// TreeNode view model
  /// </summary>
  public class TreeNodeViewModel : NotifyMaster
  {
    private readonly ObservableCollection<TreeNodeViewModel> _children;

    /// <summary>
    /// Children
    /// </summary>
    public IEnumerable<TreeNodeViewModel> Children => _children;

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
        _isSelected = value;
        OnPropertyChanged(nameof(IsSelected));
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of node</param>
    /// <param name="children">IEnumerable of <see cref="TreeNodeViewModel"/></param>
    public TreeNodeViewModel(string name, IEnumerable<TreeNodeViewModel> children)
    {
      Name = name;
      _children = new ObservableCollection<TreeNodeViewModel>(children);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="option">Option page</param>
    public TreeNodeViewModel(IOptionPage option)
      : this(option.PageTitle, Enumerable.Empty<TreeNodeViewModel>()) => OptionPage = option;
  }
}
