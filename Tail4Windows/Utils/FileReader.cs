using System;
using System.IO;
using System.Text;
using log4net;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// FileReader
  /// </summary>
  public class FileReader : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileReader));

    private FileStream fs;
    private StreamReader reader;
    private Encoding fileEncoding;


    /// <summary>
    /// Releases all resources used by the FileReader.
    /// </summary>
    public void Dispose()
    {
      if(fs != null)
      {
        fs.Dispose();
        fs = null;
      }

      if(reader == null)
        return;

      reader.Dispose();
      reader = null;
    }

    /// <summary>
    /// Read file to tail
    /// </summary>
    /// <param name="fileName">Name of file</param>
    /// <param name="encode">By default encode is null but you can set your own encoding</param>
    /// <returns><code>True</code>if file exists otherwise <code>false</code></returns>
    public bool OpenTailFileStream(string fileName, Encoding encode = null)
    {
      if(!File.Exists(fileName))
        return (false);

      try
      {
        fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        if(encode != null)
          fileEncoding = encode;
        else
          DetectEncoding();

        reader = new StreamReader(fs, fileEncoding);

        return (true);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        Dispose();
      }
      return (false);
    }

    /// <summary>
    /// Change current file encoding to new encoding
    /// </summary>
    /// <param name="encode">New encoding</param>
    /// <exception cref="ArgumentException">If encode is null or filestream is null</exception>
    public void ChangeFileEncoding(Encoding encode)
    {
      Arg.NotNull(encode, "Encoding");
      Arg.NotNull(fs, "FileStream");

      reader = null;
      reader = new StreamReader(fs, encode);
    }

    /// <summary>
    /// Read last n line from file
    /// </summary>
    /// <param name="nLines">Number of line to read</param>
    /// <returns>Readed lines</returns>
    public void ReadLastNLines(int nLines)
    {
      if(reader == null)
      {
        LOG.Info("StreamReader is null!");
        return;
      }

      reader.BaseStream.Seek(0, SeekOrigin.End);
      LinesRead = 0;

      while((LinesRead < nLines) && (reader.BaseStream.Position > 0))
      {
        reader.BaseStream.Position--;
        int c = reader.BaseStream.ReadByte();

        if(reader.BaseStream.Position > 0)
          reader.BaseStream.Position--;
        if(c == Convert.ToInt32('\n'))
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
      return (File.Exists(fileName));
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
      fileEncoding = EncodingDetector.GetEncoding(fs) ?? Encoding.Default;
    }

    #region Properties

    /// <summary>
    /// Get file encoding
    /// </summary>
    public Encoding FileEncoding
    {
      get => fileEncoding;
      set => fileEncoding = value;
    }

    /// <summary>
    /// Get filestream
    /// </summary>
    public FileStream TailFileStream => fs;

    /// <summary>
    /// Get streamreader
    /// </summary>
    public StreamReader TailStreamReader => reader;

    /// <summary>
    /// Get filesize in KBytes
    /// </summary>
    public Double FileSizeKb
    {
      get
      {
        try
        {
          if(reader?.BaseStream == null)
            return (Double.NaN);

          return (reader.BaseStream.Length / 1024.00);
        }
        catch(Exception ex)
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          return (Double.NaN);
        }
      }
    }

    /// <summary>
    /// Lines read in logfile
    /// </summary>
    private Int64 LinesRead
    {
      get;
      set;
    }

    #endregion
  }
}
