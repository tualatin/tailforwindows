using System;
using Org.Vs.TailForWin.PlugIns.GoToLineModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.GoToLineModule
{
  /// <summary>
  /// Interaction logic for GoToLine.xaml
  /// </summary>
  public partial class GoToLine
  {
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="min">Min line number</param>
    /// <param name="max">Max line number</param>
    /// <param name="windowId"><see cref="Guid"/></param>
    public GoToLine(int min, int max, Guid windowId)
    {
      InitializeComponent();

      var vm = (GoToLineViewModel) DataContext;
      vm.MinLines = min;
      vm.MaxLines = max;
      vm.ParentWindowId = windowId;
    }
  }
}
