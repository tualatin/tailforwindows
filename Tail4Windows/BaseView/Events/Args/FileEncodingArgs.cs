using System;
using System.Text;


namespace Org.Vs.TailForWin.BaseView.Events.Args
{
  /// <summary>
  /// FileEncoding event args
  /// </summary>
  public class FileEncodingArgs : EventArgs
  {
    /// <summary>
    /// <see cref="Encoding"/>
    /// </summary>
    public Encoding Encoding
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="encoding"><see cref="Encoding"/></param>
    public FileEncodingArgs(Encoding encoding) => Encoding = encoding;
  }
}
