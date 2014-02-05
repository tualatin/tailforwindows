using System.Windows;
using TailForWin.Controller;
using System;
using TailForWin.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Specialized;
using TailForWin.Utils;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SearchDialog.xaml
  /// </summary>
  public partial class SearchDialog: Window, INotifyPropertyChanged
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

    private FileManagerStructure fmStructure;
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
        OnPropertyChanged ("SearchWords");
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


    public SearchDialog ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      fmStructure = new FileManagerStructure (true);
      SearchWords = new ObservableDictionary<string, string> ( );
    }

    /// <summary>
    /// Set title of search window
    /// </summary>
    public string SetTitle
    {
      set
      {
        string advanced = value;
        Title = string.Format ("{0} in {1}", Application.Current.FindResource ("FindDialogTitle"), advanced);
      }
    }

    public void SetStatusBarSearchCountText (int count)
    {
      string myString = Application.Current.FindResource ("SearchCount").ToString ( ); 
      stsBarSearchCount.Content = string.Format (myString, count);
    }

    #region ClickEvents

    private void btnFindNext_Click (object sender, RoutedEventArgs e)
    {
      stsBarSearchCount.Content = string.Empty;
      DoFindNextEvent ( );
    }

    private void btnCount_Click (object sender, RoutedEventArgs e)
    {
      DoFindNextEvent (true);

      if (CountSearchEvent != null)
        CountSearchEvent (this, EventArgs.Empty);
    }

    private void btnClose_Click (object sender, RoutedEventArgs e)
    {
      if (HideSearchBox != null)
        HideSearchBox (this, EventArgs.Empty);

      Hide ( );
    }

    private void checkBoxWrapAround_Click (object sender, RoutedEventArgs e)
    {
      WrapAroundBool wrap = new WrapAroundBool ( );

      if (checkBoxWrapAround.IsChecked == true)
        wrap.Wrap = fmStructure.Wrap = true;
      else
        wrap.Wrap = fmStructure.Wrap = false;

      fmStructure.SaveFindHistoryWrap ( );

      if (WrapAround != null)
        WrapAround (this, wrap);
    }

    private void checkBoxBookmark_Click (object sender, RoutedEventArgs e)
    {
      if (checkBoxBookmarkLine.IsChecked == true)
      {
        checkBoxBookmarkLine.IsChecked = false;

        checkBoxBookmarkLine_Click (sender, e);
      }

      if (FindTextChanged != null)
        FindTextChanged (this, EventArgs.Empty);
    }

    private void checkBoxBookmarkLine_Click (object sender, RoutedEventArgs e)
    {
      BookmarkLineBool bookmarkLine = new BookmarkLineBool ( );

      if (checkBoxBookmarkLine.IsChecked == true)
        bookmarkLine.BookmarkLine = true;
      else
        bookmarkLine.BookmarkLine = false;

      if (BookmarkLine != null)
        BookmarkLine (this, bookmarkLine);
    }

    #endregion

    #region HelperFunctions

    private void AddSearchWordToDictionary ()
    {
      if (!SearchWords.ContainsKey (comboBoxWordToFind.Text))
      {
        SearchWords.Add (comboBoxWordToFind.Text.Trim ( ), comboBoxWordToFind.Text.Trim ( ));
        fmStructure.SaveFindHistoryName (comboBoxWordToFind.Text.Trim ( ));

        // comboBoxWordToFind.Items.Refresh ( );
      }
    }

    private void SetFocusToTextBox ()
    {
      var textBox = (comboBoxWordToFind.Template.FindName ("PART_EditableTextBox", comboBoxWordToFind) as TextBox);

      if (textBox != null)
      {
        textBox.Focus ( );
        textBox.SelectionStart = textBox.Text.Length;
      }
    }

    #endregion

    #region Events

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnClose_Click (sender, e);
    }

    private void DoFindNextEvent (bool count = false)
    {
      SearchData searching = new SearchData ( )
      {
        Count = count,
        SearchBookmarks = false,
        WordToFind = string.Empty
      };

      if (checkBoxBookmark.IsChecked == true)
        searching.SearchBookmarks = true;
      else
      {
        AddSearchWordToDictionary ( );
        searching.WordToFind = comboBoxWordToFind.Text;
      }

      if (FindNextEvent != null)
        FindNextEvent (this, searching);
    }

    private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
    {
      SettingsHelper.TailSettings.SearchWndXPos = Left;
      SettingsHelper.TailSettings.SearchWndYPos = Top;

      SettingsHelper.SaveSearchWindowPosition ( );

      if (HideSearchBox != null)
        HideSearchBox (this, EventArgs.Empty);

      e.Cancel = true;
      Hide ( );
    }

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      fmStructure.ReadFindHistory (ref searchWords);
      checkBoxWrapAround.IsChecked = fmStructure.Wrap;
      comboBoxWordToFind.DataContext = this;
      // comboBoxWordToFind.DisplayMemberPath = "Key";

      WrapAroundBool wrap = new WrapAroundBool ( ) { Wrap = fmStructure.Wrap };

      if (WrapAround != null)
        WrapAround (this, wrap);

      SetFocusToTextBox ( );
    }

    private void comboBoxWordToFind_TextChanged (object sender, TextChangedEventArgs e)
    {
      if (FindTextChanged != null)
        FindTextChanged (this, EventArgs.Empty);
    }

    private void comboBoxWordToFind_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged (string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;

      if (handler != null)
        handler (this, new PropertyChangedEventArgs (name));
    }

    protected void ItemPropertyChanged (object sender, PropertyChangedEventArgs e)
    {
      NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset);
    }

    #endregion
  }

  public class WrapAroundBool : EventArgs
  {
    /// <summary>
    /// Wrap boolean
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }
  }

  public class BookmarkLineBool : EventArgs
  {
    /// <summary>
    /// Bookmark line
    /// </summary>
    public bool BookmarkLine
    {
      get;
      set;
    }
  }
}
