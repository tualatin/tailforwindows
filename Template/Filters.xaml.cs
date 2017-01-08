using System;
using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Enums;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for Filters.xaml
  /// </summary>
  public partial class Filters
  {
    private FilterData filterData;
    private readonly TailLogData tailLogData;
    private FilterData.MementoFilterData mementoFilterData;
    private EFileManagerState fState;
    private readonly bool isInit;
    private int filterId;

    /// <summary>
    /// Save event handler
    /// </summary>
    public event EventHandler SaveNow;


    public Filters(TailLogData tailLogData)
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;
      this.tailLogData = tailLogData;
      isInit = true;

      if (tailLogData.ListOfFilter.Count != 0)
        filterId = tailLogData.ListOfFilter[tailLogData.ListOfFilter.Count - 1].Id + 1;
      else
        filterId = 0;
    }

    #region ClickEvents

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void btnFont_Click(object sender, RoutedEventArgs e)
    {
      System.Drawing.Font filterFont = filterData.FilterFontType;
      System.Windows.Forms.FontDialog fontManager = new System.Windows.Forms.FontDialog
      {
        ShowEffects = true,
        Font = filterFont,
        FontMustExist = true,
        Color = filterData.FilterColor,
        ShowColor = true
      };

      if (fontManager.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
      {
        filterFont = new System.Drawing.Font(fontManager.Font.FontFamily, fontManager.Font.Size, fontManager.Font.Style);
        filterData.FilterFontType = filterFont;
        filterData.FilterColor = fontManager.Color;

        ChangeState();
      }
    }

    private void btnNew_Click(object sender, RoutedEventArgs e)
    {
      fState = EFileManagerState.AddFile;
      btnCancel.IsEnabled = false;

      filterData = new FilterData
      {
        FilterFontType = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Regular),
        FilterColor = System.Drawing.Color.Black,
        Id = filterId
      };

      tailLogData.ListOfFilter.Add(filterData);

      SaveNow?.Invoke(this, EventArgs.Empty);

      dataGridFilters.Items.Refresh();
      SelectLastItemInDataGrid();

      filterId++;
    }

    private void btnCancelAdd_Click(object sender, RoutedEventArgs e)
    {
      switch (fState)
      {
      case EFileManagerState.AddFile:

      var lastItem = tailLogData.ListOfFilter[tailLogData.ListOfFilter.Count - 1];
      tailLogData.ListOfFilter.Remove(lastItem);
      dataGridFilters.Items.Refresh();
      break;

      case EFileManagerState.EditItem:

      if (mementoFilterData != null)
        filterData.RestoreFromMemento(mementoFilterData);
      break;

      case EFileManagerState.OpenFileManager:
      break;

      case EFileManagerState.EditFilter:
      break;

      default:
      throw new ArgumentOutOfRangeException();
      }

      fState = EFileManagerState.OpenFileManager;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      var findResource = Application.Current.FindResource("QDeleteDataGridItem");

      if (findResource == null || MessageBox.Show(findResource.ToString(), LogFile.APPLICATION_CAPTION, MessageBoxButton.YesNo,
            MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
        return;

      var index = dataGridFilters.SelectedIndex;
      filterData = dataGridFilters.SelectedItem as FilterData;

      tailLogData.ListOfFilter.RemoveAt(index);
      dataGridFilters.Items.Refresh();

      SaveNow?.Invoke(this, EventArgs.Empty);

      if (tailLogData.ListOfFilter.Count != 0)
        SelectLastItemInDataGrid();
    }

    #endregion

    #region Events

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      foreach (FilterData item in tailLogData.ListOfFilter)
      {
        if (!string.IsNullOrWhiteSpace(item.Filter))
          continue;

        var findResource = Application.Current.FindResource("FilterNotEmpty");

        if (findResource != null)
          MessageBox.Show(findResource.ToString(), LogFile.MSGBOX_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);

        e.Cancel = true;
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      dataGridFilters.DataContext = tailLogData.ListOfFilter;
      filterProperties.DataContext = filterData;
      fState = EFileManagerState.OpenFileManager;
    }

    private void dataGridFiles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      e.Handled = true;

      filterData = dataGridFilters.SelectedItem as FilterData;
      mementoFilterData = filterData?.SaveToMemento();
    }

    private void textBoxFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(textBoxFilter.Text))
      {
        btnCancel.IsEnabled = true;
        ChangeState();
      }
      else
        btnCancel.IsEnabled = false;
    }

    private void textBoxDescription_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      if (!string.IsNullOrEmpty(textBoxDescription.Text))
        ChangeState();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnCancel_Click(this, e);
    }

    #endregion

    #region HelperFunctions

    private void SelectLastItemInDataGrid()
    {
      if (dataGridFilters.Items.Count <= 0)
        return;

      dataGridFilters.SelectedItem = tailLogData.ListOfFilter[tailLogData.ListOfFilter.Count - 1];
      dataGridFilters.ScrollIntoView(tailLogData.ListOfFilter[tailLogData.ListOfFilter.Count - 1]);
    }

    private void ChangeState()
    {
      if (!isInit)
        return;

      if (fState == EFileManagerState.OpenFileManager && mementoFilterData != null && filterData != null && !filterData.EqualsProperties(mementoFilterData))
        fState = EFileManagerState.EditItem;

      SaveNow?.Invoke(this, EventArgs.Empty);
    }

    #endregion
  }
}
