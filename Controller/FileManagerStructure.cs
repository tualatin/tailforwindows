using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TailForWin.Data;
using TailForWin.Data.Enums;
using TailForWin.Utils;


namespace TailForWin.Controller
{
  public class FileManagerStructure : INotifyMaster
  {
    XDocument fmDoc;
    readonly string fmFile = string.Empty;
    List<FileManagerData> fmProperties;
    ObservableCollection<string> category = new ObservableCollection<string>();
    private const string XMLROOT = "fileManager";


    public FileManagerStructure(bool findHistory = false)
    {
      fmFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\FileManager.xml";

      if (findHistory)
        return;

      fmProperties = new List<FileManagerData>();
      LastFileId = -1;
      LastFilterId = -1;

      OpenFmDoc();
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
        OnPropertyChanged("Category");
      }
    }

    /// <summary>
    /// List of FileManager Properties
    /// </summary>
    public List<FileManagerData> FmProperties
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
    public void OpenFmDoc()
    {
      if (File.Exists(fmFile))
        ReadFmDoc();
    }

    /// <summary>
    /// Read FileManager file
    /// </summary>
    public void ReadFmDoc()
    {
      try
      {
        fmDoc = XDocument.Load(fmFile);

        if (fmDoc.Root != null)
        {
          foreach (XElement xe in fmDoc.Root.Descendants("file"))
          {
            string cate = GetCategory(xe);

            if (cate != null)
              AddCategoryToDictionary(cate);

            var xElement = xe.Element("id");
            if (xElement == null)
              continue;

            var element = xe.Element("fileName");
            if (element == null)
              continue;

            var xElement1 = xe.Element("killSpace");
            var element1 = xe.Element("lineWrap");

            var xElement2 = xe.Element("description");
            if (xElement2 == null)
              continue;

            var element2 = xe.Element("timeStamp");

            var xElement3 = xe.Element("threadPriority");
            if (xElement3 == null)
              continue;

            var element3 = xe.Element("refreshRate");
            if (element3 == null)
              continue;

            var xElement4 = xe.Element("newWindow");

            var element4 = xe.Element("fileEncoding");
            if (element4 == null)
              continue;

            var filterElement = xe.Element("filters");
            if (filterElement == null)
            {
              filterElement = new XElement("filters");
              filterElement.Value = "false";
            }

            FileManagerData item = new FileManagerData
            {
              ID = GetId(xElement.Value),
              FileName = element.Value,
              FontType = GetFont(xe.Element("font")),
              KillSpace = xElement1 != null && IsKillSpace(xElement1.Value),
              Wrap = element1 != null && IsLineWrap(element1.Value),
              Category = cate,
              Description = xElement2.Value,
              Timestamp = element2 != null && IsTimeStamp(element2.Value),
              ThreadPriority = GetThreadPriority(xElement3.Value),
              RefreshRate = GetRefreshRate(element3.Value),
              NewWindow = xElement4 != null && IsNewWindow(xElement4.Value),
              FileEncoding = GetEncoding(element4.Value),
              FilterState = filterElement != null && GetFilterState(filterElement.Value)
            };

            foreach (FilterData data in xe.Elements("filter").Select(GetFilters).Where(data => data != null))
            {
              item.ListOfFilter.Add(data);
            }

            fmProperties.Add(item);
          }
        }

        SortListIfRequired();
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    /// <summary>
    /// Save FileManager file
    /// </summary>
    public void SaveFmDoc()
    {
      if (!File.Exists(fmFile))
        fmDoc = new XDocument(new XElement(XMLROOT));

      fmDoc.Save(@fmFile, SaveOptions.None);
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
    public void ReadFindHistory(ref ObservableDictionary<string, string> words)
    {
      try
      {
        if (!File.Exists(fmFile))
          return;

        fmDoc = XDocument.Load(fmFile);

        if (fmDoc.Root == null)
          return;

        XElement findHistoryRoot = fmDoc.Root.Element("FindHistory");

        if (findHistoryRoot == null)
          return;

        string wrapAround = findHistoryRoot.Attribute("wrap").Value;
        bool wrap;

        Wrap = bool.TryParse(wrapAround, out wrap) && wrap;

        foreach (XElement find in findHistoryRoot.Elements("Find"))
        {
          words.Add(find.Attribute("name").Value, find.Attribute("name").Value);
        }
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    /// <summary>
    /// Save find history attribute wrap
    /// </summary>
    public XElement SaveFindHistoryWrap()
    {
      if (!File.Exists(fmFile))
        fmDoc = new XDocument(new XElement(XMLROOT));

      try
      {
        if (fmDoc.Root != null)
        {
          XElement findHistoryRoot = fmDoc.Root.Element("FindHistory");

          if (findHistoryRoot != null)
            findHistoryRoot.Attribute("wrap").Value = Wrap.ToString();
          else
          {
            findHistoryRoot = new XElement("FindHistory");
            findHistoryRoot.Add(new XAttribute("wrap", Wrap.ToString()));
            fmDoc.Root.Add(findHistoryRoot);
          }

          fmDoc.Save(@fmFile, SaveOptions.None);

          return (findHistoryRoot);
        }
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
      return (null);
    }

    /// <summary>
    /// Save find history attribute name
    /// </summary>
    /// <param name="searchWord">Find what word</param>
    public void SaveFindHistoryName(string searchWord)
    {
      if (!File.Exists(fmFile))
        fmDoc = new XDocument(new XElement(XMLROOT));

      try
      {
        if (fmDoc.Root != null)
        {
          XElement findHistoryRoot = fmDoc.Root.Element("FindHistory") ?? SaveFindHistoryWrap();
          XElement findHistoryFind = new XElement("Find");
          findHistoryFind.Add(new XAttribute("name", searchWord));
          findHistoryRoot.Add(findHistoryFind);
        }

        fmDoc.Save(@fmFile, SaveOptions.None);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    #endregion

    /// <summary>
    /// Get a XML node by Id
    /// </summary>
    /// <param name="id">Reference of id</param>
    public FileManagerData GetNodeById(string id)
    {
      FileManagerData cmdParameterItem = null;

      if (!File.Exists(fmFile))
      {
        System.Windows.MessageBox.Show(System.Windows.Application.Current.FindResource("FileNotFound") as string, string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (null);
      }

      if (fmProperties.Count == 0)
      {
        System.Windows.MessageBox.Show(System.Windows.Application.Current.FindResource("NoContentFound") as string, string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (null);
      }

      int iid;

      if (!int.TryParse(id, out iid))
        iid = -1;
      if (iid != -1)
        cmdParameterItem = fmProperties.Find(o => o.ID == iid);

      return (cmdParameterItem);
    }

    /// <summary>
    /// Add new node to FileManager
    /// </summary>
    /// <param name="property">FileManagerData property</param>
    public void AddNewNode(FileManagerData property)
    {
      if (!File.Exists(fmFile))
        fmDoc = new XDocument(new XElement(XMLROOT));

      if (property.FileEncoding == null)
      {
        FileReader reader = new FileReader();

        if (!reader.OpenTailFileStream(property.FileName))
        {
          System.Windows.MessageBox.Show(string.Format("{0} '{1}'", System.Windows.Application.Current.FindResource("FileNotFound"), property.File), string.Format("{0} - {1}", LogFile.APPLICATION_CAPTION, LogFile.MSGBOX_ERROR), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
          return;
        }

        property.FileEncoding = reader.FileEncoding;
        reader.Dispose();
      }

      var xElement = fmDoc.Element(XMLROOT);

      if (xElement != null)
        xElement.Add(AddNode(property));

      fmDoc.Save(@fmFile, SaveOptions.None);
    }

    public void UpdateNode(FileManagerData property)
    {
      try
      {
        if (fmDoc.Root != null)
        {
          XElement node = (fmDoc.Root.Descendants("file").Where(x =>
                                                                {
                                                                  var element = x.Element("id");
                                                                  return (element != null && String.Compare(element.Value, property.ID.ToString(CultureInfo.InvariantCulture), false) == 0);
                                                                })).SingleOrDefault();
          if (node != null)
          {
            var element = node.Element("fileEncoding");

            if (element != null)
              element.Value = property.FileEncoding.HeaderName;

            var xElement1 = node.Element("fileName");

            if (xElement1 != null)
              xElement1.Value = property.FileName;

            var element1 = node.Element("timeStamp");

            if (element1 != null)
              element1.Value = property.Timestamp.ToString();

            var xElement2 = node.Element("killSpace");

            if (xElement2 != null)
              xElement2.Value = property.KillSpace.ToString();

            var element2 = node.Element("newWindow");

            if (element2 != null)
              element2.Value = property.NewWindow.ToString();

            var xElement3 = node.Element("lineWrap");

            if (xElement3 != null)
              xElement3.Value = property.Wrap.ToString();

            var element3 = node.Element("category");

            if (element3 != null)
              element3.Value = string.IsNullOrEmpty(property.Category) ? string.Empty : property.Category;

            var xElement4 = node.Element("description");

            if (xElement4 != null)
              xElement4.Value = property.Description;

            var element4 = node.Element("threadPriority");

            if (element4 != null)
              element4.Value = property.ThreadPriority.ToString();

            var xElement5 = node.Element("refreshRate");

            if (xElement5 != null)
              xElement5.Value = property.RefreshRate.ToString();

            var filterElement = node.Element("filters");

            if (filterElement != null)
              filterElement.Value = property.FilterState.ToString();
            else
            {
              filterElement = new XElement("filters");
              filterElement.Value = property.FilterState.ToString();
              node.Add(filterElement);
            }

            var xFont = node.Element("font");

            if (xFont != null)
            {
              var xFontFamily = xFont.Element("name");
              var xFontSize = xFont.Element("size");
              var xFontBold = xFont.Element("bold");
              var xFontItalic = xFont.Element("italic");

              xFontFamily.Value = property.FontType.Name;
              xFontSize.Value = property.FontType.Size.ToString();
              xFontBold.Value = property.FontType.Bold.ToString();
              xFontItalic.Value = property.FontType.Italic.ToString();
            }

            List<string> filterId = node.Elements("filter").Select(filter =>
                                                                   {
                                                                     var element5 = filter.Element("id");
                                                                     return element5 != null ? element5.Value : null;
                                                                   }).ToList();

            // TODO NOT nice!
            foreach (string id in filterId)
            {
              var id1 = id;
              node.Elements("filter").Where(x =>
                                            {
                                              var xElement = x.Element("id");
                                              return (xElement != null && String.Compare(xElement.Value, id1, false) == 0);
                                            }).Remove();
            }

            fmDoc.Save(@fmFile, SaveOptions.None);

            foreach (FilterData item in property.ListOfFilter)
            {
              node.Add(AddFilterToDoc(item));
            }
          }
        }
        //////////////////////////////////////////////

        fmDoc.Save(@fmFile, SaveOptions.None);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    /// <summary>
    /// Remove node from FileManager
    /// </summary>
    /// <param name="property">FileManagerData property</param>
    public bool RemoveNode(FileManagerData property)
    {
      try
      {
        if (fmDoc.Root != null)
          fmDoc.Root.Descendants("file").Where(x =>
                                               {
                                                 var xElement = x.Element("id");
                                                 return (xElement != null && String.Compare(xElement.Value, property.ID.ToString(CultureInfo.InvariantCulture), false) == 0);
                                               }).Remove();

        fmDoc.Save(@fmFile, SaveOptions.None);

        return (true);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1} exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));

        return (false);
      }
    }

    private XElement AddNode(FileManagerData fmProperty)
    {
      try
      {
        XElement file = new XElement("file",
              new XElement("id", fmProperty.ID),
              new XElement("fileName", fmProperty.FileName),
              new XElement("description", fmProperty.Description),
              new XElement("category", fmProperty.Category),
              new XElement("threadPriority", fmProperty.ThreadPriority),
              new XElement("newWindow", fmProperty.NewWindow),
              new XElement("refreshRate", fmProperty.RefreshRate),
              new XElement("timeStamp", fmProperty.Timestamp),
              new XElement("killSpace", fmProperty.KillSpace),
              new XElement("lineWrap", fmProperty.Wrap),
              new XElement("fileEncoding", fmProperty.FileEncoding.HeaderName),
              new XElement("filters", fmProperty.FilterState),
              new XElement("font",
                new XElement("name", fmProperty.FontType.Name),
                new XElement("size", fmProperty.FontType.Size),
                new XElement("bold", fmProperty.FontType.Bold),
                new XElement("italic", fmProperty.FontType.Italic)));

        foreach (FilterData item in fmProperty.ListOfFilter)
        {
          file.Add(AddFilterToDoc(item));
        }
        return (file);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
      return (null);
    }

    #region HelperFunctions

    /// <summary>
    /// Sort list if file sort is FileAge or FileCreateTime
    /// </summary>
    public void SortListIfRequired()
    {
      switch (SettingsHelper.TailSettings.DefaultFileSort)
      {
      case EFileSort.FileCreationTime:

      fmProperties.Sort(new LogFile.FileManagerDataFileCreationTimeComparer());
      break;

      case EFileSort.Nothing:

      fmProperties = fmProperties.OrderBy(o => o.File).ToList();
      break;
      }
    }

    /// <summary>
    /// Get new category from XML file
    /// </summary>
    public void RefreshCategories()
    {
      List<string> categories = (from x in fmDoc.Descendants("file")
                                 let xElement = x.Element("category")
                                 where xElement != null
                                 select xElement.Value).ToList<string>();
      Category.Clear();

      categories.ForEach(AddCategoryToDictionary);
    }

    private void AddCategoryToDictionary(string key)
    {
      try
      {
        if (!Category.Contains(key))
          Category.Add(key);
      }
      catch (Exception ex)
      {
        ErrorLog.WriteLog(ErrorFlags.Error, GetType().Name, string.Format("{1}, exception: {0}", ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
      }
    }

    private static string GetCategory(XContainer root)
    {
      var xElement = root.Element("category");

      if (xElement == null)
        return (null);

      string category = xElement.Value;

      return (string.IsNullOrEmpty(category) ? (null) : (category));
    }

    private int GetId(string sId, bool isFile = true)
    {
      int id;

      if (!int.TryParse(sId, out id))
        id = -1;

      if (isFile)
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

    private static bool GetFilterState(string sFilters)
    {
      bool filters;

      if (!bool.TryParse(sFilters, out filters))
        filters = false;

      return (filters);
    }

    private static bool IsKillSpace(string sKillSpace)
    {
      bool killspace;

      if (!bool.TryParse(sKillSpace, out killspace))
        killspace = false;

      return (killspace);
    }

    private static bool IsLineWrap(string sLineWrap)
    {
      bool lineWrap;

      if (!bool.TryParse(sLineWrap, out lineWrap))
        lineWrap = false;

      return (lineWrap);
    }

    private static bool IsTimeStamp(string sTimeStamp)
    {
      bool timeStamp;

      if (!bool.TryParse(sTimeStamp, out timeStamp))
        timeStamp = false;

      return (timeStamp);
    }

    private static bool IsNewWindow(string sNewWindow)
    {
      bool newWindow;

      if (!bool.TryParse(sNewWindow, out newWindow))
        newWindow = false;

      return (newWindow);
    }

    private static Font GetFont(XContainer root)
    {
      var xElement = root.Element("name");

      if (xElement == null)
        return (null);

      string name = xElement.Value;
      FontStyle fs = FontStyle.Regular;
      float size = 10;

      var element = root.Element("size");

      if (element != null && !float.TryParse(element.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out size))
        size = 10f;

      bool bold = false;

      var xElement1 = root.Element("bold");

      if (xElement1 != null && !bool.TryParse(xElement1.Value, out bold))
        bold = false;

      bool italic = false;

      var element1 = root.Element("italic");

      if (element1 != null && !bool.TryParse(element1.Value, out italic))
        italic = false;

      fs |= bold ? FontStyle.Bold : FontStyle.Regular;
      fs |= italic ? FontStyle.Italic : FontStyle.Regular;

      return (new Font(name, size, fs));
    }

    private static System.Threading.ThreadPriority GetThreadPriority(string sThreadPriority)
    {
      return (SettingsHelper.GetThreadPriority(sThreadPriority));
    }

    private static ETailRefreshRate GetRefreshRate(string sRefreshRate)
    {
      return (SettingsHelper.GetRefreshRate(sRefreshRate));
    }

    private static Encoding GetEncoding(string sEncode)
    {
      Encoding encoding = null;

      foreach (Encoding encode in LogFile.FileEncoding)
      {
        if (String.Compare(encode.HeaderName, sEncode, StringComparison.Ordinal) == 0)
        {
          encoding = encode;
          break;
        }
        encoding = Encoding.UTF8;
      }
      return (encoding);
    }

    private static XElement AddFilterToDoc(FilterData filter)
    {
      XElement docPart = new XElement("filter",
            new XElement("id", filter.Id),
            new XElement("filterName", filter.Description),
            new XElement("filterPattern", filter.Filter),
            new XElement("font",
              new XElement("name", filter.FilterFontType.Name),
              new XElement("size", filter.FilterFontType.Size),
              new XElement("bold", filter.FilterFontType.Bold),
              new XElement("italic", filter.FilterFontType.Italic)));

      return (docPart);
    }

    private FilterData GetFilters(XElement root)
    {
      var xElement = root.Element("id");

      if (xElement != null && GetId(xElement.Value) == -1)
        return (null);

      var element = root.Element("id");

      if (element == null)
        return (null);

      var xElement1 = root.Element("filterPattern");

      if (xElement1 == null)
        return (null);

      var element1 = root.Element("filterName");

      if (element1 == null)
        return (null);

      FilterData filter = new FilterData
      {
        Id = GetId(element.Value),
        Filter = xElement1.Value,
        Description = element1.Value,
        FilterFontType = GetFont(root.Element("font"))
      };
      return (filter);
    }

    #endregion
  }
}
