using System.Runtime.InteropServices;

namespace Org.Vs.TailForWin.Core.Native.Data
{
  /// <summary>
  /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
  /// </summary>
  [StructLayout(LayoutKind.Sequential, Pack = 0)]
  public struct Rect
  {
    /// <summary>
    /// The x-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int left;

    /// <summary>
    /// The y-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int top;

    /// <summary>
    /// The x-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int right;

    /// <summary>
    /// The y-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int bottom;
  }
}
