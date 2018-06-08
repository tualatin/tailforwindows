using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.UserControls.Interfaces;


namespace Org.Vs.TailForWin.UI.UserControls
{
  /// <summary>
  /// Interaction logic for RegexHelper.xaml
  /// </summary>
  public partial class RegexHelper : IRegexHelper
  {
    #region Dependency properties

    /// <summary>
    /// ElementHasFocus property
    /// </summary>
    public static readonly DependencyProperty ElementHasFocusProperty = DependencyProperty.Register(nameof(ElementHasFocus), typeof(bool), typeof(RegexHelper));

    /// <summary>
    /// ElementHasFocus
    /// </summary>
    public bool ElementHasFocus
    {
      get => (bool) GetValue(ElementHasFocusProperty);
      set => SetValue(ElementHasFocusProperty, value);
    }

    /// <summary>
    /// ElementText property
    /// </summary>
    public static readonly DependencyProperty ElementTextProperty = DependencyProperty.Register(nameof(ElementText), typeof(string), typeof(RegexHelper));

    /// <summary>
    /// ElementText
    /// </summary>
    public string ElementText
    {
      get => (string) GetValue(ElementTextProperty);
      set => SetValue(ElementTextProperty, value);
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RegexHelper() => InitializeComponent();

    #region Commands

    private ICommand _regexContextMenuItemCommand;

    /// <summary>
    /// RegexContextMenuItem command
    /// </summary>
    public ICommand RegexContextMenuItemCommand => _regexContextMenuItemCommand ?? (_regexContextMenuItemCommand = new RelayCommand(ExecuteRegexConteMenuItemCommand));

    private ICommand _regexContextMenuHelpCommand;

    /// <summary>
    /// RegexContextMenuHelp command
    /// </summary>
    public ICommand RegexContextMenuHelpCommand => _regexContextMenuHelpCommand ?? (_regexContextMenuHelpCommand = new RelayCommand(p => ExecuteRegexConteMenuHelpCommand()));

    #endregion

    #region Command functions

    private void ExecuteRegexConteMenuHelpCommand() => Process.Start(new ProcessStartInfo(new Uri("https://regexr.com/").AbsoluteUri));

    private void ExecuteRegexConteMenuItemCommand(object param)
    {
      if ( !(param is string str) )
        return;

      ElementHasFocus = false;
      ElementText = $"{ElementText}{str}";
      ElementHasFocus = true;
    }

    #endregion
  }
}
