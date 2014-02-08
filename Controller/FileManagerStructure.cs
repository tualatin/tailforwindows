using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using TailForWin.Data;
using System.Drawing;
using System;
using System.Globalization;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using TailForWin.Utils;


namespace TailForWin.Controller
{
  public class FileManagerStructure: INotifyMaster
  {
    XDocument fmDoc;
    readonly string fmFile = string.Empty;
    readonly List<FileManagerData> fmProperties;
    ObservableCollection<string> category = new ObservableCollection<string> ( );
    private const string XMLROOT = "fileManager";


    public FileManagerStructure (bool findHistory = false)
    {
      fmFile = Path.GetDirectoryName (System.Reflection.Assembly.GetEntryAssembly ( ).Location) + "\\FileManager.xml";

      if (!findHistory)
      {
        fmProperties = new List<FileManagerData> ( );
        LastFileId = -1;
        LastFilterId = -1;

        OpenFMDoc ( );
      }
    }

    #region HelperProperties

    /// <summary>
    /// List of categories
    /// </summary>
    public ObservableCollection<string> Category
    {
      get
      {
        return (category);
      }
      set
      {
        category = value;
        OnPropertyChanged ("Category");
      }
    }

    /// <summary>
    /// List of FileManager Properties
    /// </summary>
    public List<FileManagerData> FMProperties
    {
      get
      {
        return (fmProperties);
      }
    }

    /// <summary>
    /// Last Id of file node
    /// </summary>
    public int LastFileId
    {
      set;
      get;
    }

    /// <summary>
    /// Last Id of filter node
    /// </summary>
    public int LastFilterId
    {
      get;
      set;
    }

    #endregion

    /// <summary>
    /// Open FileManager XML document
    /// </summary>
    public void OpenFMDoc ()
    {
      if (File.Exists (fmFile))
      {
        ReadFMDoc ( );
      }
    }

    /// <summary>
    /// Read FileManager file
    /// </summary>
    public void ReadFMDoc ()
    {
      try
      {
        fmDoc = XDocument.Load (fmFile);

        foreach (XElement xe in fmDoc.Root.Descendants ("file"))
        {
          string category = GetCategory (xe);

          if (category != null)
            AddCategoryToDictionary (category, category);

          FileManagerData item = new FileManagerData ( )
          {
            ID = GetId (xe.Element ("id").Value),
            FileName = xe.Element ("fileName").Value,
            FontType = GetFont (xe.Element ("font")),
            KillSpace = IsKillSpace (xe.Element ("killSpace").Value),
            Wrap = IsLineWrap (xe.Element ("lineWrap").Value),
            Category = category,
            Description = xe.Element ("description").Value,
            Timestamp = IsTimeStamp (xe.Element ("timeStamp").Value),
            ThreadPriority = GetThreadPriority (xe.Element ("threadPriority").Value),
            RefreshRate = GetRefreshRate (xe.Element ("refreshRate").Value),
            NewWindow = IsNewWindow (xe.Element ("newWindow").Value),
            FileEncoding = GetEncoding (xe.Element ("fileEncoding").Value),
          };
          
          foreach (XElement filter in xe.Elements ("filter"))
          {
            FilterData data = GetFilters (filter);

            if (data != null)
              item.ListOfFilter.Add (data);
          }

          fmProperties.Add (item);
        }

        SortListIfRequired ( );
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("ReadFMDoc exception: {0}", ex));
      }
    }

    /// <summary>
    /// Save FileManager file
    /// </summary>
    public void SaveFMDoc ()
    {
      if (!File.Exists (fmFile))
        fmDoc = new XDocument (new XElement (XMLROOT));

      fmDoc.Save (@fmFile, SaveOptions.None);
    }

    #region FindHistory

    /// <summary>
    /// Wrap around in search dialogue
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Read find history section in XML file
    /// </summary>
    public void ReadFindHistory (ref ObservableDictionary<string, string> words)
    {
      try
      {
        if (File.Exists (fmFile))
        {
          fmDoc = XDocument.Load (fmFile);
          XElement findHistoryRoot = fmDoc.Root.Element ("FindHistory");

          if (findHistoryRoot != null)
          {
            string wrapAround = findHistoryRoot.Attribute ("wrap").Value;
            bool wrap = false;

            if (!bool.TryParse (wrapAround, out wrap))
              Wrap = false;
            else
              Wrap = wrap;

            foreach (XElement find in findHistoryRoot.Elements ("Find"))
            {
              words.Add (find.Attribute ("name").Value, find.Attribute ("name").Value);
            }
          }
        }
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("ReadFindHistory exception: {0}", ex));
      }
    }

    /// <summary>
    /// Save find history attribute wrap
    /// </summary>
    public XElement SaveFindHistoryWrap ( )
    {
      if (!File.Exists (fmFile))
        fmDoc = new XDocument (new XElement (XMLROOT));

      try
      {
        XElement findHistoryRoot = fmDoc.Root.Element ("FindHistory");

        if (findHistoryRoot != null)
          findHistoryRoot.Attribute ("wrap").Value = Wrap.ToString ( );
        else
        {
          findHistoryRoot = new XElement ("FindHistory");
          findHistoryRoot.Add (new XAttribute ("wrap", Wrap.ToString ( )));
          fmDoc.Root.Add (findHistoryRoot);
        }

        fmDoc.Save (@fmFile, SaveOptions.None);

        return (findHistoryRoot);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("SaveFindHistoryWrap exception: {0}", ex));
        return (null);
      }
    }

    /// <summary>
    /// Save find history attribute name
    /// </summary>
    /// <param name="searchWord">Find what word</param>
    public void SaveFindHistoryName (string searchWord)
    {
      if (!File.Exists (fmFile))
        fmDoc = new XDocument (new XElement (XMLROOT));

      try
      {
        XElement findHistoryRoot = fmDoc.Root.Element ("FindHistory");

        if (findHistoryRoot == null)
          findHistoryRoot = SaveFindHistoryWrap ( );

        XElement findHistoryFind = new XElement ("Find");
        findHistoryFind.Add (new XAttribute ("name", searchWord));
        findHistoryRoot.Add (findHistoryFind);

        fmDoc.Save (@fmFile, SaveOptions.None);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("SaveFindHistoryName exception: {0}", ex));
      }
    }

    #endregion

    /// <summary>
    /// Get a XML node by Id
    /// </summary>
    /// <param name="id">Reference of id</param>
    public FileManagerData GetNodeById (string id)
    {
      FileManagerData cmdParameterItem = null;

      if (!File.Exists (fmFile)) {
        System.Windows.MessageBox.Show (System.Windows.Application.Current.FindResource ("FileNotFound").ToString ( ), string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (cmdParameterItem);
      }

      if (fmProperties.Count == 0)
      {
        System.Windows.MessageBox.Show (System.Windows.Application.Current.FindResource ("NoContentFound").ToString ( ), string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (cmdParameterItem);
      }

      int iid;

      if (!int.TryParse (id, out iid))
        iid = -1;
      if (iid != -1)
        cmdParameterItem = fmProperties.Find (o => o.ID == iid);

      return (cmdParameterItem);
    }

    /// <summary>
    /// Add new node to FileManager
    /// </summary>
    /// <param name="property">FileManagerData property</param>
    public void AddNewNode (FileManagerData property)
    {
      if (!File.Exists (fmFile))
        fmDoc = new XDocument (new XElement (XMLROOT));

      if (property.FileEncoding == null)
      {
        FileReader reader = new FileReader ( );

        if (!reader.OpenTailFileStream (property.FileName))
        {
          System.Windows.MessageBox.Show (string.Format ("{0} '{1}'", System.Windows.Application.Current.FindResource ("FileNotFound"), property.File), string.Format ("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
          return;
        }

        property.FileEncoding = reader.FileEncoding;
        reader.Dispose ( );
      }

      var xElement = fmDoc.Element (XMLROOT);

      if (xElement != null)
        xElement.Add (AddNode (property));

      fmDoc.Save (@fmFile, SaveOptions.None);
    }

    public void UpdateNode (FileManagerData property)
    {
      try
      {
        if (fmDoc.Root != null)
        {
          XElement node = (fmDoc.Root.Descendants ("file").Where (x =>
                                                                  {
                                                                    var element = x.Element ("id");
                                                                    return element != null && element.Value == property.ID.ToString ( );
                                                                  })).SingleOrDefault ( );

          if (node != null)
          {
            var element = node.Element ("fileEncoding");

            if (element != null)
              element.Value = property.FileEncoding.HeaderName;

            var xElement1 = node.Element ("fileName");

            if (xElement1 != null)
              xElement1.Value = property.FileName;

            var element1 = node.Element ("timeStamp");

            if (element1 != null)
              element1.Value = property.Timestamp.ToString ( );

            var xElement2 = node.Element ("killSpace");
            
            if (xElement2 != null)
              xElement2.Value = property.KillSpace.ToString ( );

            var element2 = node.Element ("newWindow");
            
            if (element2 != null)
              element2.Value = property.NewWindow.ToString ( );

            var xElement3 = node.Element ("lineWrap");
            
            if (xElement3 != null)
              xElement3.Value = property.Wrap.ToString ( );

            var element3 = node.Element ("category");

            if (element3 != null)
              element3.Value = string.IsNullOrEmpty (property.Category) ? string.Empty : property.Category;

            var xElement4 = node.Element ("description");
           
            if (xElement4 != null)
              xElement4.Value = property.Description;

            var element4 = node.Element ("threadPriority");

            if (element4 != null)
              element4.Value = property.ThreadPriority.ToString ( );

            var xElement5 = node.Element ("refreshRate");

            if (xElement5 != null)
              xElement5.Value = property.RefreshRate.ToString ( );

            List<string> filterID = node.Elements ("filter").Select (filter =>
                                                                     {
                                                                       var element5 = filter.Element ("id");
                                                                       return element5 != null ? element5.Value : null;
                                                                     }).ToList ( );

            // TODO NOT nice!

            foreach (string id in filterID)
            {
              node.Elements ("filter").Where (x =>
                                              {
                                                var xElement = x.Element ("id");
                                                return xElement != null && xElement.Value == id;
                                              }).Remove ( );
            }

            fmDoc.Save (@fmFile, SaveOptions.None);

            foreach (FilterData item in property.ListOfFilter)
            {
              node.Add (AddFilterToDoc (item));
            }
          }
        }
        //////////////////////////////////////////////

        fmDoc.Save (@fmFile, SaveOptions.None);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("UpdateNode exception: {0}", ex));
      }
    }

    /// <summary>
    /// Remove node from FileManager
    /// </summary>
    /// <param name="property">FileManagerData property</param>
    public bool RemoveNode (FileManagerData property)
    {
      try
      {
        if (fmDoc.Root != null)
          fmDoc.Root.Descendants ("file").Where (x =>
                                                 {
                                                   var xElement = x.Element ("id");
                                                   return xElement != null && xElement.Value == property.ID.ToString (CultureInfo.InvariantCulture);
                                                 }).Remove ( );

        fmDoc.Save (@fmFile, SaveOptions.None);

        return (true);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("RemoveNode exception: {0}", ex));

        return (false);
      }
    }

    private XElement AddNode (FileManagerData fmProperty)
    {
      try
      {
        XElement file = new XElement ("file",
              new XElement ("id", fmProperty.ID),
              new XElement ("fileName", fmProperty.FileName),
              new XElement ("description", fmProperty.Description),
              new XElement ("category", fmProperty.Category),
              new XElement ("threadPriority", fmProperty.ThreadPriority),
              new XElement ("newWindow", fmProperty.NewWindow),
              new XElement ("refreshRate", fmProperty.RefreshRate),
              new XElement ("timeStamp", fmProperty.Timestamp),
              new XElement ("killSpace", fmProperty.KillSpace),
              new XElement ("lineWrap", fmProperty.Wrap),
              new XElement ("fileEncoding", fmProperty.FileEncoding.HeaderName),
              new XElement ("font",
                new XElement ("name", fmProperty.FontType.Name),
                new XElement ("size", fmProperty.FontType.Size),
                new XElement ("bold", fmProperty.FontType.Bold),
                new XElement ("italic", fmProperty.FontType.Italic)));

        foreach (FilterData item in fmProperty.ListOfFilter)
        {
          file.Add (AddFilterToDoc (item));
        }
        return (file);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("AddNode exception: {0}", ex));
        return (null);
      }
    }

    #region HelperFunctions

    /// <summary>
    /// Sort list if file sort is FileAge or FileCreateTime
    /// </summary>
    public void SortListIfRequired ()
    {
      switch (SettingsHelper.TailSettings.DefaultFileSort)
      {
      case SettingsData.EFileSort.FileCreationTime:

        fmProperties.Sort (new LogFile.FileManagerDataFileCreationTimeComparer ( ));
        break;

      case SettingsData.EFileSort.Nothing:

        break;

      default:

        break;
      }
    }

    /// <summary>
    /// Get new category from XML file
    /// </summary>
    public void RefreshCategories ()
    {
      List<string> categories = (from x in fmDoc.Descendants ("file") select x.Element ("category").Value).ToList<string> ( );
      Category.Clear ( );

      foreach (string category in categories)
      {
        AddCategoryToDictionary (category, category);
      }
    }

    private void AddCategoryToDictionary (string key, string value)
    {
      try
      {
        if (!Category.Contains (key))
          Category.Add (key);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog (ErrorFlags.Error, "FileManagerStructure", string.Format ("AddCategoryToDictionary exception: {0}", ex));
      }
    }

    private string GetCategory (XElement root)
    {
      string category = root.Element ("category").Value;

      if (string.IsNullOrEmpty (category))
        return (null);
      else
        return (category);
    }

    private int GetId (string sId, bool isFile = true)
    {
      int id = -1;

      if (!int.TryParse (sId, out id))
        id = -1;

      if (isFile == true)
      {
        if (id > LastFileId)
          LastFileId = id;
      }
      else
      {
        if (id > LastFilterId)
          LastFilterId = id;
      }
      return (id);
    }

    private bool IsKillSpace (string sKillSpace)
    {
      bool killspace;

      if (!bool.TryParse (sKillSpace, out killspace))
        killspace = false;

      return (killspace);
    }

    private bool IsLineWrap (string sLineWrap)
    {
      bool lineWrap;

      if (!bool.TryParse (sLineWrap, out lineWrap))
        lineWrap = false;

      return (lineWrap);
    }

    private bool IsTimeStamp (string sTimeStamp)
    {
      bool timeStamp;

      if (!bool.TryParse (sTimeStamp, out timeStamp))
        timeStamp = false;

      return (timeStamp);
    }

    private bool IsNewWindow (string sNewWindow)
    {
      bool newWindow;

      if (!bool.TryParse (sNewWindow, out newWindow))
        newWindow = false;

      return (newWindow);
    }

    private Font GetFont (XElement root)
    {
      string name = root.Element ("name").Value;
      FontStyle fs = FontStyle.Regular;
      float size;

      if (!float.TryParse (root.Element ("size").Value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out size))
        size = 10f;

      bool bold;

      if (!bool.TryParse (root.Element ("bold").Value, out bold))
        bold = false;

      bool italic;

      if (!bool.TryParse (root.Element ("italic").Value, out italic))
        italic = false;

      fs |= bold ? FontStyle.Bold : FontStyle.Regular;
      fs |= italic ? FontStyle.Italic : FontStyle.Regular;

      return (new Font (name, size, fs));
    }

    private System.Threading.ThreadPriority GetThreadPriority (string sThreadPriority)
    {
      return (SettingsHelper.GetThreadPriority (sThreadPriority));
    }

    private SettingsData.ETailRefreshRate GetRefreshRate (string sRefreshRate)
    {
      return (SettingsHelper.GetRefreshRate (sRefreshRate));
    }

    private Encoding GetEncoding (string sEncode)
    {
      Encoding encoding = null;

      foreach (Encoding encode in LogFile.FileEncoding)
      {
        if (String.Compare(encode.HeaderName, sEncode, StringComparison.Ordinal) == 0)
        {
          encoding = encode;
          break;
        }
        else
          encoding = Encoding.UTF8;
      }
      return (encoding);
    }

    private XElement AddFilterToDoc (FilterData filter)
    {
      XElement docPart = new XElement ("filter",
            new XElement ("id", filter.ID),
            new XElement ("filterName", filter.Description),
            new XElement ("filterPattern", filter.Filter),
            new XElement ("font",
              new XElement ("name", filter.FilterFontType.Name),
              new XElement ("size", filter.FilterFontType.Size),
              new XElement ("bold", filter.FilterFontType.Bold),
              new XElement ("italic", filter.FilterFontType.Italic)));

      return (docPart);
    }

    private FilterData GetFilters (XElement root)
    {
      if (GetId (root.Element ("id").Value) == -1)
        return (null);

      FilterData filter = new FilterData ( )
      {
        ID = GetId (root.Element ("id").Value),
        Filter = root.Element ("filterPattern").Value,
        Description = root.Element ("filterName").Value,
        FilterFontType = GetFont (root.Element ("font"))
      };

      return (filter);
    }

    #endregion
  }
}
