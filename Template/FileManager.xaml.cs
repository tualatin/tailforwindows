using System.Windows;
using TailForWin.Data;
using TailForWin.Controller;
using System.Windows.Input;
using System.Diagnostics;
using System.Text;
using System;
using TailForWin.Utils;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for FileManger.xaml
  /// </summary>
  public partial class FileManager: Window
  {
    /// <summary>
    /// FileManager DoUpdate event handler
    /// </summary>
    public event EventHandler DoUpdate;

    /// <summary>
    /// FileManager open file in new tab event handler
    /// </summary>
    public event EventHandler OpenFileAsNewTab;

    private SettingsData.EFileManagerState fmState;
    private FileManagerStructure fmDoc;
    private bool isInit = false;

    /// <summary>
    /// Default settings or settings of added file
    /// </summary>
    private FileManagerData fmProperties;

    /// <summary>
    /// This is the member to working with
    /// </summary>
    private FileManagerData fmWorkingProperties;

    /// <summary>
    /// This is the reference of fmWorkingProperties
    /// </summary>
    private FileManagerData.MementoFileManagerData fmMemento;


    public FileManager (SettingsData.EFileManagerState fmState, TailLogData addFile)
    {
      this.fmState = fmState;
      fmProperties = new FileManagerData ( )
      {
        FontType = addFile.FontType,
        KillSpace = addFile.KillSpace,
        ListOfFilter = addFile.ListOfFilter,
        RefreshRate = addFile.RefreshRate,
        ThreadPriority = addFile.ThreadPriority,
        Wrap = addFile.Wrap,
        Timestamp = addFile.Timestamp,
        FileEncoding = addFile.FileEncoding, 
      };

      fmWorkingProperties = fmProperties.Clone ( );

      InitializeComponent ( );
      InitFileManager ( );

      if (fmState == SettingsData.EFileManagerState.AddFile)
      {
        SetAddSaveButton (false);

        fmWorkingProperties.FileName = addFile.FileName;
        fmWorkingProperties.ID = ++fmDoc.LastFileId;
        fmWorkingProperties.FileEncoding = addFile.FileEncoding;

        fmDoc.FMProperties.Add (fmWorkingProperties);
        dataGridFiles.Items.Refresh ( );
      }

      isInit = true;
    }

    #region ClickEvents

    private void btnFont_Click (object sender, RoutedEventArgs e)
    {
      System.Drawing.Font textFont = fmWorkingProperties.FontType;
      System.Windows.Forms.FontDialog fontManager = new System.Windows.Forms.FontDialog ( ) { ShowEffects = false, Font = textFont, FontMustExist = true };

      if (fontManager.ShowDialog ( ) != System.Windows.Forms.DialogResult.Cancel)
      {
        fmWorkingProperties.FontType = fontManager.Font;
        ChangeFMStateToEditItem ( );

      }
    }

    private void btnCancel_Click (object sender, RoutedEventArgs e)
    {
      Close ( );
    }

    private void btnOK_Click (object sender, RoutedEventArgs e)
    {
      if (fmWorkingProperties.NewWindow == true)
      {
        Process newWindow = new Process ( );
        newWindow.StartInfo.FileName = Process.GetCurrentProcess ( ).MainModule.FileName;
        newWindow.StartInfo.Arguments = string.Format ("/id={0}", fmWorkingProperties.ID);
        newWindow.Start ( );
      }
      else
      {
        FileManagerDataEventArgs argument = new FileManagerDataEventArgs (fmWorkingProperties);

        if (OpenFileAsNewTab != null)
          OpenFileAsNewTab (this, argument);

        argument.Dispose ( );
      }

      Close ( );
    }

    private void btnNew_Click (object sender, RoutedEventArgs e)
    {
      string fName = string.Empty;

      if (LogFile.OpenFileLogDialog (out fName, "Logfiles (*.log)|*.log|Textfiles (*.txt)|*.txt|All files (*.*)|*.*", Application.Current.FindResource ("OpenFileDialog") as string))
        AddNewFile (fName);
    }

    private void btnCancelAdd_Click (object sender, RoutedEventArgs e)
    {
      switch (fmState)
      {
      case SettingsData.EFileManagerState.AddFile:

        FileManagerData lastItem = fmDoc.FMProperties[fmDoc.FMProperties.Count - 1];
        fmDoc.FMProperties.Remove (lastItem);
        dataGridFiles.Items.Refresh ( );
        SelectLastItemInDataGrid ( );
        SetDialogTitle ( );
        break;

      case SettingsData.EFileManagerState.EditItem:

        if (fmMemento != null)
          fmWorkingProperties.RestoreFromMemento (fmMemento);

        SetSelectedComboBoxItem (fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
        break;

      default:

        if (fmMemento != null)
          fmWorkingProperties.RestoreFromMemento (fmMemento);
        break;
      }

      fmState = SettingsData.EFileManagerState.OpenFileManager;
      SetAddSaveButton ( );
    }

    private void btnDelete_Click (object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show (Application.Current.FindResource ("QDeleteDataGridItem").ToString ( ), LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
      {
        int index = dataGridFiles.SelectedIndex;
        fmWorkingProperties = dataGridFiles.SelectedItem as FileManagerData;

        if (fmDoc.RemoveNode (fmWorkingProperties))
        {
          fmDoc.FMProperties.RemoveAt (index);
          dataGridFiles.Items.Refresh ( );

          fmDoc.RefreshCategories ( );
          RefreshCategoryComboBox ( );

          SortDataGrid ( );

          if (fmDoc.FMProperties.Count != 0)
            SelectLastItemInDataGrid ( );
        }

        fmState = SettingsData.EFileManagerState.OpenFileManager;
      }
    }

    private void btnSave_Click (object sender, RoutedEventArgs e)
    {
      if (checkBoxInsertCategory.IsChecked == false)
        fmWorkingProperties.Category = comboBoxCategory.SelectedItem as string;

      // TODO better solution (IsEnable property) at the moment workaground
      if (fmWorkingProperties.EqualsProperties (fmMemento) && fmState != SettingsData.EFileManagerState.EditFilter)
        return;

      switch (fmState)
      {
      case SettingsData.EFileManagerState.AddFile:

        fmDoc.AddNewNode (fmWorkingProperties);
        break;

      case SettingsData.EFileManagerState.EditItem:
      case SettingsData.EFileManagerState.EditFilter:

        fmDoc.UpdateNode (fmWorkingProperties);
        break;

      default:

        break;
      }

      if (checkBoxInsertCategory.IsChecked == true)
      {
        fmDoc.RefreshCategories ( );
        RefreshCategoryComboBox ( );
        checkBoxInsertCategory.IsChecked = false;
      }

      SortDataGrid ( );
      SetDialogTitle ( );

      SetSelectedComboBoxItem (fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      SetAddSaveButton ( );
      fmState = SettingsData.EFileManagerState.OpenFileManager;
      fmMemento = fmWorkingProperties.SaveToMemento ( );
    }

    private void checkBoxWrap_Click (object sender, RoutedEventArgs e)
    {
      ChangeFMStateToEditItem ( );
    }

    private void checkBoxTimestamp_Click (object sender, RoutedEventArgs e)
    {
      ChangeFMStateToEditItem ( );
    }

    private void checkBoxKillSpace_Click (object sender, RoutedEventArgs e)
    {
      ChangeFMStateToEditItem ( );
    }

    private void checkBoxThreadNewWindow_Click (object sender, RoutedEventArgs e)
    {
      ChangeFMStateToEditItem ( );
    }

    private void btnFilters_Click (object sender, RoutedEventArgs e)
    {
      Filters filters = new Filters (fmWorkingProperties)
      {
        Owner = LogFile.APP_MAIN_WINDOW
      };

      filters.SaveNow += SaveFilters;
      filters.ShowDialog ( );
    }

    #endregion

    #region Events

    private void Window_Loaded (object sender, RoutedEventArgs e)
    {
      if (fmState == SettingsData.EFileManagerState.OpenFileManager)
      {
        dataGridFiles.SelectedItem = dataGridFiles.Items[0];
        dataGridFiles.ScrollIntoView (dataGridFiles.Items[0]);
      }
      else
        SelectLastItemInDataGrid ( );
    }

    private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (dataGridFiles.IsEnabled == false)
      {
        MessageBox.Show (Application.Current.FindResource ("FileManagerCloseUnsaveItem").ToString ( ), LogFile.APPLICATION_CAPTION, MessageBoxButton.OK, MessageBoxImage.Information);
        e.Cancel = true;
      }
    }

    private void comboBoxCategory_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      if (comboBoxCategory.SelectedItem != null && fmWorkingProperties != null)
      {
        fmWorkingProperties.Category = comboBoxCategory.SelectedItem as string;
        ChangeFMStateToEditItem ( );
      }
    }

    private void comboBoxRefreshRate_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      fmWorkingProperties.RefreshRate = (SettingsData.ETailRefreshRate) comboBoxRefreshRate.SelectedItem;
      ChangeFMStateToEditItem ( );
    }

    private void comboBoxThreadPriority_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      fmWorkingProperties.ThreadPriority = (System.Threading.ThreadPriority) comboBoxThreadPriority.SelectedItem;
      ChangeFMStateToEditItem ( );
    }

    private void comboBoxFileEncode_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      if (!isInit)
        return;

      fmWorkingProperties.FileEncoding = (Encoding) comboBoxFileEncode.SelectedItem;
      ChangeFMStateToEditItem ( );
    }

    private void dataGridFiles_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      fmWorkingProperties = dataGridFiles.SelectedItem as FileManagerData;

      if (fmWorkingProperties != null)
      {
        fmMemento = fmWorkingProperties.SaveToMemento ( );
        SetSelectedComboBoxItem (fmWorkingProperties.Category, fmWorkingProperties.ThreadPriority, fmWorkingProperties.RefreshRate, fmWorkingProperties.FileEncoding);
      }
      else
        fmMemento = null;

      FMProperties.DataContext = fmWorkingProperties;
    }
    
    private void textBlockDescription_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(textBlockDescription.Text))
        ChangeFMStateToEditItem ( );
    }

    private void textBoxNewCategorie_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      ChangeFMStateToEditItem ( );
    }

    private void Window_Drop (object sender, DragEventArgs e)
    {
      e.Handled = true;

      try
      {
        var text = e.Data.GetData (DataFormats.FileDrop);

        if (text != null)
        {
          string fileName = string.Format ("{0}", ((string[]) text)[0]);
          AddNewFile (fileName);
        }
      }
      catch (Exception ex)
      {
#if DEBUG
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManager", string.Format ("Drop exception: {0}", ex));
#endif
      }
    }

    private void Window_DragEnter (object sender, DragEventArgs e)
    {
      e.Handled = true;

      if (e.Source == sender)
        e.Effects = DragDropEffects.None;
    }

    private void SaveFilters (object sender, EventArgs e)
    {
      dataGridFiles.Items.Refresh ( );
      FMProperties.DataContext = fmWorkingProperties;

      if (fmState != SettingsData.EFileManagerState.AddFile)
        fmState = SettingsData.EFileManagerState.EditFilter;

      SetAddSaveButton (false);
    }

    #endregion

    #region HelperFunctions

    private void AddNewFile (string fileName)
    {
      if (!string.IsNullOrEmpty (fileName))
      {
        fmState = SettingsData.EFileManagerState.AddFile;

        fmWorkingProperties = new FileManagerData ( )
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

        fmWorkingProperties.ListOfFilter.Clear ( );

        fmDoc.FMProperties.Add (fmWorkingProperties);
        // dataGridFiles.Items.Refresh ( );
        SelectLastItemInDataGrid ( );

        comboBoxCategory.SelectedIndex = 0;
        SetAddSaveButton (false);
      }
    }

    private void SetDialogTitle ()
    {
      if (Title.CompareTo ("FileManager") != 0)
        Title = "FileManager";
    }

    private void SortDataGrid ()
    {
      fmDoc.SortListIfRequired ( );
      dataGridFiles.Items.Refresh ( );
    }

    private void ChangeFMStateToEditItem ()
    {
      // TODO better solution
      if (!isInit)
        return;

      if (fmState == SettingsData.EFileManagerState.OpenFileManager && fmMemento != null && fmWorkingProperties != null && !fmWorkingProperties.EqualsProperties (fmMemento))
      {
        fmState = SettingsData.EFileManagerState.EditItem;
        SetAddSaveButton (false);
      }
    }

    private void RefreshCategoryComboBox ()
    {
      if (fmDoc.Category.Count == 0)
        comboBoxCategory.IsEnabled = false;
      else
      {
        comboBoxCategory.Items.Refresh ( );
        comboBoxCategory.IsEnabled = true;
      }
    }

    private void SetAddSaveButton (bool state = true)
    {
      btnNew.IsEnabled = state;
      dataGridFiles.IsEnabled = state;
    }

    private void SelectLastItemInDataGrid ()
    {
      if (dataGridFiles.Items.Count > 0)
      {
        dataGridFiles.SelectedItem = fmDoc.FMProperties[fmDoc.FMProperties.Count - 1];
        dataGridFiles.ScrollIntoView (fmDoc.FMProperties[fmDoc.FMProperties.Count - 1]);
      }
    }

    private void SetSelectedComboBoxItem (string category, System.Threading.ThreadPriority tp, SettingsData.ETailRefreshRate rr, Encoding fe)
    {
      if (category != null)
        comboBoxCategory.SelectedValue = category;
       
      comboBoxThreadPriority.SelectedValue = tp;
      comboBoxRefreshRate.SelectedValue = rr;
      comboBoxFileEncode.SelectedValue = fe;
    }

    private void InitFileManager ()
    {
      PreviewKeyDown += HandleEsc;

      fmDoc = new FileManagerStructure ( );
      dataGridFiles.DataContext = fmDoc.FMProperties;
      labelFileEncodingHint.Content = Application.Current.FindResource ("FileEncodingLabel");

      SetAddSaveButton ( );

      comboBoxCategory.DataContext = fmDoc.Category;
      comboBoxRefreshRate.DataContext = LogFile.RefreshRate;
      comboBoxThreadPriority.DataContext = LogFile.ThreadPriority;
      comboBoxFileEncode.DataContext = LogFile.FileEncoding;
      comboBoxFileEncode.DisplayMemberPath = "HeaderName";

      RefreshCategoryComboBox ( );
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit ( );
    }

    private void OnExit ()
    {
      Close ( );
    }

    #endregion
  }
}
