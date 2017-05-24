using System.ComponentModel;
using System.Windows;
using log4net;


namespace Org.Vs.TailForWin.UI
{
  /// <summary>
  /// Interaction logic for OverlayDragWnd.xaml
  /// </summary>
  public partial class OverlayDragWnd : INotifyPropertyChanged
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(OverlayDragWnd));

    private bool isTabTargetOver;

    /// <summary>
    /// Is new tab window over current window
    /// </summary>
    public bool IsTabTargetOver
    {
      get => isTabTargetOver;
      set
      {
        isTabTargetOver = value;
        OnPropertyChanged("IsTabTargetOver");
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public OverlayDragWnd()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Is mouse over tab target
    /// </summary>
    /// <param name="mousePos">Current mouse position</param>
    /// <returns>If mouse pointer is over <c>true</c> otherwise <c>false</c></returns>
    public bool IsMouseOverTabTarget(Point mousePos)
    {
      Point buttonPosToScreen = btnDropTarget.PointToScreen(new Point(0, 0));
      PresentationSource source = PresentationSource.FromVisual(this);

      if(source?.CompositionTarget == null)
        return false;

      Point targetPos = source.CompositionTarget.TransformFromDevice.Transform(buttonPosToScreen);

      bool isMouseOver = mousePos.X > targetPos.X && mousePos.X < targetPos.X + btnDropTarget.Width && mousePos.Y > targetPos.Y && mousePos.Y < targetPos.Y + btnDropTarget.Height;
      IsTabTargetOver = isMouseOver;

      LOG.Trace("{0} IsMouseOver {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, isMouseOver);

      return isMouseOver;
    }

    /// <summary>
    /// Represents the method that will handle the <c>PropertyChanged</c> event raised when a property is changed on a component.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
