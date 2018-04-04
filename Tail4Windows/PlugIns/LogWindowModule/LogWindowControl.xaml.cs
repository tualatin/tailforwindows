using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Org.Vs.TailForWin.Core.Enums;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for LogWindowControl.xaml
  /// </summary>
  public partial class LogWindowControl : ILogWindow
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogWindowControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// LogWindowTabItem <see cref="DragSupportTabItem"/>
    /// </summary>
    public DragSupportTabItem LogWindowTabItem
    {
      get;
      set;
    }

    private EStatusbarState _logWindowState;

    /// <summary>
    /// Current LogWindowState <see cref="EStatusbarState"/>
    /// </summary>
    public EStatusbarState LogWindowState
    {
      get => _logWindowState;
      private set
      {
        if ( value == _logWindowState )
          return;

        _logWindowState = value;
        OnPropertyChanged();
      }
    }

    #region Click events

    private void ButtonStartTailClick(object sender, RoutedEventArgs e)
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Visible;
      LogWindowState = EStatusbarState.Busy;
    }

    private void ButtonStopTailClick(object sender, RoutedEventArgs e)
    {
      if ( LogWindowTabItem == null )
        return;

      LogWindowTabItem.TabItemBusyIndicator = Visibility.Collapsed;
      LogWindowState = EStatusbarState.Default;
    }

    private void ButtonOpenFileClick(object sender, RoutedEventArgs e)
    {
      if ( FileOpenDialog.OpenDialog("All files(*.*)|*.*", EnvironmentContainer.ApplicationTitle, out string fileName) )
      {
      }
    }

    private void ButtonAddToFileManagerClick(object sender, RoutedEventArgs e)
    {
      throw new System.NotImplementedException();
    }

    #endregion

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      var handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
