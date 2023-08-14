using System.IO;
using System.IO.Compression;
using System.Text;
using log4net.Appender;

namespace Org.Vs.TailForWin.Core.Logging
{
  /// <summary>
  /// Zipping rolling file appender
  /// </summary>
  public class ZippingRollingFileAppender : RollingFileAppender
  {
    /// <summary>
    /// Zips files if they get rolled.
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/26276296/how-to-implement-auto-archiving-for-log-file-using-log4net"/>
    protected override void AdjustFileBeforeAppend()
    {
      var previousFile = File; // the file that may need to get zipped
      base.AdjustFileBeforeAppend();

      if ( File != previousFile )
      {
        // ### zip the file ###
        // maybe this should be done in a background thread so the logging does not get blocked for this


        //using (ZipFile zip = new ZipFile(File + ".zip"))
        //{
        //  string newFile = DateTime.Now.ToString("HHmmss") + fa.Name;
        //  zip.AddFile(File).FileName = newFile;
        //  zip.Save(File + ".zip");
        //}

        //As of v1.7, the DotNetZip distribution now includes a version built specifically for the .NET Compact Framework, either v2.0 or v3.5.
        // http://www.codeplex.com/DotNetZip/Release/ProjectReleases.aspx. 
        // It is about ~70k DLL. It does zip, unzip, zip editing, passwords, ZIP64, unicode, streams, and more.
        //
        // DotNetZip is 100% managed code, open source, and free/gratis to use. It's also very simple and easy.

        //try
        //{
        //  using (var zip1 = Ionic.Zip.ZipFile.Read(zipToUnpack))
        //  {
        //    foreach (var entry in zip1)
        //    {
        //      entry.Extract(dir, ExtractExistingFileAction.OverwriteSilently);
        //    }
        //  }
        //}
        //catch (Exception ex)
        //{
        //  MessageBox.Show("Exception! " + ex);
        //}
      }
    }

    /// <summary>
    /// Compress a string
    /// </summary>
    /// <param name="str">String to compress</param>
    /// <returns>Compressed archive</returns>
    private byte[] Compress(string str)
    {
      var bytes = Encoding.UTF8.GetBytes(str);

      using ( var msi = new MemoryStream(bytes) )
      using ( var mso = new MemoryStream() )
      {
        using ( var gs = new GZipStream(mso, CompressionMode.Compress) )
        {
          CopyTo(msi, gs);
        }
        return mso.ToArray();
      }
    }

    private void CopyTo(Stream src, Stream dest)
    {
      byte[] bytes = new byte[4096];
      int cnt;

      while ( (cnt = src.Read(bytes, 0, bytes.Length)) != 0 )
      {
        dest.Write(bytes, 0, cnt);
      }
    }
  }
}
