using System;
using System.Runtime.InteropServices;


namespace Org.Vs.TailForWin.Data.Native
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
    public Int32 X;

    /// <summary>
    /// The y-coordinate of the point.
    /// </summary>
    public Int32 Y;
  }
}
