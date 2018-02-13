using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Org.Vs.TailForWin.BaseView.Interfaces;
using Org.Vs.TailForWin.Business.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;


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
        if ( _isEnabled == value )
          return;

        _isEnabled = value;
        OnPropertyChanged(nameof(IsEnabled));
      }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="optionRoot">Root option</param>
    /// <param name="children">IEnumerable of <see cref="TreeNodeOptionViewModel"/></param>
    /// <param name="isEnabled">Node is enabled</param>
    public TreeNodeOptionViewModel(IOptionPage optionRoot, IEnumerable<TreeNodeOptionViewModel> children, bool isEnabled = true)
    {
      Name = optionRoot.PageTitle;
      OptionPage = optionRoot;
      IsEnabled = isEnabled;
      _children = new ObservableCollection<TreeNodeOptionViewModel>(children);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="option">Option page</param>
    /// <param name="isEnabled">Node is enabled</param>
    public TreeNodeOptionViewModel(IOptionPage option, bool isEnabled = true)
      : this(option, Enumerable.Empty<TreeNodeOptionViewModel>(), isEnabled)
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
      if ( Children != null )
      {
        foreach ( var child in Children )
        {
          child.Dispose();
        }
      }

      GC.SuppressFinalize(this);
    }
  }
}
