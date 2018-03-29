using System;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils.Interfaces;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Interaction logic for DragWindow.xaml
  /// </summary>
  public partial class DragWindow : IDragWindow
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public DragWindow()
    {
      InitializeComponent();
    }

    public ItemCollection TabItems => throw new NotImplementedException();

    public void AddTabItem(string tabHeader, Control content)
    {
      throw new NotImplementedException();
    }

    public void RemoveTabItem(DragSupportTabItem tabItem)
    {
      throw new NotImplementedException();
    }
  }
}
