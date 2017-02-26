using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.Base;
using Org.Vs.TailForWin.Data.Enums;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Controller
{
  /// <summary>
  /// FileManager structure
  /// </summary>
  public class FileManagerStructure : INotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FileManagerStructure));

    XDocument fmDoc;
    readonly string fmFile;
    List<FileManagerData> fmProperties;
    ObservableCollection<string> category = new ObservableCollection<string>();
    private const string XMLROOT = "fileManager";


    /// <summary>
    /// Constructor
    /// </summary>
    public FileManagerStructure()
    {
      fmFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\FileManager.xml";
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
      if(File.Exists(fmFile))
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

        if(fmDoc.Root != null)
        {
          foreach(XElement xe in fmDoc.Root.Descendants("file"))
          {
            string cate = GetCategory(xe);

            if(cate != null)
              AddCategoryToDictionary(cate);

            var xElement = xe.Element("id");

            if(xElement == null)
              continue;

            var element = xe.Element("fileName");

            if(element == null)
              continue;

            var xElement1 = xe.Element("killSpace");
            var element1 = xe.Element("lineWrap");
            var xElement2 = xe.Element("description");

            if(xElement2 == null)
              continue;

            var element2 = xe.Element("timeStamp");
            var xElement3 = xe.Element("threadPriority");

            if(xElement3 == null)
              continue;

            var element3 = xe.Element("refreshRate");

            if(element3 == null)
              continue;

            var xElement4 = xe.Element("newWindow");
            var element4 = xe.Element("fileEncoding");

            if(element4 == null)
              continue;

            var filterElement = xe.Element("useFilters");

            if(filterElement == null)
            {
              filterElement = new XElement("useFilters")
              {
                Value = "false"
              };
            }

            FileManagerData item = new FileManagerData
            {
              ID = GetId(xElement.Value),
              FileName = element.Value,
              FontType = GetFont(xe.Element("font")),
              KillSpace = xElement1 != null && StringToBool(xElement1.Value),
              Wrap = element1 != null && StringToBool(element1.Value),
              Category = cate,
              Description = xElement2.Value,
              Timestamp = element2 != null && StringToBool(element2.Value),
              ThreadPriority = GetThreadPriority(xElement3.Value),
              RefreshRate = GetRefreshRate(element3.Value),
              NewWindow = xElement4 != null && StringToBool(xElement4.Value),
              FileEncoding = GetEncoding(element4.Value),
              FilterState = filterElement != null && StringToBool(filterElement.Value),
            };

            #region Search pattern

            var searchPatternElement = xe.Element("searchPattern");

            if(searchPatternElement != null)
            {
              item.SearchPattern = GetSearchPattern(searchPatternElement);

              var partsElement = searchPatternElement.Element("parts");

              if(partsElement != null)
              {
                foreach(Part part in partsElement.Elements("part").Select(GetPart).Where(part => part != null))
                {
                  item.SearchPattern.PatternParts.Add(part);
                }

              }
            }

            #endregion

            #region Filters

            var filtersElement = xe.Element("filters");

            if(filtersElement != null)
            {
              foreach(FilterData data in filtersElement.Elements("filter").Select(GetFilter).Where(data => data != null))
              {
                item.ListOfFilter.Add(data);
              }
            }

            #endregion

            fmProperties.Add(item);
          }
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Save FileManager file
    /// </summary>
    public void SaveFmDoc()
    {
      if(!File.Exists(fmFile))
        fmDoc = new XDocument(new XElement(XMLROOT));

      fmDoc.Save(@fmFile, SaveOptions.None);
    }

    /// <summary>
    /// Get a XML node by Id
    /// </summary>
    /// <param name="id">Reference of id</param>
    public FileManagerData GetNodeById(string id)
    {
      FileManagerData cmdParameterItem = null;

      if(!File.Exists(fmFile))
      {
        System.Windows.MessageBox.Show(System.Windows.Application.Current.FindResource("FileNotFound") as string,
          $"{LogFile.APPLICATION_CAPTION} - {LogFile.MSGBOX_ERROR}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (null);
      }

      if(fmProperties.Count == 0)
      {
        System.Windows.MessageBox.Show(System.Windows.Application.Current.FindResource("NoContentFound") as string,
          $"{LogFile.APPLICATION_CAPTION} - {LogFile.MSGBOX_ERROR}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (null);
      }

      if(!int.TryParse(id, out int iid))
        iid = -1;

      if(iid != -1)
        cmdParameterItem = fmProperties.Find(o => o.ID == iid);

      return (cmdParameterItem);
    }

    /// <summary>
    /// Add new node to FileManager
    /// </summary>
    /// <param name="property">FileManagerData property</param>
    public void AddNewNode(FileManagerData property)
    {
      if(!File.Exists(fmFile))
        fmDoc = new XDocument(new XElement(XMLROOT));

      if(property.FileEncoding == null)
      {
        FileReader reader = new FileReader();

        if(!reader.OpenTailFileStream(property.FileName))
        {
          System.Windows.MessageBox.Show($"{System.Windows.Application.Current.FindResource("FileNotFound")} '{property.File}'",
            $"{LogFile.APPLICATION_CAPTION} - {LogFile.MSGBOX_ERROR}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
          return;
        }

        property.FileEncoding = reader.FileEncoding;
        reader.Dispose();
      }

      var xElement = fmDoc.Element(XMLROOT);

      if(xElement != null)
      {
        var newNode = AddNode(property);

        if(newNode == null)
        {
          System.Windows.MessageBox.Show("Can not create new FileManager entry. Internal error, please try it again.", $"{LogFile.APPLICATION_CAPTION} - {LogFile.MSGBOX_ERROR}",
            System.Windows.MessageBoxButton.OK);
          return;
        }

        xElement.Add(newNode);
      }

      fmDoc.Save(@fmFile, SaveOptions.None);
    }

    /// <summary>
    /// Update a current XML node
    /// </summary>
    /// <param name="property">FileManagerData property</param>
    public void UpdateNode(FileManagerData property)
    {
      try
      {
        if(fmDoc.Root != null)
        {
          XElement node = (fmDoc.Root.Descendants("file").Where(x =>
                                                                {
                                                                  var element = x.Element("id");
                                                                  return (element != null && String.Compare(element.Value, property.ID.ToString(CultureInfo.InvariantCulture), false) == 0);
                                                                })).SingleOrDefault();
          if(node != null)
          {
            var element = node.Element("fileEncoding");

            if(element != null)
              element.Value = property.FileEncoding.HeaderName;

            var xElement1 = node.Element("fileName");

            if(xElement1 != null)
              xElement1.Value = property.FileName;

            var element1 = node.Element("timeStamp");

            if(element1 != null)
              element1.Value = property.Timestamp.ToString();

            var xElement2 = node.Element("killSpace");

            if(xElement2 != null)
              xElement2.Value = property.KillSpace.ToString();

            var element2 = node.Element("newWindow");

            if(element2 != null)
              element2.Value = property.NewWindow.ToString();

            var xElement3 = node.Element("lineWrap");

            if(xElement3 != null)
              xElement3.Value = property.Wrap.ToString();

            var element3 = node.Element("category");

            if(element3 != null)
              element3.Value = string.IsNullOrEmpty(property.Category) ? string.Empty : property.Category;

            var xElement4 = node.Element("description");

            if(xElement4 != null)
              xElement4.Value = property.Description;

            var element4 = node.Element("threadPriority");

            if(element4 != null)
              element4.Value = property.ThreadPriority.ToString();

            var xElement5 = node.Element("refreshRate");

            if(xElement5 != null)
              xElement5.Value = property.RefreshRate.ToString();

            var filterElement = node.Element("useFilters");

            if(filterElement != null)
            {
              filterElement.Value = property.FilterState.ToString();
            }
            else
            {
              filterElement = new XElement("useFilters")
              {
                Value = property.FilterState.ToString()
              };
              node.Add(filterElement);
            }

            #region Font

            var xFont = node.Element("font");

            if(xFont != null)
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

            #endregion

            #region Search pattern

            var searchPattern = node.Element("searchPattern");

            if(searchPattern != null)
            {
              var isRegexElement = searchPattern.Element("isRegex");
              var patternElement = searchPattern.Element("pattern");

              isRegexElement.Value = property.SearchPattern.IsRegex.ToString();
              patternElement.Value = property.SearchPattern.Pattern;

              var parts = searchPattern.Element("parts");

              if(parts == null)
              {
                searchPattern.Add(AddPartsToSearchPattern(property.SearchPattern.PatternParts));
              }
              else
              {
                parts.Remove();
                searchPattern.Add(AddPartsToSearchPattern(property.SearchPattern.PatternParts));
              }
            }
            else
            {
              node.Add(AddSearchPatternToDoc(property.SearchPattern));

              searchPattern = node.Element("searchPattern");
              searchPattern.Add(AddPartsToSearchPattern(property.SearchPattern.PatternParts));
            }

            #endregion

            #region Filters

            var filtersElement = node.Element("filters");

            if(filtersElement != null)
            {
              filtersElement.Remove();
              node.Add(AddFiltersToRoot(property.ListOfFilter));
            }
            else
            {
              node.Add(AddFiltersToRoot(property.ListOfFilter));
            }

            #endregion
          }
        }

        fmDoc.Save(@fmFile, SaveOptions.None);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
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
        if(fmDoc.Root != null)
          fmDoc.Root.Descendants("file").Where(x =>
                                               {
                                                 var xElement = x.Element("id");
                                                 return (xElement != null && String.Compare(xElement.Value, property.ID.ToString(CultureInfo.InvariantCulture), false) == 0);
                                               }).Remove();
        fmDoc.Save(@fmFile, SaveOptions.None);

        return (true);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        return (false);
      }
    }

    private XElement AddNode(FileManagerData fmProperty)
    {
      if(fmProperty == null)
        return (null);

      try
      {
        if(fmProperty.FontType == null)
          fmProperty.FontType = new Font("Segoe UI", 12f, FontStyle.Regular);

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
              new XElement("useFilters", fmProperty.FilterState),
              new XElement("font",
                new XElement("name", fmProperty.FontType.Name),
                new XElement("size", fmProperty.FontType.Size),
                new XElement("bold", fmProperty.FontType.Bold),
                new XElement("italic", fmProperty.FontType.Italic)));

        var searchPattern = new XElement("searchPattern",
                              new XElement("isRegex", fmProperty.SearchPattern.IsRegex),
                              new XElement("pattern", fmProperty.SearchPattern.Pattern));
        var parts = new XElement("parts");

        foreach(Part item in fmProperty.SearchPattern.PatternParts)
        {
          parts.Add(AddPartToDoc(item));
        }

        searchPattern.Add(parts);
        file.Add(searchPattern);

        var filters = new XElement("filters");

        foreach(FilterData item in fmProperty.ListOfFilter)
        {
          filters.Add(AddFilterToDoc(item));
        }

        file.Add(filters);

        return (file);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return (null);
    }

    #region HelperFunctions

    /// <summary>
    /// Sort list if file sort is FileAge or FileCreateTime
    /// </summary>
    public void SortListIfRequired()
    {
      try
      {
        switch(SettingsHelper.TailSettings.DefaultFileSort)
        {
        case EFileSort.FileCreationTime:

          fmProperties.Sort();
          break;

        case EFileSort.Nothing:

          fmProperties = fmProperties.OrderBy(o => o.File).ToList();
          break;
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
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
                                 select xElement.Value).ToList();
      Category.Clear();
      categories.ForEach(AddCategoryToDictionary);
    }

    private void AddCategoryToDictionary(string key)
    {
      try
      {
        if(!Category.Contains(key))
          Category.Add(key);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private static string GetCategory(XContainer root)
    {
      var xElement = root.Element("category");

      if(xElement == null)
        return (null);

      string category = xElement.Value;

      return (string.IsNullOrEmpty(category) ? (null) : (category));
    }

    private int GetId(string sId, bool isFile = true)
    {
      if(!int.TryParse(sId, out int id))
        id = -1;

      if(isFile)
      {
        if(id > LastFileId)
          LastFileId = id;
      }
      else
      {
        if(id > LastFilterId)
          LastFilterId = id;
      }
      return (id);
    }

    private bool StringToBool(string boolean)
    {
      if(!bool.TryParse(boolean, out bool outValue))
        outValue = false;

      return (outValue);
    }

    private int StringToInt(string integer)
    {
      if(!int.TryParse(integer, out int outValue))
        outValue = -1;

      return (outValue);
    }

    private static Font GetFont(XContainer root)
    {
      var xElement = root.Element("name");

      if(xElement == null)
        return (null);

      string name = xElement.Value;
      FontStyle fs = FontStyle.Regular;
      float size = 10;

      var element = root.Element("size");

      if(element != null && !float.TryParse(element.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out size))
        size = 10f;

      bool bold = false;

      var xElement1 = root.Element("bold");

      if(xElement1 != null && !bool.TryParse(xElement1.Value, out bold))
        bold = false;

      bool italic = false;

      var element1 = root.Element("italic");

      if(element1 != null && !bool.TryParse(element1.Value, out italic))
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

      foreach(Encoding encode in LogFile.FileEncoding)
      {
        if(String.Compare(encode.HeaderName, sEncode, StringComparison.Ordinal) == 0)
        {
          encoding = encode;
          break;
        }
        encoding = Encoding.UTF8;
      }
      return (encoding);
    }

    private XElement AddFilterToDoc(FilterData filter)
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

    private FilterData GetFilter(XElement root)
    {
      var xElement = root.Element("id");

      if(xElement != null && GetId(xElement.Value) == -1)
        return (null);

      var element = root.Element("id");

      if(element == null)
        return (null);

      var xElement1 = root.Element("filterPattern");

      if(xElement1 == null)
        return (null);

      var element1 = root.Element("filterName");

      if(element1 == null)
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

    private XElement AddSearchPatternToDoc(SearchPatter pattern)
    {
      XElement docPart = new XElement("searchPattern",
        new XElement("isRegex", pattern.IsRegex),
        new XElement("pattern", pattern.Pattern));

      return (docPart);
    }

    private SearchPatter GetSearchPattern(XElement root)
    {
      var isRegexElement = root.Element("isRegex");
      var patternElement = root.Element("pattern");

      SearchPatter searchPattern = new SearchPatter
      {
        IsRegex = StringToBool(isRegexElement.Value),
        Pattern = patternElement == null ? string.Empty : patternElement.Value
      };
      return (searchPattern);
    }

    private XElement AddPartToDoc(Part part)
    {
      XElement docPart = new XElement("part",
        new XElement("index", part.Index),
        new XElement("begin", part.Begin),
        new XElement("end", part.End));

      return (docPart);
    }

    private Part GetPart(XElement root)
    {
      var beginPart = root.Element("begin");
      var endPart = root.Element("end");
      var index = root.Element("index");

      Part part = new Part
      {
        Index = StringToInt(index.Value),
        Begin = StringToInt(beginPart.Value),
        End = StringToInt(endPart.Value)
      };
      return (part);
    }

    private XElement AddPartsToSearchPattern(List<Part> parts)
    {
      var partsElement = new XElement("parts");

      foreach(var item in parts)
      {
        partsElement.Add(AddPartToDoc(item));
      }
      return (partsElement);
    }

    private XElement AddFiltersToRoot(ObservableCollection<FilterData> filters)
    {
      var filtersElement = new XElement("filters");

      foreach(var item in filters)
      {
        filtersElement.Add(AddFilterToDoc(item));
      }
      return (filtersElement);
    }


    #endregion
  }
}
