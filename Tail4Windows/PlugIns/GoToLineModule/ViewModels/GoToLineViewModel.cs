using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.GoToLineModule.ViewModels
{
  /// <summary>
  /// GoToLine view model
  /// </summary>
  public class GoToLineViewModel : NotifyMaster
  {
    #region Properties

    /// <summary>
    /// Parent window id
    /// </summary>
    public Guid ParentWindowId
    {
      private get;
      set;
    }

    /// <summary>
    /// Max linenumber
    /// </summary>
    public int MaxLines
    {
      private get;
      set;
    }

    /// <summary>
    /// Min linenumber
    /// </summary>
    public int MinLines
    {
      private get;
      set;
    }

    private string _linesResult;

    /// <summary>
    /// Lines result
    /// </summary>
    public string LinesResult
    {
      get => _linesResult;
      set
      {
        if ( Equals(value, _linesResult) )
          return;

        _linesResult = value;
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

    private string _goToLine;

    /// <summary>
    /// Go to line
    /// </summary>
    public string GoToLine
    {
      get => _goToLine;
      set
      {
        _goToLine = value;
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

    private ICommand _goToLineCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand GoToLineCommand => _goToLineCommand ?? (_goToLineCommand = new RelayCommand(p => CanExecuteGoToLineCommand(), p => ExecuteGoToLineCommand((Window) p)));

    #endregion

    #region Command functions

    private bool CanExecuteGoToLineCommand()
    {
      if ( string.IsNullOrWhiteSpace(GoToLine) )
        return false;

      var regex = new Regex(@"^\d{1,9}$");

      if ( !regex.IsMatch(GoToLine.Trim()) )
        return false;

      int index = Convert.ToInt32(GoToLine.Trim());
      return index >= MinLines && index <= MaxLines;
    }

    private void ExecuteGoToLineCommand(Window window)
    {
      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new GoToLineMessage(Convert.ToInt32(GoToLine), ParentWindowId));
      window.Close();
    }

    private void ExecuteLoadedCommand()
    {
      var findResource = Application.Current.TryFindResource("GoToLineNumber");
      LinesResult = string.Format(findResource.ToString(), MinLines, MaxLines);

      TextBoxHasFocus = true;
    }

    #endregion
  }
}
