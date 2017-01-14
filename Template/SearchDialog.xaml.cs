using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data.Events;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SearchDialog.xaml
  /// </summary>
  public partial class SearchDialog : INotifyPropertyChanged
  {
    #region Public EventHandler

    /// <summary>
    /// Find next event handler
    /// </summary>
    public event EventHandler FindNextEvent;

    /// <summary>
    /// Find count event handler
    /// </summary>
    public event EventHandler CountSearchEvent;

    /// <summary>
    /// Hide searchbox event handler
    /// </summary>
    public event EventHandler HideSearchBox;

    /// <summary>
    /// Find what text changed event handler
    /// </summary>
    public event EventHandler FindTextChanged;

    /// <summary>
    /// Wrap around event handler
    /// </summary>
    public event EventHandler WrapAround;

    /// <summary>
    /// Bookmark line event handler
    /// </summary>
    public event EventHandler BookmarkLine;

    #endregion

    private readonly FileManagerStructure fmStructure;
    private ObservableDictionary<string, string> searchWords;

    /// <summary>
    /// Combobox search word history
    /// </summary>
    public ObservableDictionary<string, string> SearchWords
    {
      get
      {
        return (searchWords);
      }
      set
      {
        searchWords = value;
        OnPropertyChanged("SearchWords");
      }
    }

    /// <summary>
    /// Wrap search active
    /// </summary>
    public bool WrapSearch
    {
      get
      {
        return (fmStructure.Wrap);
      }
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public SearchDialog()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;

      fmStructure = new FileManagerStructure(true);
      SearchWords = new ObservableDictionary<string, string>();
    }

    /// <summary>
    /// Set title of search window
    /// </summary>
    public string SetTitle
    {
      set
      {
        string advanced = value;
        Title = string.Format("{0} in {1}", Application.Current.FindResource("FindDialogTitle"), advanced);
      }
    }

    /// <summary>
    /// Set statusbar seach count text
    /// </summary>
    /// <param name="count">Current tail log lines</param>
    public void SetStatusBarSearchCountText(int count)
    {
      string myString = Application.Current.FindResource("SearchCount").ToString();
      stsBarSearchCount.Content = string.Format(myString, count);
    }

    #region ClickEvents

    private void btnFindNext_Click(object sender, RoutedEventArgs e)
    {
      stsBarSearchCount.Content = string.Empty;
      DoFindNextEvent();
    }

    private void btnCount_Click(object sender, RoutedEventArgs e)
    {
      DoFindNextEvent(true);

      CountSearchEvent?.Invoke(this, EventArgs.Empty);
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      SaveBoxPosition();

      HideSearchBox?.Invoke(this, EventArgs.Empty);
      Hide();
    }

    private void checkBoxWrapAround_Click(object sender, RoutedEventArgs e)
    {
      WrapAroundBool wrap = new WrapAroundBool();

      if(checkBoxWrapAround.IsChecked == true)
        wrap.Wrap = fmStructure.Wrap = true;
      else
        wrap.Wrap = fmStructure.Wrap = false;

      fmStructure.SaveFindHistoryWrap();

      WrapAround?.Invoke(this, wrap);
    }

    private void checkBoxBookmark_Click(object sender, RoutedEventArgs e)
    {
      if(checkBoxBookmarkLine.IsChecked == true)
      {
        checkBoxBookmarkLine.IsChecked = false;

        checkBoxBookmarkLine_Click(sender, e);
      }

      FindTextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void checkBoxBookmarkLine_Click(object sender, RoutedEventArgs e)
    {
      BookmarkLineBool bookmarkLine = new BookmarkLineBool();

      if(checkBoxBookmarkLine.IsChecked == true)
        bookmarkLine.BookmarkLine = true;
      else
        bookmarkLine.BookmarkLine = false;

      BookmarkLine?.Invoke(this, bookmarkLine);
    }

    #endregion

    #region HelperFunctions

    private void AddSearchWordToDictionary()
    {
      if(SearchWords.ContainsKey(comboBoxWordToFind.Text))
        return;

      SearchWords.Add(comboBoxWordToFind.Text.Trim(), comboBoxWordToFind.Text.Trim());
      fmStructure.SaveFindHistoryName(comboBoxWordToFind.Text.Trim());

      // comboBoxWordToFind.Items.Refresh ( );
    }

    private void SetFocusToTextBox()
    {
      var textBox = (comboBoxWordToFind.Template.FindName("PART_EditableTextBox", comboBoxWordToFind) as TextBox);

      if(textBox == null)
        return;

      textBox.Focus();
      textBox.SelectionStart = textBox.Text.Length;
    }

    private void SaveBoxPosition()
    {
      SettingsHelper.TailSettings.SearchWndXPos = Left;
      SettingsHelper.TailSettings.SearchWndYPos = Top;

      SettingsHelper.SaveSearchWindowPosition();
    }

    #endregion

    #region Events

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        btnClose_Click(sender, e);
    }

    private void DoFindNextEvent(bool count = false)
    {
      SearchData searching = new SearchData
      {
        Count = count,
        SearchBookmarks = false,
        WordToFind = string.Empty
      };

      if(checkBoxBookmark.IsChecked == true)
      {
        searching.SearchBookmarks = true;
      }
      else
      {
        if(!string.IsNullOrWhiteSpace(comboBoxWordToFind.Text))
        {
          AddSearchWordToDictionary();
          searching.WordToFind = comboBoxWordToFind.Text;
        }
      }

      FindNextEvent?.Invoke(this, searching);
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      SaveBoxPosition();
      HideSearchBox?.Invoke(this, EventArgs.Empty);

      e.Cancel = true;
      Hide();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      fmStructure.ReadFindHistory(ref searchWords);
      checkBoxWrapAround.IsChecked = fmStructure.Wrap;
      comboBoxWordToFind.DataContext = this;
      // comboBoxWordToFind.DisplayMemberPath = "Key";

      WrapAroundBool wrap = new WrapAroundBool
      {
        Wrap = fmStructure.Wrap
      };

      WrapAround?.Invoke(this, wrap);
      SetFocusToTextBox();
    }

    private void comboBoxWordToFind_TextChanged(object sender, TextChangedEventArgs e)
    {
      FindTextChanged?.Invoke(this, EventArgs.Empty);
    }

    private void comboBoxWordToFind_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected static void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }

    #endregion
  }
}