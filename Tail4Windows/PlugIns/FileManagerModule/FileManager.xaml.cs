using System;
using System.ComponentModel;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule
{
  /// <summary>
  /// Interaction logic for FileManager.xaml
  /// </summary>
  public partial class FileManager
  {
    private readonly IFileManagerViewModel _viewModel;
    private bool? _showExtendedSettingsOldState;
    private int _clickClount;

    /// <summary>
    /// Current Window ID
    /// </summary>
    public Guid WindowId
    {
      get => _viewModel?.WindowId ?? Guid.Empty;
      set
      {
        if ( _viewModel == null )
          return;

        _viewModel.WindowId = value;
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FileManager()
    {
      InitializeComponent();
      Closing += OnFileManagerClosing;
      _viewModel = (IFileManagerViewModel) DataContext;
      _clickClount = 0;

      if ( _viewModel == null )
      {
        return;
      }

      _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName == nameof(_viewModel.SelectedItems) )
      {
        if ( _viewModel.SelectedItems.Count > 1 )
        {
          if ( _clickClount == 0 )
          {
            _showExtendedSettingsOldState = ExtendedSettingsToggleButton.IsChecked;
            ExtendedSettingsToggleButton.IsChecked = false;
            ExtendedSettingsToggleButton.IsEnabled = false;
            _clickClount++;
          }
        }
        else
        {
          ExtendedSettingsToggleButton.IsChecked = _showExtendedSettingsOldState ?? ExtendedSettingsToggleButton.IsChecked;
          ExtendedSettingsToggleButton.IsEnabled = true;

          if ( _clickClount == 0 )
          {
            return;
          }

          _clickClount--;
        }
      }
    }

    private void OnFileManagerClosing(object sender, CancelEventArgs e)
    {
      TailManagerDataGrid.SaveDataGridOptions();

      if ( _viewModel == null )
      {
        return;
      }

      _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
      ExtendedSettingsToggleButton.IsEnabled = true;
      ExtendedSettingsToggleButton.IsChecked = SettingsHelperController.CurrentSettings.ShowExtendedSettings;
    }
  }
}
