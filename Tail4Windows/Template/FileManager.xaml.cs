using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Data.Events;
using Org.Vs.TailForWin.PatternUtil.UI;
using Org.Vs.TailForWin.PatternUtil.Utils;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for FileManger.xaml
  /// </summary>
  public sealed partial class FileManager : INotifyPropertyChanged
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileManager));

    /// <summary>
    /// FileManager DoUpdate event handler
    /// </summary>
    public event EventHandler DoUpdate;

    /// <summary>
    /// FileManager open file in new tab event handler
    /// </summary>
    public event EventHandler OpenFileAsNewTab;

    private EFileManagerState fmState;
    private FileManagerStructure fmDoc;
    private FileManagerDataList fmData;

    /// <summary>
    /// Default settings or settings of added file
    /// </summary>
    private readonly FileManagerData fmProperties;

    /// <summary>
    /// This is the member to working with
    /// </summary>
    private FileManagerData fmWorkingProperties;

    /// <summary>
    /// This is the reference of fmWorkingProperties
    /// </summary>
    private FileManagerData.MementoFileManagerData fmMemento;

    private int errors;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fmState">EFileManagerState</param>
    /// <param name="addFile">TailForLogData object</param>
    public FileManager(EFileManagerState fmState, TailLogData addFile)
    {
      this.fmState = fmState;
      fmProperties = new FileManagerData
      {
        FontType = addFile.FontType,
        KillSpace = addFile.KillSpace,
        ListOfFilter = addFile.ListOfFilter,
        RefreshRate = addFile.RefreshRate,
        ThreadPriority = addFile.ThreadPriority,
        Wrap = addFile.Wrap,
        Timestamp = addFile.Timestamp,
        FileEncoding = addFile.FileEncoding,
        OpenFromFileManager = addFile.OpenFromFileManager,
        PatternString = addFile.PatternString,
        IsRegex = addFile.IsRegex
      };

      fmWorkingProperties = fmProperties.Clone();

      InitializeComponent();
      InitFileManager();

      if(fmState == EFileManagerState.AddFile)
      {
        SetAddSaveButton(false);

        fmWorkingProperties.FileName = addFile.FileName;
        fmWorkingProperties.OriginalFileName = addFile.FileName;
        fmWorkingProperties.ID = ++fmDoc.LastFileId;
        fmWorkingProperties.FileEncoding = addFile.FileEncoding;

        fmData.Add(fmWorkingProperties);
      }

      comboBoxCategory.SelectedIndex = 0;
    }

    #region ClickEvents

    private void dataGridFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if(e.ChangedButton != MouseButton.Left)
        return;

      OpenFileToTail();
    }

    private void btnFont_Click(object sender, RoutedEventArgs e)
    {
      System.Drawing.Font textFont = new System.Drawing.Font(fmProperties.FontType.FontFamily, fmProperties.FontType.Size, fmProperties.FontType.Style);
      System.Windows.Forms.FontDialog fontManager = new System.Windows.Forms.FontDialog
      {
        ShowEffects = false,
        Font = textFont,
        FontMustExist = true
      };

      if(fontManager.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
        return;

      fmWorkingProperties.FontType = fontManager.Font;
      ChangeFmStateToEditItem();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      OpenFileToTail();
    }

    private void btnNew_Click(object sender, RoutedEventArgs e)
    {
      string fName;

      if(LogFile.OpenFileLogDialog(out fName, "Logfiles (*.log)|*.log|Textfiles (*.txt)|*.txt|All files (*.*)|*.*", Application.Current.FindResource("OpenFileDialog") as string))
        AddNewFile(fName);
    }

    private void btnCancelAdd_Click(object sender, RoutedEventArgs e)
    {
      switch(fmState)
      {
      case EFileManagerState.AddFile:

        FileManagerData lastItem = fmData[fmData.Count - 1];
        fmData.Remove(lastItem);

        SelectLastItemInDataGrid();
        SetDialogTitle();
        break;

      case EFileManagerState.EditItem:

        if(fmMemento != null)
          fmWorkingProperties.RestoreFromMemento(fmMemento);

        SetSelectedComboBoxItem(fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
        break;

      default:

        if(fmMemento != null)
          fmWorkingProperties.RestoreFromMemento(fmMemento);
        break;
      }

      fmState = EFileManagerState.OpenFileManager;

      SetAddSaveButton();
      GetCurrentCollectionViewSource().Refresh();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if(MessageBox.Show(Application.Current.FindResource("QDeleteDataGridItem") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
        return;

      int index = dataGridFiles.SelectedIndex;
      fmWorkingProperties = dataGridFiles.SelectedItem as FileManagerData;

      if(fmDoc.RemoveNode(fmWorkingProperties))
      {
        fmData.Remove(fmWorkingProperties);
        dataGridFiles.Items.Refresh();

        fmDoc.RefreshCategories();
        RefreshCategoryComboBox();
        SortDataGrid();

        if(fmDoc.FmProperties.Count != 0)
          SelectLastItemInDataGrid();
      }

      fmState = EFileManagerState.OpenFileManager;
      CollectionViewSource.GetDefaultView(dataGridFiles.ItemsSource).Refresh();
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if(!checkBoxInsertCategory.IsChecked.Value)
        fmWorkingProperties.Category = comboBoxCategory.SelectedItem as string;

      // TODO better solution (IsEnable property) at the moment workaround
      if(fmWorkingProperties.EqualsProperties(fmMemento) && fmState != EFileManagerState.EditFilter)
        return;

      switch(fmState)
      {
      case EFileManagerState.AddFile:

        fmDoc.AddNewNode(fmWorkingProperties);
        break;

      case EFileManagerState.EditItem:
      case EFileManagerState.EditFilter:

        fmDoc.UpdateNode(fmWorkingProperties);
        break;
      }

      if(checkBoxInsertCategory.IsChecked.Value)
      {
        fmDoc.RefreshCategories();
        RefreshCategoryComboBox();
        checkBoxInsertCategory.IsChecked = false;
      }

      if(fmWorkingProperties.UsePattern)
        UsePatternToLogfile(fmWorkingProperties);
      else
        fmWorkingProperties.FileName = fmWorkingProperties.OriginalFileName;

      SortDataGrid();
      SetDialogTitle();

      SetSelectedComboBoxItem(fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      Title = "FileManager";
      SetAddSaveButton();
      fmState = EFileManagerState.OpenFileManager;
      fmMemento = fmWorkingProperties.SaveToMemento();
      GetCurrentCollectionViewSource().Refresh();
    }

    private void CheckBoxUsePattern_Click(object sender, RoutedEventArgs e)
    {
      if(!IsInitialized)
        return;

      ChangeFmStateToEditItem();
    }

    private void CheckBoxWrap_Click(object sender, RoutedEventArgs e)
    {
      if(!IsInitialized)
        return;

      ChangeFmStateToEditItem();
    }

    private void CheckBoxTimestamp_Click(object sender, RoutedEventArgs e)
    {
      if(!IsInitialized)
        return;

      ChangeFmStateToEditItem();
    }

    private void CheckBoxKillSpace_Click(object sender, RoutedEventArgs e)
    {
      if(!IsInitialized)
        return;

      ChangeFmStateToEditItem();
    }

    private void CheckBoxThreadNewWindow_Click(object sender, RoutedEventArgs e)
    {
      if(!IsInitialized)
        return;

      ChangeFmStateToEditItem();
    }

    private void CheckBoxAlwaysGroupByCategory_Click(object sender, RoutedEventArgs e)
    {
      if(!IsInitialized)
        return;

      GroupByCategory = CheckBoxAlwaysGroupByCategory.IsChecked.Value;
      ICollectionView cvFmData = GetCurrentCollectionViewSource();

      if(GroupByCategory)
        GridControlGroupByCategory(cvFmData);
      else
        cvFmData.GroupDescriptions.Clear();

      SettingsHelper.SaveSettings();
    }

    private void btnFilters_Click(object sender, RoutedEventArgs e)
    {
      Filters filters = new Filters(fmWorkingProperties)
      {
        Owner = LogFile.APP_MAIN_WINDOW
      };

      filters.SaveNow += SaveFilters;
      filters.ShowDialog();
    }

    private void CollapseGroups_Click(object sender, RoutedEventArgs e)
    {
      if(sender is MenuItem)
        IsExpanded = false;
    }

    private void ExpandGroups_Click(object sender, RoutedEventArgs e)
    {
      if(sender is MenuItem)
        IsExpanded = true;
    }

    private void GroupUngroupByCategory_Click(object sender, RoutedEventArgs e)
    {
      ICollectionView cvFmData = GetCurrentCollectionViewSource();

      if(GroupByCategory)
      {
        cvFmData.GroupDescriptions.Clear();
        GroupByCategory = false;
      }
      else
      {
        GridControlGroupByCategory(cvFmData);
        GroupByCategory = true;
      }
    }

    private void BtnAddPattern_Click(object sender, RoutedEventArgs e)
    {
      DefineParts defineParts = new DefineParts
      {
        Owner = this,
        TailLogFile = fmWorkingProperties.FileName
      };
      defineParts.PatternObjectChanged += PatternObjectChanged;
      defineParts.ShowDialog();
    }

    #endregion

    #region Events

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      ICollectionView cvFmData = GetCurrentCollectionViewSource();

      if(SettingsHelper.TailSettings.GroupByCategory)
      {
        GroupByCategory = true;
        GridControlGroupByCategory(cvFmData);
      }
      else
      {
        GroupByCategory = false;
        cvFmData.GroupDescriptions.Clear();
      }

      if(fmState == EFileManagerState.OpenFileManager && fmData.Count > 0)
      {
        dataGridFiles.SelectedItem = dataGridFiles.Items[0];
        dataGridFiles.ScrollIntoView(dataGridFiles.Items[0]);
      }
      else
      {
        SelectLastItemInDataGrid();
      }

      CheckBoxAlwaysGroupByCategory.DataContext = SettingsHelper.TailSettings;

      if(fmData.Count == 0)
        return;

      IsExpanded = true;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      fmData.CollectionChanged -= FmData_CollectionChanged;

      if(dataGridFiles.IsEnabled)
        return;

      MessageBox.Show(Application.Current.FindResource("FileManagerCloseUnsaveItem") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
      e.Cancel = true;
    }

    private void comboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      e.Handled = true;

      if(comboBoxCategory.SelectedItem == null || fmWorkingProperties == null)
        return;

      fmWorkingProperties.Category = comboBoxCategory.SelectedItem as string;
      ChangeFmStateToEditItem();
    }

    private void comboBoxRefreshRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      e.Handled = true;

      fmWorkingProperties.RefreshRate = (ETailRefreshRate) comboBoxRefreshRate.SelectedItem;
      ChangeFmStateToEditItem();
    }

    private void comboBoxThreadPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;
      e.Handled = true;

      fmWorkingProperties.ThreadPriority = (ThreadPriority) comboBoxThreadPriority.SelectedItem;
      ChangeFmStateToEditItem();
    }

    private void comboBoxFileEncode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      e.Handled = true;

      fmWorkingProperties.FileEncoding = (Encoding) comboBoxFileEncode.SelectedItem;
      ChangeFmStateToEditItem();
    }

    private void dataGridFiles_Loaded(object sender, RoutedEventArgs e)
    {
      ICollectionView cvFmData = GetCurrentCollectionViewSource();

      if(fmData.Count == 0)
        return;

      SortDataGrid();

      if(dataGridFiles.SelectedCells.Count == 0)
        return;

      var dc = GetDataGridCell(dataGridFiles.SelectedCells[0]);
      Keyboard.Focus(dc);
    }

    private void dataGridFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      e.Handled = true;

      fmWorkingProperties = dataGridFiles.SelectedItem as FileManagerData;

      if(fmWorkingProperties != null)
      {
        fmMemento = fmWorkingProperties.SaveToMemento();
        SetSelectedComboBoxItem(fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      }
      else
      {
        fmMemento = null;
      }

      FMProperties.DataContext = fmWorkingProperties;
    }

    private void textBlockDescription_TextChanged(object sender, TextChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      if(!string.IsNullOrEmpty(textBlockDescription.Text))
        ChangeFmStateToEditItem();
    }

    private void textBoxNewCategorie_TextChanged(object sender, TextChangedEventArgs e)
    {
      if(!IsInitialized)
        return;

      ChangeFmStateToEditItem();
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      e.Handled = true;

      try
      {
        var text = e.Data.GetData(DataFormats.FileDrop);

        if(text == null)
          return;

        const string fileName = "{((string[])text)[0]}";
        AddNewFile(fileName);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
      e.Handled = true;

      if(e.Source == sender)
        e.Effects = DragDropEffects.None;
    }

    private void SaveFilters(object sender, EventArgs e)
    {
      dataGridFiles.Items.Refresh();
      FMProperties.DataContext = fmWorkingProperties;

      if(fmState != EFileManagerState.AddFile)
        fmState = EFileManagerState.EditFilter;

      SetAddSaveButton(false);
    }

    private void OnDoUpdate()
    {
      EventHandler handler = DoUpdate;
      handler?.Invoke(this, EventArgs.Empty);
    }

    private void CollectionViewSource_Filter(object sender, System.Windows.Data.FilterEventArgs e)
    {
      FileManagerData fmData = e.Item as FileManagerData;

      if(fmData == null)
        return;
      if(string.IsNullOrEmpty(fmData.Category) || string.IsNullOrEmpty(fmData.Description))
        return;

      try
      {
        CultureInfo culturInfo = CultureInfo.CurrentCulture;
        int categoryResult = culturInfo.CompareInfo.IndexOf(fmData.Category, FilterTextBox.Text, CompareOptions.IgnoreCase);
        int descriptionResult = culturInfo.CompareInfo.IndexOf(fmData.Description, FilterTextBox.Text, CompareOptions.IgnoreCase);
        int result = categoryResult & descriptionResult;

        if(!string.IsNullOrEmpty(FilterTextBox.Text) && result < 0)
          e.Accepted = false;
        else
          e.Accepted = true;
      }
      catch(ArgumentNullException)
      {
        e.Accepted = true;
      }
    }

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      GetCurrentCollectionViewSource().Refresh();
    }

    private void Validation_Error(object sender, ValidationErrorEventArgs e)
    {
      if(e.Action == ValidationErrorEventAction.Added)
        errors++;
      else
        errors--;
    }

    private void PatternObjectChanged(object sender, string pattern, bool isRegex)
    {
      fmWorkingProperties.PatternString = pattern;
      fmWorkingProperties.IsRegex = isRegex;

      ChangeFmStateToEditItem();
    }

    #endregion

    #region HelperFunctions

    private ICollectionView GetCurrentCollectionViewSource()
    {
      return (CollectionViewSource.GetDefaultView(dataGridFiles.ItemsSource));
    }

    private void GridControlGroupByCategory(ICollectionView cvFmData)
    {
      if(cvFmData != null && cvFmData.CanGroup)
      {
        cvFmData.GroupDescriptions.Clear();
        cvFmData.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
      }
    }

    private void AddNewFile(string fileName)
    {
      if(string.IsNullOrEmpty(fileName))
        return;

      fmState = EFileManagerState.AddFile;

      fmWorkingProperties = new FileManagerData
      {
        Category = string.Empty,
        Description = string.Empty,
        FileName = fileName,
        OriginalFileName = fileName,
        ID = ++fmDoc.LastFileId,
        RefreshRate = fmProperties.RefreshRate,
        ThreadPriority = fmProperties.ThreadPriority,
        KillSpace = false,
        Timestamp = false,
        NewWindow = false,
        FontType = fmProperties.FontType,
        Wrap = false
      };

      fmWorkingProperties.ListOfFilter.Clear();

      fmData.Add(fmWorkingProperties);

      Title = $"FileManager - Add file '{fileName}'";
      dataGridFiles.Items.Refresh();
      SelectLastItemInDataGrid();

      comboBoxCategory.SelectedIndex = 0;
      SetAddSaveButton(false);
    }

    private static DataGridCell GetDataGridCell(DataGridCellInfo cellInfo)
    {
      var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

      return (DataGridCell) cellContent?.Parent;
    }

    private void SetDialogTitle()
    {
      if(string.Compare(Title, "FileManager", StringComparison.Ordinal) != 0)
        Title = "FileManager";
    }

    private void SortDataGrid()
    {
      switch(SettingsHelper.TailSettings.DefaultFileSort)
      {
      case EFileSort.FileCreationTime:

        ListCollectionView view = (ListCollectionView) GetCurrentCollectionViewSource();
        view.CustomSort = fmData;
        break;

      case EFileSort.Nothing:

        ICollectionView cvFmData = GetCurrentCollectionViewSource();

        if(cvFmData.CanSort)
          cvFmData.SortDescriptions.Add(new SortDescription("File", ListSortDirection.Ascending));

        break;
      }
    }

    private void ChangeFmStateToEditItem()
    {
      // TODO better solution
      if(!IsInitialized)
        return;

      if(fmState != EFileManagerState.OpenFileManager || fmMemento == null || fmWorkingProperties == null || fmWorkingProperties.EqualsProperties(fmMemento))
        return;

      fmState = EFileManagerState.EditItem;
      SetAddSaveButton(false);
    }

    private void RefreshCategoryComboBox()
    {
      if(fmDoc.Category.Count == 0)
        comboBoxCategory.IsEnabled = false;
      else
      {
        comboBoxCategory.Items.Refresh();
        comboBoxCategory.IsEnabled = true;
      }
    }

    private void SetAddSaveButton(bool state = true)
    {
      btnNew.IsEnabled = state;
      dataGridFiles.IsEnabled = state;
      FilterTextBox.IsEnabled = state;
    }

    private void SelectLastItemInDataGrid()
    {
      if(dataGridFiles.Items.Count <= 0)
        return;

      dataGridFiles.SelectedItem = fmData[fmData.Count - 1];
      dataGridFiles.ScrollIntoView(fmData[fmData.Count - 1]);
    }

    private void SetSelectedComboBoxItem(string category, ThreadPriority tp, ETailRefreshRate rr, Encoding fe)
    {
      if(category != null)
        comboBoxCategory.SelectedValue = category;

      comboBoxThreadPriority.SelectedValue = tp;
      comboBoxRefreshRate.SelectedValue = rr;
      comboBoxFileEncode.SelectedValue = fe;
    }

    private void InitFileManager()
    {
      PreviewKeyDown += HandleEsc;

      fmDoc = new FileManagerStructure();

      // Get a reference to FileManagerData collection
      fmData = (FileManagerDataList) Resources["fileManagerData"];

      if(LogFile.FmHelper != null && LogFile.FmHelper.Count > 0)
      {
        fmDoc.FmProperties.ForEach(item =>
         {
           FileManagerHelper f = LogFile.FmHelper.SingleOrDefault(x => x.ID == item.ID);

           if(f != null)
             item.OpenFromFileManager = f.OpenFromFileManager;
         });
      }

      foreach(FileManagerData item in fmDoc.FmProperties)
      {
        if(item.UsePattern)
          UsePatternToLogfile(item);

        fmData.Add(item);
      }

      fmData.CollectionChanged += FmData_CollectionChanged;
      dataGridFiles.DataContext = fmDoc.FmProperties;
      labelFileEncodingHint.Content = Application.Current.FindResource("FileEncodingLabel");

      SetAddSaveButton();

      comboBoxCategory.DataContext = fmDoc.Category;
      comboBoxRefreshRate.DataContext = LogFile.RefreshRate;
      comboBoxThreadPriority.DataContext = LogFile.ThreadPriority;
      comboBoxFileEncode.DataContext = LogFile.FileEncoding;
      comboBoxFileEncode.DisplayMemberPath = "HeaderName";

      RefreshCategoryComboBox();
    }

    private static void UsePatternToLogfile(FileManagerData item)
    {
      using(var patternController = new SearchPatternController())
      {
        item.FileName = patternController.GetCurrentFileByPattern(item);
      }
    }

    private void FmData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if(e.NewItems != null)
      {
        foreach(FileManagerData item in e.NewItems)
        {
          if(e.Action == NotifyCollectionChangedAction.Add)
            fmDoc.FmProperties.Add(item);
        }
      }

      if(e.OldItems != null)
      {
        foreach(FileManagerData item in e.OldItems)
        {
          if(e.Action == NotifyCollectionChangedAction.Remove)
            fmDoc.FmProperties.Remove(item);
        }
      }
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if(e.Key == Key.Escape)
        OnExit();
      if(e.Key == Key.Enter)
        btnOK_Click(sender, e);
    }

    private void OnExit()
    {
      Close();
    }

    private void OpenFileToTail()
    {
      if(fmWorkingProperties == null)
        return;

      if(fmWorkingProperties.NewWindow)
      {
        try
        {
          Process newWindow = new Process
          {
            StartInfo =
            {
              FileName = Process.GetCurrentProcess ().MainModule.FileName,
              Arguments = $"/id={fmWorkingProperties.ID}"
            }
          };
          newWindow.Start();
        }
        catch(Exception ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
      }
      else
      {
        fmDoc.FmProperties[dataGridFiles.SelectedIndex].OpenFromFileManager = true;
        FileManagerHelper helper = new FileManagerHelper
        {
          ID = fmDoc.FmProperties[dataGridFiles.SelectedIndex].ID,
          OpenFromFileManager = fmDoc.FmProperties[dataGridFiles.SelectedIndex].OpenFromFileManager
        };


        if(LogFile.FmHelper.Count > 0)
        {
          try
          {
            FileManagerHelper item = LogFile.FmHelper.SingleOrDefault(x => x.ID == helper.ID);

            if(item == null)
              LogFile.FmHelper.Add(helper);
          }
          catch(ArgumentNullException ex)
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
        }
        else
        {
          LogFile.FmHelper.Add(helper);
        }

        FileManagerDataEventArgs argument = new FileManagerDataEventArgs(fmWorkingProperties);
        OpenFileAsNewTab?.Invoke(this, argument);

        argument.Dispose();
      }

      Close();
    }

    #endregion

    #region GridControl groups expanded/collapse

    /// <summary>
    /// PropertyChangedEventHandler
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool isExpanded;

    /// <summary>
    /// Groups are expanded
    /// </summary>
    public bool IsExpanded
    {
      get
      {
        return (isExpanded);
      }
      set
      {
        isExpanded = value;
        NotifyPropertyChanged("IsExpanded");
      }
    }

    private bool groupByCategory;

    /// <summary>
    /// GroupByCategory
    /// </summary>
    public bool GroupByCategory
    {
      get
      {
        return (groupByCategory);
      }
      set
      {
        groupByCategory = value;
        NotifyPropertyChanged("GroupByCategory");
      }
    }

    #endregion
  }
}
