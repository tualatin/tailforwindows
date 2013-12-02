using System.Windows;
using TailForWin.Controller;
using System;
using TailForWin.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SearchDialog.xaml
  /// </summary>
  public partial class SearchDialog: Window
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

    #endregion

    private Dictionary<string, string> SearchWords;


    public SearchDialog ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      SearchWords = new Dictionary<string, string> ( );
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

    #endregion

    #region HelperFunctions

    private void AddSearchWordToDictionary ()
    {
      if (!SearchWords.ContainsKey (comboBoxWordToFind.Text))
      {
        SearchWords.Add (comboBoxWordToFind.Text, comboBoxWordToFind.Text);
        comboBoxWordToFind.Items.Clear ( );

        foreach (var item in SearchWords)
        {
          comboBoxWordToFind.Items.Add (item.Key);
        }
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
      AddSearchWordToDictionary ( );

      SearchData searching = new SearchData ( )
      {
        WordToFind = comboBoxWordToFind.Text,
        Count = count
      };

      if (FindNextEvent != null)
        FindNextEvent (this, searching);
    }

    private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
    {
      SettingsHelper.TailSettings.SearchWndXPos = Left;
      SettingsHelper.TailSettings.SearchWndYPos = Top;

      SettingsHelper.SaveSearchWindowPosition ( );

      e.Cancel = true;
      Hide ( );
    }

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      SetFocusToTextBox ( );
    }

    private void comboBoxWordToFind_TextChanged (object sender, TextChangedEventArgs e)
    {
      if (FindTextChanged != null)
        FindTextChanged (this, EventArgs.Empty);
    }

    #endregion
  }
}
