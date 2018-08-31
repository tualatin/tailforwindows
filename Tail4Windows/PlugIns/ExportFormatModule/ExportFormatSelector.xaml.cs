using Org.Vs.TailForWin.Controllers.PlugIns.ExportFormatModule.Enums;
using Org.Vs.TailForWin.Controllers.PlugIns.ExportFormatModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.ExportFormatModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.ExportFormatModule
{
  /// <summary>
  /// Interaction logic for ExportFormatSelector.xaml
  /// </summary>
  public partial class ExportFormatSelector
  {
    private readonly IExportFormatSelectorViewModel _exportFormatSelectorViewModel;

    /// <summary>
    /// Export format
    /// </summary>
    public EExportFormat ExportFormat => _exportFormatSelectorViewModel?.SelectedExportFormat ?? EExportFormat.Csv;

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExportFormatSelector()
    {
      InitializeComponent();
      _exportFormatSelectorViewModel = (ExportFormatSelectorViewModel) DataContext;
    }
  }
}
