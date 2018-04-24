using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data.Messages;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Controller;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Data;
using Org.Vs.TailForWin.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.Interfaces;
using Org.Vs.TailForWin.UI.Services;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule.ViewModels
{
  /// <summary>
  /// FilterManager view model
  /// </summary>
  public class FilterManagerViewModel : NotifyMaster
  {
    private CancellationTokenSource _cts;
    private readonly IXmlFileManager _xmlFileManagerController;

    #region Properties

    private TailData _currenTailData;

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    private TailData CurrentTailData
    {
      get => _currenTailData;
      set
      {
        _currenTailData = value;

        if ( _currenTailData == null )
          SaveButtonVisibility = Visibility.Collapsed;
        else
          SaveButtonVisibility = CurrentTailData.IsLoadedByXml ? Visibility.Visible : Visibility.Collapsed;

        OnPropertyChanged();
      }
    }

    /// <summary>
    /// FileManager view
    /// </summary>
    public ListCollectionView FilterManagerView
    {
      get;
      set;
    }

    private ObservableCollection<FilterData> FilterManagerCollection
    {
      get => CurrentTailData?.ListOfFilter;
      set
      {
        if ( value == CurrentTailData.ListOfFilter )
          return;

        CurrentTailData.ListOfFilter = value;
        OnPropertyChanged();
      }
    }

    private FilterData SelectedItem
    {
      get => FilterManagerView?.CurrentItem as FilterData;
      set
      {
        FilterManagerView?.MoveCurrentTo(value);
        OnPropertyChanged();
      }
    }

    private Visibility _saveButtonVisibility;

    /// <summary>
    /// SaveButtonVisibility
    /// </summary>
    public Visibility SaveButtonVisibility
    {
      get => _saveButtonVisibility;
      set
      {
        _saveButtonVisibility = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FilterManagerViewModel()
    {
      _xmlFileManagerController = new XmlFileManagerController();

      EnvironmentContainer.Instance.CurrentEventManager.RegisterHandler<OpenFilterDataFromTailDataMessage>(OnOpenTailData);
      SetCancellationTokenSource();
    }

    private void OnOpenTailData(OpenFilterDataFromTailDataMessage args)
    {
      if ( args == null )
        return;

      CurrentTailData = args.TailData;

      if ( CurrentTailData.ListOfFilter.Count == 0 )
        CurrentTailData.ListOfFilter.Add(new FilterData());

      foreach ( var item in CurrentTailData.ListOfFilter )
      {
        item.CommitChanges();
      }
    }

    #region Commands

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((Window) p)));

    private IAsyncCommand _saveCommand;

    /// <summary>
    /// Save command
    /// </summary>
    public IAsyncCommand SaveCommand => _saveCommand ?? (_saveCommand = AsyncCommand.Create(p => CanExecuteSaveCommand(), ExecuteSaveCommandAsync));

    private ICommand _addFilterDataCommand;

    /// <summary>
    /// Add <see cref="FilterData"/> command
    /// </summary>
    public ICommand AddFilterDataCommand => _addFilterDataCommand ?? (_addFilterDataCommand = new RelayCommand(p => ExecuteAddFilterDataCommand()));

    private IAsyncCommand _deleteFilterDataCommand;

    /// <summary>
    /// Delete <see cref="FilterData"/> from FileManager
    /// </summary>
    public IAsyncCommand DeleteFilterDataCommand => _deleteFilterDataCommand ?? (_deleteFilterDataCommand = AsyncCommand.Create(p => SelectedItem != null, ExecuteDeleteCommandAsync));

    private ICommand _undoCommand;

    /// <summary>
    /// Undo command
    /// </summary>
    public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(p => CanExecuteUndo(), p => ExecuteUndoCommand()));

    private ICommand _fontCommand;

    /// <summary>
    /// Font command
    /// </summary>
    public ICommand FontCommand => _fontCommand ?? (_fontCommand = new RelayCommand(p => SelectedItem != null, p => ExecuteFontCommand()));

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    #endregion

    #region Command functions

    private void ExecuteLoadedCommand()
    {
      FilterManagerView = (ListCollectionView) new CollectionViewSource { Source = FilterManagerCollection }.View;
      FilterManagerCollectionViewHolder.Cv = FilterManagerView;
      SelectedItem = FilterManagerCollection.First();

      OnPropertyChanged(nameof(FilterManagerCollection));
      OnPropertyChanged(nameof(FilterManagerView));
    }

    private void ExecuteFontCommand()
    {
      if ( SelectedItem == null )
        return;

      var filterFont = new System.Drawing.Font(SelectedItem.FilterFontType.FontFamily, SelectedItem.FilterFontType.Size, SelectedItem.FilterFontType.Style);
      var fontManager = new System.Windows.Forms.FontDialog
      {
        ShowEffects = true,
        Font = filterFont,
        FontMustExist = true,
        Color = EnvironmentContainer.ConvertStringToDrawingColor(SelectedItem.FilterColorHex, System.Drawing.Color.Crimson),
        ShowColor = true
      };

      if ( fontManager.ShowDialog() == System.Windows.Forms.DialogResult.Cancel )
        return;

      filterFont = new System.Drawing.Font(fontManager.Font.FontFamily, fontManager.Font.Size, fontManager.Font.Style);
      SelectedItem.FilterFontType = filterFont;
      SelectedItem.FilterColorHex = fontManager.Color.ToHexString();
    }

    private bool CanExecuteUndo() => SelectedItem != null && SelectedItem.CanUndo;

    private void ExecuteUndoCommand() => SelectedItem?.Undo();

    private void ExecuteAddFilterDataCommand()
    {
      var newItem = new FilterData();
      newItem.CommitChanges();

      FilterManagerCollection.Add(newItem);
      SelectedItem = FilterManagerCollection.Last();

      OnPropertyChanged(nameof(FilterManagerView));
    }

    private async Task ExecuteDeleteCommandAsync()
    {
      if ( SelectedItem == null )
        return;

      if ( !FilterManagerCollection.Contains(SelectedItem) )
        return;

      FilterManagerCollection.Remove(SelectedItem);

      if ( CurrentTailData.IsLoadedByXml )
        await ExecuteSaveCommandAsync().ConfigureAwait(false);

      OnPropertyChanged(nameof(FilterManagerView));
    }

    private bool CanExecuteSaveCommand()
    {
      if ( CurrentTailData == null )
        return false;

      var errors = GetFilterErrors();
      bool undo = CanExecuteUndo();

      return errors.Count <= 0 && undo && CurrentTailData.IsLoadedByXml && FilterManagerCollection != null && FilterManagerCollection.Count > 0;
    }

    private async Task ExecuteSaveCommandAsync()
    {
      MouseService.SetBusyState();
      SetCancellationTokenSource();

      await _xmlFileManagerController.ReadXmlFileAsync(_cts.Token).ConfigureAwait(false);
      await _xmlFileManagerController.UpdateTailDataInXmlFileAsync(_cts.Token, CurrentTailData).ConfigureAwait(false);

      foreach ( var item in FilterManagerCollection )
      {
        item.CommitChanges();
      }
    }

    private void ExecuteCloseCommand(Window window)
    {
      var errors = GetFilterErrors();

      if ( errors.Count > 0 )
      {
        foreach ( var filterData in errors )
        {
          FilterManagerCollection.Remove(filterData);
        }

        OnPropertyChanged(nameof(FilterManagerView));
      }

      var unsavedItems = FilterManagerCollection.Where(p => p.CanUndo).ToList();

      if ( SaveButtonVisibility == Visibility.Visible && unsavedItems.Count > 0 )
      {
        if ( EnvironmentContainer.ShowQuestionMessageBox(Application.Current.TryFindResource("FileManagerCloseUnsaveItem").ToString()) == MessageBoxResult.Yes )
        {
          ExecuteSaveCommandAsync().GetAwaiter().GetResult();
        }
        else
        {
          foreach ( var item in unsavedItems )
          {
            FilterManagerCollection.Remove(item);
          }
        }
      }

      _cts.Cancel();
      window?.Close();
    }

    private List<FilterData> GetFilterErrors()
    {
      var errors = FilterManagerCollection.Where(p => p["Description"] != null || p["Filter"] != null).ToList();
      return errors;
    }

    #endregion

    private void SetCancellationTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
