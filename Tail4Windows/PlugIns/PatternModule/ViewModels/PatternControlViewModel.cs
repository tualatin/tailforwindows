using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.Commands.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Interfaces;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;


namespace Org.Vs.TailForWin.PlugIns.PatternModule.ViewModels
{
  /// <summary>
  /// PatternControl view model
  /// </summary>
  public class PatternControlViewModel : StateManager, IPatternControlViewModel
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(PatternControlViewModel));

    private readonly IXmlPattern _patternController;
    private List<PatternData> _patterns;

    #region Properties

    private TailData _currenTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrenTailData
    {
      get => _currenTailData;
      set
      {
        if ( Equals(value, _currenTailData) )
          return;

        _currenTailData = value;
        OnPropertyChanged();
      }
    }

    private string _workingPattern;

    /// <summary>
    /// Current working pattern
    /// </summary>
    public string WorkingPattern
    {
      get => _workingPattern;
      set
      {
        if ( Equals(value, _workingPattern) )
          return;

        string currentValue = _workingPattern;
        ChangeState(new Command(() => _workingPattern = value, () => _workingPattern = currentValue, nameof(WorkingPattern), Notification));
      }
    }

    private bool _textBoxHasFocus;

    /// <summary>
    /// <see cref="System.Windows.Controls.TextBox"/> has focus
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

    private ObservableCollection<MenuItem> _menuItems;

    /// <summary>
    /// <see cref="ObservableCollection{T}"/> of <see cref="MenuItem"/>
    /// </summary>
    public ObservableCollection<MenuItem> MenuItems
    {
      get => _menuItems;
      set
      {
        if ( Equals(value, _menuItems) )
          return;

        _menuItems = value;
        OnPropertyChanged();
      }
    }

    private int _caretIndex;

    /// <summary>
    /// Caret index
    /// </summary>
    public int CaretIndex
    {
      get => _caretIndex;
      set
      {
        if ( value == _caretIndex )
          return;

        _caretIndex = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public PatternControlViewModel()
    {
      _patternController = new XmlPatternController();
      ((AsyncCommand<object>) LoadedCommand).PropertyChanged += OnLoadedPropertyChanged;
    }

    #region Commands

    private IAsyncCommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public IAsyncCommand LoadedCommand => _loadedCommand ?? (_loadedCommand = AsyncCommand.Create(ExecuteLoadedCommandAsync));

    /// <summary>
    /// Unloaded command
    /// </summary>
    public ICommand UnloadedCommand => throw new NotImplementedException();

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndoCommand(), p => ExecuteUndoCommand()));

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((Window) p)));

    #endregion

    #region Command functions

    private void ExecuteCloseCommand(Window window)
    {
      CurrenTailData.PatternString = WorkingPattern;

      window.Close();
    }

    private bool CanExecuteUndoCommand() => CurrenTailData != null && CurrenTailData.FindSettings.CanUndo || CanUndo;

    private void ExecuteUndoCommand()
    {
      Undo();
      CurrenTailData.FindSettings.Undo();
    }

    private async Task ExecuteLoadedCommandAsync() => _patterns = await _patternController.ReadDefaultPatternsAsync().ConfigureAwait(false);

    #endregion

    private void OnLoadedPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      WorkingPattern = CurrenTailData == null ? string.Empty : !string.IsNullOrWhiteSpace(CurrenTailData.PatternString) ? CurrenTailData.PatternString : CurrenTailData.File;
      TextBoxHasFocus = true;

      CommitChanges();
      CurrenTailData?.FindSettings.CommitChanges();

      if ( _patterns == null || _patterns.Count == 0 )
        return;

      _menuItems = new ObservableCollection<MenuItem>();

      try
      {
        foreach ( var pattern in _patterns.Where(p => p.IsRegex).OrderBy(p => p.PatternString).ToList() )
        {
          var menuItem = new MenuItem
          {
            Header = pattern.PatternString,
            Foreground = Brushes.RoyalBlue
          };

          _menuItems.Add(menuItem);
        }
      }
      catch ( Exception ex )
      {
        _menuItems = null;
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }

      OnPropertyChanged(nameof(MenuItems));
    }
  }
}
