using System;
using System.IO;
using System.Text;
using TailForWin.Data.Enums;


namespace TailForWin.Utils
{
  public class FileReader : IDisposable
  {
    private FileStream fs;
    private StreamReader reader;
    private Encoding fileEncoding;


    public void Dispose()
    {
      if (fs != null)
      {
        fs.Dispose();
        fs = null;
      }

      if (reader == null)
        return;

      reader.Dispose();
      reader = null;
    }

    /// <summary>
    /// Read file to tail
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="encode">By default encode is null but you can set your own encoding</param>
    public bool OpenTailFileStream(string fileName, Encoding encode = null)
    {
      if (!File.Exists(fileName))
        return (false);

      try
      {
        fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        if (encode != null)
          fileEncoding = encode;
        else
          DetectEncoding();

        reader = new StreamReader(fs, fileEncoding);

        return (true);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{0}, exception: {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex));
        Dispose();
      }
      return (false);
    }

    /// <summary>
    /// Read last n line from file
    /// </summary>
    /// <param name="nLines">Number of line to read</param>
    /// <returns>Readed lines</returns>
    public void ReadLastNLines(int nLines)
    {
      reader.BaseStream.Seek(0, SeekOrigin.End);
      LinesRead = 0;

      while ((LinesRead < nLines) && (reader.BaseStream.Position > 0))
      {
        reader.BaseStream.Position--;
        int c = reader.BaseStream.ReadByte();

        if (reader.BaseStream.Position > 0)
          reader.BaseStream.Position--;
        if (c == Convert.ToInt32('\n'))
          LinesRead++;
      }
    }

    /// <summary>
    /// Check if file exists
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <returns>If exist true otherwise false</returns>
    public static bool FileExists(string fileName)
    {
      if (File.Exists(fileName))
        return (true);
      else
        return (false);
    }

    /// <summary>
    /// Close filestreamer and streamreader
    /// </summary>
    public void CloseFileStream()
    {
      fs.Close();
      reader.Close();
    }

    private void DetectEncoding()
    {
      fileEncoding = EncodingDetector.GetEncoding(fs);

      if (fileEncoding == null)
        fileEncoding = Encoding.Default;
    }

    #region Properties

    /// <summary>
    /// Get file encoding
    /// </summary>
    public Encoding FileEncoding
    {
      get
      {
        return (fileEncoding);
      }
      set
      {
        fileEncoding = value;
      }
    }

    /// <summary>
    /// Get filestream
    /// </summary>
    public FileStream TailFileStream
    {
      get
      {
        return (fs);
      }
    }

    /// <summary>
    /// Get streamreader
    /// </summary>
    public StreamReader TailStreamReader
    {
      get
      {
        return (reader);
      }
    }

    /// <summary>
    /// Get filesize in KBytes
    /// </summary>
    public Double FileSizeKB
    {
      get
      {
        try
        {

          if (reader != null)
            return (reader.BaseStream.Length / 1024.00);
          else
            return (Double.NaN);
        }
        catch (Exception ex)
        {
          ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{0}, exception: {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex));
          return (Double.NaN);
        }
      }
    }

    /// <summary>
    /// Lines read in logfile
    /// </summary>
    public Int64 LinesRead
    {
      get;
      set;
    }

    #endregion
  }
}
