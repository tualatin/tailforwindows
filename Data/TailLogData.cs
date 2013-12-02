using System.IO;
using System;
using System.Drawing;
using System.Text;
using System.Collections.ObjectModel;


namespace TailForWin.Data
{
  /// <summary>
  /// Dataobject for Tabproperties
  /// </summary>
  public class TailLogData: INotifyMaster, IDisposable
  {
    private Font fontType;
    private string guiFont;
    private bool killSpace;
    private bool wrap;
    private bool timeStamp;
    private string fileName;
    private string file;


    public void Dispose ()
    {
      if (fontType != null)
      {
        fontType.Dispose ( );
        fontType = null;
      }
    }

    /// <summary>
    /// Filename
    /// </summary>
    public string FileName
    {
      get
      {
        return (fileName);
      }
      set
      {
        fileName = value;
        File = Path.GetFileName (FileName);
        OnPropertyChanged ("FileName");
      }
    }

    /// <summary>
    /// Filename without path
    /// </summary>
    public string File
    {
      get
      {
        return (file);
      }
      set
      {
        file = value;
        OnPropertyChanged ("File");
      }
    }

    /// <summary>
    /// Wrap text in textbox
    /// </summary>
    public bool Wrap
    {
      get
      {
        return (wrap);
      }
      set
      {
        wrap = value;
        OnPropertyChanged ("Wrap");
      }
    }

    /// <summary>
    /// Remove extra space in each line
    /// </summary>
    public bool KillSpace
    {
      get
      {
        return (killSpace);
      }
      set
      {
        killSpace = value;
        OnPropertyChanged ("KillSpace");
      }
    }

    /// <summary>
    /// Refreshrate for thread
    /// </summary>
    public SettingsData.ETailRefreshRate RefreshRate
    {
      get;
      set;
    }

    /// <summary>
    /// Timestamp in taillog
    /// </summary>
    public bool Timestamp
    {
      get
      {
        return (timeStamp);
      }
      set
      {
        timeStamp = value;
        OnPropertyChanged ("Timestamp");
      }
    }

    /// <summary>
    /// Font type
    /// </summary>
    public Font FontType
    {
      get
      {
        return (fontType);
      }
      set
      {
        fontType = value;
        GuiFont = string.Format ("{0} ({1}) {2} {3}", FontType.Name, FontType.Size, FontType.Italic ? "Italic" : string.Empty, FontType.Bold ? "Bold" : string.Empty);
        OnPropertyChanged ("FontType");
      }
    }

    /// <summary>
    /// Gui font type
    /// </summary>
    public string GuiFont
    {
      get
      {
        return (guiFont);
      }
      set
      {
        guiFont = value;
        OnPropertyChanged ("GuiFont");
      }
    }

    /// <summary>
    /// ThreadPriority
    /// </summary>
    public System.Threading.ThreadPriority ThreadPriority
    {
      get;
      set;
    }

    /// <summary>
    /// Last refresh time
    /// </summary>
    public DateTime LastRefreshTime
    {
      get;
      set;
    }

    private ObservableCollection<FilterData> listOfFilter;

    /// <summary>
    /// List of filters
    /// </summary>
    public ObservableCollection<FilterData> ListOfFilter
    {
      get
      {
        return (listOfFilter);
      }
      set
      {
        listOfFilter = value;
        OnPropertyChanged ("ListOfFilter");
      }
    }

    /// <summary>
    /// File encoding
    /// </summary>
    public Encoding FileEncoding
    {
      get;
      set;
    }
  }
}
