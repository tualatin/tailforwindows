using System.Runtime.InteropServices;

namespace Org.Vs.Tail4Win.Shared.Native.Data
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
