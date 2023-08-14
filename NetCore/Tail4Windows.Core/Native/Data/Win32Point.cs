using System.Runtime.InteropServices;

namespace Org.Vs.TailForWin.Core.Native.Data
{
  /// <summary>
  /// The POINT structure defines the x- and y- coordinates of a point.
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  internal struct Win32Point
  {
    /// <summary>
    /// The x-coordinate of the point.
    /// </summary>
    public int X;

    /// <summary>
    /// The y-coordinate of the point.
    /// </summary>
    public int Y;
  }
}
