using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Encoding detector class
  /// </summary>
  public static class EncodingDetector
  {
    private const int DefaultBufferSize = 128;

    /// <summary>
    /// Get current file encoding asnyc
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Current file encoding as <see cref="Encoding"/></returns>
    public static async Task<Encoding> GetEncodingAsync(string path) => await Task.Run(() => GetEncoding(path)).ConfigureAwait(false);

    /// <summary>
    /// Get current file encoding
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Current file encoding as <see cref="Encoding"/></returns>
    public static Encoding GetEncoding(string path)
    {
      Encoding encoding = Encoding.Default;
      byte[] byteBuffer = new byte[DefaultBufferSize];

      using ( FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read) )
      {
        int byteLength = fs.Read(byteBuffer, 0, byteBuffer.Length);

        if ( byteLength < 2 )
          return null;

        if ( byteBuffer[0] == 0xFE && byteBuffer[1] == 0xFF )
        {
          // Big Endian Unicode
          encoding = new UnicodeEncoding(true, true);
        }
        else if ( byteBuffer[0] == 0xFF && byteBuffer[1] == 0xFE )
        {
          // Little Endian Unicode, or possibly litte endian UTF32
          if ( byteLength < 4 || byteBuffer[2] != 0 || byteBuffer[3] != 0 )
          {
            encoding = new UnicodeEncoding(false, true);
          }
#if FEATURE_UTF32
          else
          {
            encoding = new UTF32Encoding(false, true);
          }
#endif
        }
        else if ( byteLength >= 3 && byteBuffer[0] == 0xEF && byteBuffer[1] == 0xBB && byteBuffer[2] == 0xBF )
        {
          // UTF-8
          encoding = Encoding.UTF8;
        }
#if FEATURE_UTF32
        else if (byteLength >= 4 && byteBuffer[0] == 0 && byteBuffer[1] == 0 && byteBuffer[2] == 0xFE && byteBuffer[3] == 0xFF)
        {
          // Big Endian UTF32
          encoding = new UTF32Encoding(true, true);
        }
#endif
      }
      return encoding;
    }
  }
}
