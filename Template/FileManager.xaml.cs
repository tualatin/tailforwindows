using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TailForWin.Controller;
using TailForWin.Data;
using TailForWin.Data.Enums;
using TailForWin.Utils;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for FileManger.xaml
  /// </summary>
  public sealed partial class FileManager
  {
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
    private readonly bool isInit;

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
        OpenFromFileManager = addFile.OpenFromFileManager
      };

      fmWorkingProperties = fmProperties.Clone();

      InitializeComponent();
      InitFileManager();

      if (fmState == EFileManagerState.AddFile)
      {
        SetAddSaveButton(false);

        fmWorkingProperties.FileName = addFile.FileName;
        fmWorkingProperties.ID = ++fmDoc.LastFileId;
        fmWorkingProperties.FileEncoding = addFile.FileEncoding;

        fmDoc.FmProperties.Add(fmWorkingProperties);
        dataGridFiles.Items.Refresh();
      }

      comboBoxCategory.SelectedIndex = 0;
      isInit = true;
    }

    #region ClickEvents

    private void dataGridFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton != MouseButton.Left)
        return;

      OpenFileToTail();
    }

    private void btnFont_Click(object sender, RoutedEventArgs e)
    {
      System.Drawing.Font textFont = fmWorkingProperties.FontType;
      System.Windows.Forms.FontDialog fontManager = new System.Windows.Forms.FontDialog
      {
        ShowEffects = false,
        Font = textFont,
        FontMustExist = true
      };

      if (fontManager.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
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

      if (LogFile.OpenFileLogDialog(out fName, "Logfiles (*.log)|*.log|Textfiles (*.txt)|*.txt|All files (*.*)|*.*", Application.Current.FindResource("OpenFileDialog") as string))
        AddNewFile(fName);
    }

    private void btnCancelAdd_Click(object sender, RoutedEventArgs e)
    {
      switch (fmState)
      {
      case EFileManagerState.AddFile:

      FileManagerData lastItem = fmDoc.FmProperties[fmDoc.FmProperties.Count - 1];
      fmDoc.FmProperties.Remove(lastItem);
      dataGridFiles.Items.Refresh();
      SelectLastItemInDataGrid();
      SetDialogTitle();
      break;

      case EFileManagerState.EditItem:

      if (fmMemento != null)
        fmWorkingProperties.RestoreFromMemento(fmMemento);

      SetSelectedComboBoxItem(fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      break;

      default:

      if (fmMemento != null)
        fmWorkingProperties.RestoreFromMemento(fmMemento);
      break;
      }

      fmState = EFileManagerState.OpenFileManager;
      SetAddSaveButton();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show(Application.Current.FindResource("QDeleteDataGridItem") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
        return;

      int index = dataGridFiles.SelectedIndex;
      fmWorkingProperties = dataGridFiles.SelectedItem as FileManagerData;

      if (fmDoc.RemoveNode(fmWorkingProperties))
      {
        fmDoc.FmProperties.RemoveAt(index);
        dataGridFiles.Items.Refresh();

        fmDoc.RefreshCategories();
        RefreshCategoryComboBox();

        SortDataGrid();

        if (fmDoc.FmProperties.Count != 0)
          SelectLastItemInDataGrid();
      }

      fmState = EFileManagerState.OpenFileManager;
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (checkBoxInsertCategory.IsChecked == false)
        fmWorkingProperties.Category = comboBoxCategory.SelectedItem as string;

      // TODO better solution (IsEnable property) at the moment workaground
      if (fmWorkingProperties.EqualsProperties(fmMemento) && fmState != EFileManagerState.EditFilter)
        return;

      switch (fmState)
      {
      case EFileManagerState.AddFile:

      fmDoc.AddNewNode(fmWorkingProperties);
      break;

      case EFileManagerState.EditItem:
      case EFileManagerState.EditFilter:

      fmDoc.UpdateNode(fmWorkingProperties);
      break;
      }

      if (checkBoxInsertCategory.IsChecked == true)
      {
        fmDoc.RefreshCategories();
        RefreshCategoryComboBox();
        checkBoxInsertCategory.IsChecked = false;
      }

      SortDataGrid();
      SetDialogTitle();

      SetSelectedComboBoxItem(fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      Title = "FileManager";
      SetAddSaveButton();
      fmState = EFileManagerState.OpenFileManager;
      fmMemento = fmWorkingProperties.SaveToMemento();
    }

    private void checkBoxWrap_Click(object sender, RoutedEventArgs e)
    {
      ChangeFmStateToEditItem();
    }

    private void checkBoxTimestamp_Click(object sender, RoutedEventArgs e)
    {
      ChangeFmStateToEditItem();
    }

    private void checkBoxKillSpace_Click(object sender, RoutedEventArgs e)
    {
      ChangeFmStateToEditItem();
    }

    private void checkBoxThreadNewWindow_Click(object sender, RoutedEventArgs e)
    {
      ChangeFmStateToEditItem();
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

    #endregion

    #region Events

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (fmState == EFileManagerState.OpenFileManager)
      {
        dataGridFiles.SelectedItem = dataGridFiles.Items[0];
        dataGridFiles.ScrollIntoView(dataGridFiles.Items[0]);
      }
      else
        SelectLastItemInDataGrid();

      var dc = GetDataGridCell(dataGridFiles.SelectedCells[0]);
      Keyboard.Focus(dc);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (dataGridFiles.IsEnabled)
        return;

      MessageBox.Show(Application.Current.FindResource("FileManagerCloseUnsaveItem") as string, LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
      e.Cancel = true;
    }

    private void comboBoxCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      if (comboBoxCategory.SelectedItem == null || fmWorkingProperties == null)
        return;

      fmWorkingProperties.Category = comboBoxCategory.SelectedItem as string;
      ChangeFmStateToEditItem();
    }

    private void comboBoxRefreshRate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      fmWorkingProperties.RefreshRate = (ETailRefreshRate)comboBoxRefreshRate.SelectedItem;
      ChangeFmStateToEditItem();
    }

    private void comboBoxThreadPriority_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      fmWorkingProperties.ThreadPriority = (ThreadPriority)comboBoxThreadPriority.SelectedItem;
      ChangeFmStateToEditItem();
    }

    private void comboBoxFileEncode_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      fmWorkingProperties.FileEncoding = (Encoding)comboBoxFileEncode.SelectedItem;
      ChangeFmStateToEditItem();
    }

    private void dataGridFiles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      fmWorkingProperties = dataGridFiles.SelectedItem as FileManagerData;

      if (fmWorkingProperties != null)
      {
        fmMemento = fmWorkingProperties.SaveToMemento();
        SetSelectedComboBoxItem(fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      }
      else
        fmMemento = null;

      FMProperties.DataContext = fmWorkingProperties;
    }

    private void textBlockDescription_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(textBlockDescription.Text))
        ChangeFmStateToEditItem();
    }

    private void textBoxNewCategorie_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      ChangeFmStateToEditItem();
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      e.Handled = true;

      try
      {
        var text = e.Data.GetData(DataFormats.FileDrop);

        if (text == null)
          return;

        string fileName = $"{((string[])text)[0]}";
        AddNewFile(fileName);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
      e.Handled = true;

      if (e.Source == sender)
        e.Effects = DragDropEffects.None;
    }

    private void SaveFilters(object sender, EventArgs e)
    {
      dataGridFiles.Items.Refresh();
      FMProperties.DataContext = fmWorkingProperties;

      if (fmState != EFileManagerState.AddFile)
        fmState = EFileManagerState.EditFilter;

      SetAddSaveButton(false);
    }

    private void OnDoUpdate()
    {
      EventHandler handler = DoUpdate;
      handler?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region HelperFunctions

    private void AddNewFile(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return;

      fmState = EFileManagerState.AddFile;

      fmWorkingProperties = new FileManagerData
      {
        Category = string.Empty,
        Description = string.Empty,
        FileName = fileName,
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

      fmDoc.FmProperties.Add(fmWorkingProperties);
      Title = $"FileManager - Add file '{fileName}'";
      dataGridFiles.Items.Refresh();
      SelectLastItemInDataGrid();

      comboBoxCategory.SelectedIndex = 0;
      SetAddSaveButton(false);
    }

    private static System.Windows.Controls.DataGridCell GetDataGridCell(System.Windows.Controls.DataGridCellInfo cellInfo)
    {
      var cellContent = cellInfo.Column.GetCellContent(cellInfo.Item);

      return (System.Windows.Controls.DataGridCell)cellContent?.Parent;
    }

    private void SetDialogTitle()
    {
      if (string.Compare(Title, "FileManager", StringComparison.Ordinal) != 0)
        Title = "FileManager";
    }

    private void SortDataGrid()
    {
      fmDoc.SortListIfRequired();
      dataGridFiles.Items.Refresh();
    }

    private void ChangeFmStateToEditItem()
    {
      // TODO better solution
      if (!isInit)
        return;

      if (fmState != EFileManagerState.OpenFileManager || fmMemento == null || fmWorkingProperties == null || fmWorkingProperties.EqualsProperties(fmMemento))
        return;

      fmState = EFileManagerState.EditItem;
      SetAddSaveButton(false);
    }

    private void RefreshCategoryComboBox()
    {
      if (fmDoc.Category.Count == 0)
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
    }

    private void SelectLastItemInDataGrid()
    {
      if (dataGridFiles.Items.Count <= 0)
        return;

      dataGridFiles.SelectedItem = fmDoc.FmProperties[fmDoc.FmProperties.Count - 1];
      dataGridFiles.ScrollIntoView(fmDoc.FmProperties[fmDoc.FmProperties.Count - 1]);
    }

    private void SetSelectedComboBoxItem(string category, ThreadPriority tp, ETailRefreshRate rr, Encoding fe)
    {
      if (category != null)
        comboBoxCategory.SelectedValue = category;

      comboBoxThreadPriority.SelectedValue = tp;
      comboBoxRefreshRate.SelectedValue = rr;
      comboBoxFileEncode.SelectedValue = fe;
    }

    private void InitFileManager()
    {
      PreviewKeyDown += HandleEsc;

      fmDoc = new FileManagerStructure();

      if (LogFile.FmHelper.Count > 0)
      {
        fmDoc.FmProperties.ForEach(item =>
         {
           FileManagerHelper f = LogFile.FmHelper.SingleOrDefault(x => x.ID == item.ID);

           if (f != null)
             item.OpenFromFileManager = f.OpenFromFileManager;
         });
      }

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

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit();
      if (e.Key == Key.Enter)
        btnOK_Click(sender, e);
    }

    private void OnExit()
    {
      Close();
    }

    private void OpenFileToTail()
    {
      if (fmWorkingProperties == null)
        return;

      if (fmWorkingProperties.NewWindow)
      {
        try
        {
          Process newWindow = new Process
          {
            StartInfo =
                                {
                                  FileName = Process.GetCurrentProcess ( ).MainModule.FileName,
                                  Arguments = $"/id={fmWorkingProperties.ID}"
                                }
          };
          newWindow.Start();
        }
        catch (Exception ex)
        {
          ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, $"{System.Reflection.MethodBase.GetCurrentMethod().Name}, exception: {ex}");
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


        if (LogFile.FmHelper.Count > 0)
        {
          FileManagerHelper item = LogFile.FmHelper.SingleOrDefault(x => x.ID == helper.ID);

          if (item == null)
            LogFile.FmHelper.Add(helper);
        }
        else
          LogFile.FmHelper.Add(helper);

        FileManagerDataEventArgs argument = new FileManagerDataEventArgs(fmWorkingProperties);

        OpenFileAsNewTab?.Invoke(this, argument);

        argument.Dispose();
      }

      Close();
    }

    #endregion
  }
}
