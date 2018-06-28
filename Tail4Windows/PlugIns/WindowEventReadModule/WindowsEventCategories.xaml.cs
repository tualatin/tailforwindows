using Org.Vs.TailForWin.PlugIns.WindowEventReadModule.Interfaces;

namespace Org.Vs.TailForWin.PlugIns.WindowEventReadModule
{
  /// <summary>
  /// Interaction logic for WindowsEventCategories.xaml
  /// </summary>
  public partial class WindowsEventCategories
  {
    #region Properties

    /// <summary>
    /// <see cref="IWindowsEventCategoriesViewModel"/>
    /// </summary>
    public IWindowsEventCategoriesViewModel WindowsEventCategoriesViewModel
    {
      get;
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public WindowsEventCategories()
    {
      InitializeComponent();
      WindowsEventCategoriesViewModel = (IWindowsEventCategoriesViewModel) DataContext;
    }
  }
}
