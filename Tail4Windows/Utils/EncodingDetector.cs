using System.IO;
using System.Text;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Encoding detector class
  /// </summary>
  public class EncodingDetector
  {
    /// <summary>
    /// Get current file encoding
    /// </summary>
    /// <param name="fs">FileStream of log file</param>
    /// <returns>A valid file encoding</returns>
    public static Encoding GetEncoding(FileStream fs)
    {
      // Read the BOM
      var bom = new byte[4];
      fs.Read(bom, 0, 4);

      // Analyze the BOM
      if(bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
        return (Encoding.UTF7);
      if(bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
        return (Encoding.UTF8);
      if(bom[0] == 0xff && bom[1] == 0xfe)
        return (Encoding.Unicode); //UTF-16LE
      if(bom[0] == 0xfe && bom[1] == 0xff)
        return (Encoding.BigEndianUnicode); //UTF-16BE
      if(bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
        return (Encoding.UTF32);

      return (Encoding.ASCII);
    }
  }
}
