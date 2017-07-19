using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Interfaces;
using Org.Vs.TailForWin.PatternUtil.Events;


namespace Org.Vs.TailForWin.PatternUtil.UI
{
  /// <summary>
  /// Interaction logic for DefineParts.xaml
  /// </summary>
  public partial class DefineParts
  {
    /// <summary>
    /// Fires, when pattern changed successful
    /// </summary>
    public event PatternObjectChangedEventHandler PatternObjectChanged;

    private string fileExtension;
    private string patternFile;
    private bool isRegex;
    private IDefaultPatternStructureController defaultPatterns;
    private List<Pattern> definedPatterns;

    /// <summary>
    /// Tail log file, which is used with patterns
    /// </summary>
    public string TailLogFile
    {
      get => TextBlockOriginal.Text;
      set
      {
        TextBlockOriginal.Text = Path.GetFileName(value);
        fileExtension = Path.GetExtension(value);
      }
    }

    /// <summary>
    /// Shows the current result
    /// </summary>
    public string ShowResultString
    {
      get => TextBlockResult.Text;
      set => TextBlockResult.Text = value;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DefineParts()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      defaultPatterns = new DefaultPatternStructureController();
      definedPatterns = defaultPatterns.ReadDefaultPatternFile();
    }

    private void BtnSelectPart_Click(object sender, RoutedEventArgs e)
    {
      var selectPart = new SelectPart
      {
        Owner = this,
        Text = Path.GetFileNameWithoutExtension(TextBlockOriginal.Text)
      };
      selectPart.PartChanged += PartChanged;
      selectPart.ShowDialog();
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
      TextBlockResult.Text = patternFile = string.Empty;
      isRegex = false;
    }

    private void BtnAddPattern_Click(object sender, RoutedEventArgs e)
    {
      var definePattern = new DefinePattern
      {
        Owner = this
      };
      definePattern.PatternChanged += PatternChanged;
      definePattern.AddDefaultPatterns(definedPatterns);
      definePattern.ShowDialog();
    }

    private void BtnSavePattern_Click(object sender, RoutedEventArgs e)
    {
      PatternObjectChanged?.Invoke(this, TextBlockResult.Text, isRegex);
      Close();
    }

    #region HelperFunctions

    private void PatternChanged(object sender, Pattern pattern)
    {
      if ( sender is DefinePattern )
      {
        if ( pattern == null )
          return;

        if ( pattern.IsRegex )
          isRegex = pattern.IsRegex;

        patternFile = $"{patternFile}{pattern.PatternString}";
        ShowResult();
      }
    }

    private void PartChanged(object sender, Part part)
    {
      string result = TextBlockOriginal.Text;

      if ( sender is SelectPart )
      {
        if ( part == null )
          return;

        patternFile = $"{patternFile}{result.Substring(part.Begin, part.End)}";
        ShowResult();
      }
    }

    private void ShowResult()
    {
      TextBlockResult.Text = $"{patternFile}{fileExtension}";
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Escape )
        Close();
    }

    #endregion
  }
}
