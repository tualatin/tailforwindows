using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.BookmarkCommentModule.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.BookmarkCommentModule.ViewModels
{
  /// <summary>
  /// AddBookmarkComment view model
  /// </summary>
  public class AddBookmarkCommentViewModel : NotifyMaster, IAddBookmarkCommentViewModel
  {
    #region Properties

    private string _comment;

    /// <summary>
    /// Comment
    /// </summary>
    public string Comment
    {
      get => _comment;
      set
      {
        if ( Equals(value, _comment) )
          return;

        _comment = value;
        OnPropertyChanged();
      }
    }

    private bool _textBoxHasFocus;

    /// <summary>
    /// TextBox has focus
    /// </summary>
    public bool TextBoxHasFocus
    {
      get => _textBoxHasFocus;
      set
      {
        _textBoxHasFocus = value;
        OnPropertyChanged();
      }
    }

    #endregion

    #region Commands

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    private ICommand _saveBookmarkCommentCommand;

    /// <summary>
    /// Save bookmark comment command
    /// </summary>
    public ICommand SaveBookmarkCommentCommand => _saveBookmarkCommentCommand ?? (_saveBookmarkCommentCommand = new RelayCommand(p => ExecuteSaveBookmarkCommentCommand((Window) p)));

    #endregion

    #region Command functions

    private void ExecuteSaveBookmarkCommentCommand(Window window) => window.Close();

    private void ExecuteLoadedCommand() => TextBoxHasFocus = true;

    #endregion
  }
}
