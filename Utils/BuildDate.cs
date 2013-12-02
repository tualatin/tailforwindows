using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;


namespace TailForWin.Utils
{
  public static class BuildDate
  {
    struct _IMAGE_FILE_HEADER
    {
      public ushort Machine;
      public ushort NumberOfSections;
      public uint TimeDateStamp;
      public uint PointerToSymbolTable;
      public uint NumberOfSymbols;
      public ushort SizeOfOptionalHeader;
      public ushort Characteristics;
    };

    public static DateTime GetBuildDateTime (Assembly assembly)
    {
      if (File.Exists (assembly.Location))
      {
        var buffer = new byte[Math.Max (Marshal.SizeOf (typeof (_IMAGE_FILE_HEADER)), 4)];

        using (var fileStream = new FileStream (assembly.Location, FileMode.Open, FileAccess.Read))
        {
          fileStream.Position = 0x3C;
          fileStream.Read (buffer, 0, 4);
          fileStream.Position = BitConverter.ToUInt32 (buffer, 0);
          fileStream.Read (buffer, 0, 4);
          fileStream.Read (buffer, 0, buffer.Length);
        }

        var pinnedBuffer = GCHandle.Alloc (buffer, GCHandleType.Pinned);

        try
        {
          var coffHeader = (_IMAGE_FILE_HEADER) Marshal.PtrToStructure (pinnedBuffer.AddrOfPinnedObject ( ), typeof (_IMAGE_FILE_HEADER));

          return (TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1) + new TimeSpan (coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond)));
        }
        finally 
        {
          pinnedBuffer.Free ( );
        }
      }
      return (new DateTime ( ));
    }
  }
}
