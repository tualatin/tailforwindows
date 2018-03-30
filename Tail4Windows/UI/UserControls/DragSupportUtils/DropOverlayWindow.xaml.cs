using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;


namespace Org.Vs.TailForWin.UI.UserControls.DragSupportUtils
{
  /// <summary>
  /// Interaction logic for DropOverlayWindow.xaml
  /// </summary>
  public sealed partial class DropOverlayWindow : INotifyPropertyChanged
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public DropOverlayWindow() => InitializeComponent();

    /// <summary>
    /// Is mouse over target
    /// </summary>
    /// <param name="mousePos"></param>
    /// <returns></returns>
    public bool IsMouseOverTabTarget(Point mousePos)
    {
      Point buttonPosToScreen = BtnDropTarget.PointToScreen(new Point(0, 0));
      PresentationSource source = PresentationSource.FromVisual(this);

      if ( source?.CompositionTarget == null )
        return false;

      Point targetPos = source.CompositionTarget.TransformFromDevice.Transform(buttonPosToScreen);
      bool isMouseOver = mousePos.X > targetPos.X && mousePos.X < targetPos.X + BtnDropTarget.Width && mousePos.Y > targetPos.Y && mousePos.Y < targetPos.Y + BtnDropTarget.Height;
      IsTabTargetOver = isMouseOver;

      return isMouseOver;
    }

    bool _isTabTargetOver;

    /// <summary>
    /// Is tabtarget over
    /// </summary>
    public bool IsTabTargetOver
    {
      get => _isTabTargetOver;
      set
      {
        if ( _isTabTargetOver == value )
          return;

        _isTabTargetOver = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="propertyName">Name property</param>
    private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
