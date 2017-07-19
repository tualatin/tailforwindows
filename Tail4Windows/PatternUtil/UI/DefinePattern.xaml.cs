using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.PatternUtil.Events;


namespace Org.Vs.TailForWin.PatternUtil.UI
{
  /// <summary>
  /// Interaction logic for DefinePattern.xaml
  /// </summary>
  public partial class DefinePattern
  {
    /// <summary>
    /// Fires, when pattern changed
    /// </summary>
    public event PatternChangedEventHandler PatternChanged;

    private Pattern pattern;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DefinePattern()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
    }

    /// <summary>
    /// Add a list of default patterns to combobox
    /// </summary>
    /// <param name="defaultPatterns">List of default patterns</param>
    public void AddDefaultPatterns(List<Pattern> defaultPatterns)
    {
      if ( defaultPatterns == null || defaultPatterns.Count == 0 )
        return;

      foreach ( var item in defaultPatterns )
      {
        ComboBoxPattern.Items.Add(item);
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      pattern = new Pattern();
      FocusManager.SetFocusedElement(this, ComboBoxPattern);
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
      if ( string.IsNullOrEmpty(ComboBoxPattern.Text) )
      {
        pattern = null;
      }
      else
      {
        if ( CheckBoxRegex.IsChecked != null )
          pattern.IsRegex = CheckBoxRegex.IsChecked.Value;

        pattern.PatternString = ComboBoxPattern.Text;
      }

      PatternChanged?.Invoke(this, pattern);
      Close();
    }

    private void ComboBoxPattern_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if ( !IsInitialized )
        return;

      if ( e.AddedItems.Count > 0 )
      {
        var pt = e.AddedItems[0];

        if ( pt is Pattern pt1 )
          CheckBoxRegex.IsChecked = pt1.IsRegex;
      }
    }

    #region HelperFunctsion

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        Close();
    }

    #endregion
  }
}
