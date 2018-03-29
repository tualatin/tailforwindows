using System;
using System.Windows;
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

    /// <summary>
    /// Creates a new <see cref="DragWindow"/>
    /// </summary>
    /// <param name="left">Left</param>
    /// <param name="top">Top</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="tabItem"><see cref="DragSupportTabItem"/></param>
    /// <returns><see cref="DragWindow"/></returns>
    public static DragWindow CreateTabWindow(double left, double top, double width, double height, DragSupportTabItem tabItem)
    {
      var dragWindow = new DragWindow
      {
        Left = left,
        Top =  top,
        Width = width,
        Height = height,
        WindowStartupLocation = WindowStartupLocation.Manual
      };

      dragWindow.Show();
      dragWindow.Activate();
      dragWindow.Focus();

      return dragWindow;
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
