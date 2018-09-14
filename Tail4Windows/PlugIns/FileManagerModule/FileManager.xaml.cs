using System;
using System.ComponentModel;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule
{
  /// <summary>
  /// Interaction logic for FileManager.xaml
  /// </summary>
  public partial class FileManager
  {
    private readonly IFileManagerViewModel _viewModel;

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
    }

    private void OnFileManagerClosing(object sender, CancelEventArgs e) => TailManagerDataGrid.SaveDataGridOptions();
  }
}
