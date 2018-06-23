using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

    /// <summary>
    /// Selection index property
    /// </summary>
    public static readonly DependencyProperty CaretIndexProperty = DependencyProperty.Register(nameof(CaretIndex), typeof(int), typeof(RegexHelper));

    /// <summary>
    /// Caret index
    /// </summary>
    public int CaretIndex
    {
      get => (int) GetValue(CaretIndexProperty);
      set => SetValue(CaretIndexProperty, value);
    }

    /// <summary>
    /// ExtendedMenuItems property
    /// </summary>
    public static readonly DependencyProperty ExtendedMenuItemsProperty = DependencyProperty.Register(nameof(ExtendedMenuItems), typeof(ObservableCollection<MenuItem>), typeof(RegexHelper),
      new PropertyMetadata(OnMenuItemsChanged));

    private static void OnMenuItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( !(e.NewValue is ObservableCollection<MenuItem> items) || items.Count == 0 )
        return;

      if ( !(sender is RegexHelper control) )
        return;

      var menuSource = control.RegexContextMenu;
      int index = 0;

      foreach ( var item in items )
      {
        item.CommandParameter = item.Header;
        item.Command = control.RegexContextMenuItemCommand;

        menuSource.Items.Insert(index, item);
        index++;
      }

      menuSource.Items.Insert(index, new Separator());
    }

    /// <summary>
    /// ExtendedMenuItems
    /// </summary>
    public ObservableCollection<MenuItem> ExtendedMenuItems
    {
      get => (ObservableCollection<MenuItem>) GetValue(ElementHasFocusProperty);
      set => SetValue(ElementHasFocusProperty, value);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Regex 1
    /// </summary>
    public string Regex1
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper1").ToString();
        return string.Format(regex, ".");
      }
    }

    /// <summary>
    /// Regex 2
    /// </summary>
    public string Regex2
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper2").ToString();
        return string.Format(regex, ".*");
      }
    }

    /// <summary>
    /// Regex 3
    /// </summary>
    public string Regex3
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper3").ToString();
        return string.Format(regex, ".+");
      }
    }

    /// <summary>
    /// Regex 4
    /// </summary>
    public string Regex4
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper4").ToString();
        return string.Format(regex, "[abc]");
      }
    }

    /// <summary>
    /// Regex 5
    /// </summary>
    public string Regex5
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper5").ToString();
        return string.Format(regex, "[^abc]");
      }
    }

    /// <summary>
    /// Regex 6
    /// </summary>
    public string Regex6
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper6").ToString();
        return string.Format(regex, "[a-f]");
      }
    }

    /// <summary>
    /// Regex 7
    /// </summary>
    public string Regex7
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper7").ToString();
        return string.Format(regex, @"\w");
      }
    }

    /// <summary>
    /// Regex 8
    /// </summary>
    public string Regex8
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper8").ToString();
        return string.Format(regex, @"\d");
      }
    }

    /// <summary>
    /// Regex 9
    /// </summary>
    public string Regex9
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper9").ToString();
        return string.Format(regex, @"(?([^\r\n])\s)");
      }
    }

    /// <summary>
    /// Regex 10
    /// </summary>
    public string Regex10
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper10").ToString();
        return string.Format(regex, "?");
      }
    }

    /// <summary>
    /// Regex 11
    /// </summary>
    public string Regex11
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper11").ToString();
        return string.Format(regex, "*");
      }
    }

    /// <summary>
    /// Regex 12
    /// </summary>
    public string Regex12
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper12").ToString();
        return string.Format(regex, "+");
      }
    }

    /// <summary>
    /// Regex 13
    /// </summary>
    public string Regex13
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper13").ToString();
        return string.Format(regex, @"\d{3}");
      }
    }

    /// <summary>
    /// Regex 14
    /// </summary>
    public string Regex14
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper14").ToString();
        return string.Format(regex, @"\b");
      }
    }

    /// <summary>
    /// Regex 15
    /// </summary>
    public string Regex15
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper15").ToString();
        return string.Format(regex, "^");
      }
    }

    /// <summary>
    /// Regex 16
    /// </summary>
    public string Regex16
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper16").ToString();
        return string.Format(regex, ".$");
      }
    }

    /// <summary>
    /// Regex 17
    /// </summary>
    public string Regex17
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper17").ToString();
        return string.Format(regex, @"\w\r?\n");

      }
    }

    /// <summary>
    /// Regex 18
    /// </summary>
    public string Regex18
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper18").ToString();
        return string.Format(regex, "(dog|cat)");
      }
    }

    /// <summary>
    /// Regex 19
    /// </summary>
    public string Regex19
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper19").ToString();
        return string.Format(regex, @"\1");
      }
    }

    /// <summary>
    /// Regex 20
    /// </summary>
    public string Regex20
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper20").ToString();
        return string.Format(regex, "0[xX][0-9a-fA-F]+");
      }
    }

    /// <summary>
    /// Regex 21
    /// </summary>
    public string Regex21
    {
      get
      {
        string regex = Application.Current.TryFindResource("RegexHelper21").ToString();
        return string.Format(regex, @"\b[0-9]*\.*[0-9]+\b");
      }
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
      ElementText = !string.IsNullOrWhiteSpace(ElementText) ? ElementText.Insert(CaretIndex, str) : $"{ElementText}{str}";
      ElementHasFocus = true;
    }

    #endregion
  }
}
