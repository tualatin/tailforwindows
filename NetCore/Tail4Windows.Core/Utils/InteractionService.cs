using System.Windows;
using Microsoft.Win32;

namespace Org.Vs.Tail4Win.Core.Utils
{
  /// <summary>
  /// Interaction service
  /// </summary>
  public static class InteractionService
  {
    /// <summary>
    /// Shows a information MessageBox
    /// </summary>
    /// <param name="message">Message to show</param>
    public static void ShowInformationMessageBox(string message)
    {
      if ( string.IsNullOrWhiteSpace(message) )
        return;

      MessageBox.Show(message, CoreEnvironment.ApplicationTitle, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// Shows a error MessageBox
    /// </summary>
    /// <param name="errorMessage">Error to show</param>
    public static void ShowErrorMessageBox(string errorMessage)
    {
      if ( string.IsNullOrWhiteSpace(errorMessage) )
        return;

      string caption = $"{CoreEnvironment.ApplicationTitle} - {Application.Current.TryFindResource("Error")}";
      MessageBox.Show(errorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Show a question MessageBox
    /// </summary>
    /// <param name="question">Question to show</param>
    /// <param name="defaultMessageBoxResult">default MessageBoxResult is <see cref="MessageBoxResult.Yes"/></param>
    /// <returns>MessageBoxResult</returns>
    public static MessageBoxResult ShowQuestionMessageBox(string question, MessageBoxResult defaultMessageBoxResult = MessageBoxResult.Yes)
    {
      string caption = $"{CoreEnvironment.ApplicationTitle} - {Application.Current.TryFindResource("Question")}";
      return string.IsNullOrWhiteSpace(question) ? MessageBoxResult.None :
        MessageBox.Show(question, caption, MessageBoxButton.YesNo, MessageBoxImage.Question, defaultMessageBoxResult);
    }

    /// <summary>
    /// Shows open file dialog
    /// </summary>
    /// <param name="fileName">Output of filename</param>
    /// <param name="filter">Filter</param>
    /// <param name="title">Title</param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public static bool OpenFileDialog(out string fileName, string filter, string title)
    {
      var openDialog = new OpenFileDialog
      {
        Filter = filter,
        RestoreDirectory = true,
        Title = title
      };

      var result = openDialog.ShowDialog();
      fileName = string.Empty;

      if ( result != true )
        return false;

      fileName = openDialog.FileName;

      return true;
    }

    /// <summary>
    /// Shows save file dialog
    /// </summary>
    /// <param name="fileName">Filename</param>
    /// <param name="filter">Filter</param>
    /// <param name="title">Title</param>
    /// <returns>If success <c>True</c> otherwise <c>False</c></returns>
    public static bool OpenSaveDialog(ref string fileName, string filter, string title)
    {
      var saveDialog = new SaveFileDialog
      {
        FileName = fileName,
        Filter = filter,
        Title = title
      };

      var result = saveDialog.ShowDialog();

      if ( result != true )
        return false;

      fileName = saveDialog.FileName;

      return true;
    }
  }
}
