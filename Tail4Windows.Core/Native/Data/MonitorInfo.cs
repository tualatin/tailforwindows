using System.Runtime.InteropServices;

namespace Org.Vs.Tail4Win.Core.Native.Data
{
  /// <summary>
  /// The MONITORINFO structure contains information about a display monitor.
  /// </summary>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public class MonitorInfo
  {
    /// <summary>
    /// The size of the structure, in bytes.
    /// </summary>            
    public int cbSize = Marshal.SizeOf(typeof(MonitorInfo));

    /// <summary>
    /// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, 
    /// some of the rectangle's coordinates may be negative values.
    /// </summary>            
    public Rect rcMonitor = new Rect();

    /// <summary>
    /// A RECT structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, 
    /// some of the rectangle's coordinates may be negative values.
    /// </summary>            
    public Rect rcWork = new Rect();

    /// <summary>
    /// A set of flags that represent attributes of the display monitor.
    /// </summary>            
    public int dwFlags = 0;
  }
}
