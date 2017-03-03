using System.IO;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.PatternUtil.Events;


namespace Org.Vs.TailForWin.PatternUtil.UI
{
  /// <summary>
  /// Interaction logic for DefineParts.xaml
  /// </summary>
  public partial class DefineParts : Window
  {
    /// <summary>
    /// Fires, when pattern changed successful
    /// </summary>
    public event PatternObjectChangedEventHandler PatternObjectChanged;

    private string fileExtension;
    private string patternFile;
    private bool isRegex;

    /// <summary>
    /// Tail log file, which is used with patterns
    /// </summary>
    public string TailLogFile
    {
      get
      {
        return (TextBlockOriginal.Text);
      }
      set
      {
        TextBlockOriginal.Text = Path.GetFileName(value);
        fileExtension = Path.GetExtension(value);
      }
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DefineParts()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
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
      if(sender is DefinePattern)
      {
        if(pattern == null)
          return;

        if(pattern.IsRegex)
          isRegex = pattern.IsRegex;

        patternFile = $"{patternFile}{pattern.PatternString}";
        ShowResult();
      }
    }

    private void PartChanged(object sender, Part part)
    {
      string result = TextBlockOriginal.Text;

      if(sender is SelectPart)
      {
        if(part == null)
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
      if(e.Key == Key.Escape)
        Close();
    }

    #endregion
  }
}
