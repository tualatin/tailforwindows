using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.ExportFormatModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.PlugIns.ExportFormatModule.ViewModels
{
  /// <summary>
  /// Export format selector view model
  /// </summary>
  public class ExportFormatSelectorViewModel : NotifyMaster, IExportFormatSelectorViewModel
  {
    private EExportFormat _selectedExportFormat;

    /// <summary>
    /// Selected export format
    /// </summary>
    public EExportFormat SelectedExportFormat
    {
      get => _selectedExportFormat;
      set
      {
        if ( Equals(value, _selectedExportFormat) )
          return;

        _selectedExportFormat = value;
        OnPropertyChanged();
      }
    }

    private bool _csvExport;

    /// <summary>
    /// CSV export
    /// </summary>
    public bool CsvExport
    {
      get => _csvExport;
      set
      {
        if ( value == _csvExport )
          return;

        _csvExport = value;
        OnPropertyChanged();
      }
    }

    private bool _excelExport;

    /// <summary>
    /// Excel export
    /// </summary>
    public bool ExcelExport
    {
      get => _excelExport;
      set
      {
        if ( value == _excelExport )
          return;

        _excelExport = value;
        OnPropertyChanged();
      }
    }

    private bool _openDocumentExport;

    /// <summary>
    /// OpenDocument export
    /// </summary>
    public bool OpenDocumentExport
    {
      get => _openDocumentExport;
      set
      {
        if ( value == _openDocumentExport )
          return;

        _openDocumentExport = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExportFormatSelectorViewModel()
    {
      SelectedExportFormat = SettingsHelperController.CurrentSettings.ExportFormat;

      if ( SettingsHelperController.CurrentSettings.ExportFormat.HasFlag(EExportFormat.Csv) )
        CsvExport = true;

      if ( SettingsHelperController.CurrentSettings.ExportFormat.HasFlag(EExportFormat.Excel) )
        ExcelExport = true;

      if ( SettingsHelperController.CurrentSettings.ExportFormat.HasFlag(EExportFormat.OpenDocument) )
        OpenDocumentExport = true;
    }

    #region Commands

    private ICommand _selectFormatFinishedCommand;

    /// <summary>
    /// Select format finished command
    /// </summary>
    public ICommand SelectFormatFinishedCommand => _selectFormatFinishedCommand ?? (_selectFormatFinishedCommand = new RelayCommand(ExecuteSelectFormatFinishedCommand));

    #endregion

    #region Command functions

    private void ExecuteSelectFormatFinishedCommand(object param)
    {
      if ( !(param is ExportFormatSelector window) )
        return;

      SelectedExportFormat = CsvExport ? EExportFormat.Csv : EExportFormat.None;

      if ( ExcelExport )
        SelectedExportFormat |= EExportFormat.Excel;
      if ( OpenDocumentExport )
        SelectedExportFormat |= EExportFormat.OpenDocument;

      SettingsHelperController.CurrentSettings.ExportFormat = SelectedExportFormat;
      window.DialogResult = true;
      window.Close();
    }

    #endregion
  }
}
