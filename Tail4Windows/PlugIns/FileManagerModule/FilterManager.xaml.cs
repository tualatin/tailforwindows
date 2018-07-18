using System.ComponentModel;


namespace Org.Vs.TailForWin.PlugIns.FileManagerModule
{
  /// <summary>
  /// Interaction logic for FilterManager.xaml
  /// </summary>
  public partial class FilterManager
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FilterManager()
    {
      InitializeComponent();
      Closing += OnFilterManagerClosing;
    }

    private void OnFilterManagerClosing(object sender, CancelEventArgs e) => FilterDataGrid.SaveDataGridOptions();
  }
}
