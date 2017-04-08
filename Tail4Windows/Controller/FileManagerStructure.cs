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
using Org.Vs.TailForWin.Data.XmlNames;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Controller
{
  /// <summary>
  /// FileManager structure
  /// </summary>
  public class FileManagerStructure : NotifyMaster
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
          foreach(XElement xe in fmDoc.Root.Descendants(XmlStructure.File))
          {
            string cate = GetCategory(xe);

            if(cate != null)
              AddCategoryToDictionary(cate);

            var xElement = xe.Element(XmlStructure.Id);

            if(xElement == null)
              continue;

            var element = xe.Element(XmlStructure.FileName);

            if(element == null)
              continue;

            var xElement1 = xe.Element(XmlStructure.RemoveSpace);
            var element1 = xe.Element(XmlStructure.LineWrap);
            var xElement2 = xe.Element(XmlStructure.Description);
            var element2 = xe.Element(XmlStructure.TimeStamp);
            var xElement3 = xe.Element(XmlStructure.ThreadPriority);

            if(xElement3 == null)
              continue;

            var element3 = xe.Element(XmlStructure.RefreshRate);

            if(element3 == null)
              continue;

            var xElement4 = xe.Element(XmlStructure.NewWindow);
            var element4 = xe.Element(XmlStructure.FileEncoding);
            var filterElement = xe.Element(XmlStructure.UseFilters);

            if(filterElement == null)
            {
              filterElement = new XElement(XmlStructure.UseFilters)
              {
                Value = "false"
              };
            }

            var usePatternElement = xe.Element(XmlStructure.UsePattern);

            if(usePatternElement == null)
            {
              usePatternElement = new XElement(XmlStructure.UsePattern)
              {
                Value = "false"
              };
            }

            var useSmartWatchElement = xe.Element(XmlStructure.UseSmartWatch);

            if(useSmartWatchElement == null)
            {
              useSmartWatchElement = new XElement(XmlStructure.UseSmartWatch)
              {
                Value = "false"
              };
            }

            FileManagerData item = new FileManagerData
            {
              ID = GetId(xElement.Value),
              OldId = GetOldId(xElement.Value),
              FileName = element.Value,
              OriginalFileName = element.Value,
              FontType = GetFont(xe.Element(XmlStructure.Font)),
              KillSpace = xElement1 != null && StringToBool(xElement1.Value),
              Wrap = element1 != null && StringToBool(element1.Value),
              Category = cate,
              Description = xElement2.Value,
              Timestamp = element2 != null && StringToBool(element2.Value),
              ThreadPriority = GetThreadPriority(xElement3.Value),
              RefreshRate = GetRefreshRate(element3.Value),
              NewWindow = xElement4 != null && StringToBool(xElement4.Value),
              FileEncoding = GetEncoding(element4.Value),
              FilterState = StringToBool(filterElement.Value),
              UsePattern = StringToBool(usePatternElement.Value),
              SmartWatch = StringToBool(useSmartWatchElement.Value)
            };

            #region Search pattern

            var searchPatternElement = xe.Element(XmlStructure.SearchPattern);

            if(searchPatternElement != null)
            {
              Pattern pattern = GetPattern(searchPatternElement);

              if(pattern != null)
              {
                item.PatternString = pattern.PatternString;
                item.IsRegex = pattern.IsRegex;
              }
            }

            #endregion

            #region Filters

            var filtersElement = xe.Element(XmlStructure.Filters);

            if(filtersElement != null)
            {
              foreach(FilterData data in filtersElement.Elements(XmlStructure.Filter).Select(GetFilter).Where(data => data != null))
              {
                item.ListOfFilter.Add(data);
              }
            }

            #endregion

            if(item.OldId >= 0)
              UpdateNode(item);

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
        System.Windows.MessageBox.Show(System.Windows.Application.Current.FindResource("FileNotFound").ToString(),
          $"{LogFile.APPLICATION_CAPTION} - {LogFile.MSGBOX_ERROR}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (null);
      }

      if(fmProperties.Count == 0)
      {
        System.Windows.MessageBox.Show(System.Windows.Application.Current.FindResource("NoContentFound").ToString(),
          $"{LogFile.APPLICATION_CAPTION} - {LogFile.MSGBOX_ERROR}", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

        return (null);
      }

      if(!Guid.TryParse(id, out Guid iid))
        iid = Guid.Empty;

      if(iid != Guid.Empty)
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
      if(fmDoc == null)
        return;

      try
      {
        if(fmDoc.Root != null)
        {
          XElement node = (fmDoc.Root.Descendants(XmlStructure.File).Where(x =>
                   {
                     var element = x.Element(XmlStructure.Id);
                     return (element != null && String.Compare(element.Value, property.ID.ToString(), false) == 0);
                   })).SingleOrDefault();

          if(node == null)
          {
            // migrate old Id to new Guid
            if(property.OldId >= 0)
            {
              node = (fmDoc.Root.Descendants(XmlStructure.File).Where(x =>
              {
                var element = x.Element(XmlStructure.Id);
                return (element != null && String.Compare(element.Value, property.OldId.ToString(CultureInfo.InvariantCulture), false) == 0);
              })).SingleOrDefault();

              if(node != null)
              {
                var xmlId = node.Element(XmlStructure.Id);
                xmlId.Value = property.ID.ToString();
              }
            }
          }

          if(node != null)
          {
            var element = node.Element(XmlStructure.FileEncoding);

            if(element != null)
              element.Value = property.FileEncoding.HeaderName;

            var xElement1 = node.Element(XmlStructure.FileName);

            if(xElement1 != null)
              xElement1.Value = property.OriginalFileName;

            var element1 = node.Element(XmlStructure.TimeStamp);

            if(element1 != null)
              element1.Value = property.Timestamp.ToString();

            var xElement2 = node.Element(XmlStructure.RemoveSpace);

            if(xElement2 != null)
              xElement2.Value = property.KillSpace.ToString();

            var element2 = node.Element(XmlStructure.NewWindow);

            if(element2 != null)
              element2.Value = property.NewWindow.ToString();

            var xElement3 = node.Element(XmlStructure.LineWrap);

            if(xElement3 != null)
              xElement3.Value = property.Wrap.ToString();

            var element3 = node.Element(XmlStructure.Category);

            if(element3 != null)
              element3.Value = string.IsNullOrEmpty(property.Category) ? string.Empty : property.Category;

            var xElement4 = node.Element(XmlStructure.Description);

            if(xElement4 != null)
              xElement4.Value = property.Description;

            var element4 = node.Element(XmlStructure.ThreadPriority);

            if(element4 != null)
              element4.Value = property.ThreadPriority.ToString();

            var xElement5 = node.Element(XmlStructure.RefreshRate);

            if(xElement5 != null)
              xElement5.Value = property.RefreshRate.ToString();

            var filterElement = node.Element(XmlStructure.UseFilters);

            if(filterElement != null)
            {
              filterElement.Value = property.FilterState.ToString();
            }
            else
            {
              filterElement = new XElement(XmlStructure.UseFilters)
              {
                Value = property.FilterState.ToString()
              };
              node.Add(filterElement);
            }

            var useSmartWatchElement = node.Element(XmlStructure.UseSmartWatch);

            if(useSmartWatchElement != null)
            {
              useSmartWatchElement.Value = property.SmartWatch.ToString();
            }
            else
            {
              useSmartWatchElement = new XElement(XmlStructure.UseSmartWatch)
              {
                Value = property.SmartWatch.ToString()
              };
              node.Add(useSmartWatchElement);
            }

            #region Font

            var xFont = node.Element(XmlStructure.Font);

            if(xFont != null)
            {
              var xFontFamily = xFont.Element(XmlStructure.Name);
              var xFontSize = xFont.Element(XmlStructure.Size);
              var xFontBold = xFont.Element(XmlStructure.Bold);
              var xFontItalic = xFont.Element(XmlStructure.Italic);

              xFontFamily.Value = property.FontType.Name;
              xFontSize.Value = property.FontType.Size.ToString();
              xFontBold.Value = property.FontType.Bold.ToString();
              xFontItalic.Value = property.FontType.Italic.ToString();
            }

            #endregion

            #region Search pattern

            var usePattern = node.Element(XmlStructure.UsePattern);

            if(usePattern != null)
            {
              usePattern.Value = property.UsePattern.ToString();
            }
            else
            {
              usePattern = new XElement(XmlStructure.UsePattern)
              {
                Value = property.UsePattern.ToString()
              };
              node.Add(usePattern);
            }

            var searchPattern = node.Element(XmlStructure.SearchPattern);

            if(searchPattern != null)
              searchPattern.Remove();

            Pattern pattern = new Pattern
            {
              IsRegex = property.IsRegex,
              PatternString = property.PatternString
            };
            node.Add(AddSearchPatternToDoc(pattern));

            #endregion

            #region Filters

            var filtersElement = node.Element(XmlStructure.Filters);

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
          fmDoc.Root.Descendants(XmlStructure.File).Where(x =>
                                               {
                                                 var xElement = x.Element(XmlStructure.Id);
                                                 return (xElement != null && String.Compare(xElement.Value, property.ID.ToString(), false) == 0);
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

        XElement file = new XElement(XmlStructure.File,
              new XElement(XmlStructure.Id, fmProperty.ID),
              new XElement(XmlStructure.FileName, fmProperty.FileName),
              new XElement(XmlStructure.Description, fmProperty.Description),
              new XElement(XmlStructure.Category, fmProperty.Category),
              new XElement(XmlStructure.ThreadPriority, fmProperty.ThreadPriority),
              new XElement(XmlStructure.NewWindow, fmProperty.NewWindow),
              new XElement(XmlStructure.RefreshRate, fmProperty.RefreshRate),
              new XElement(XmlStructure.TimeStamp, fmProperty.Timestamp),
              new XElement(XmlStructure.RemoveSpace, fmProperty.KillSpace),
              new XElement(XmlStructure.LineWrap, fmProperty.Wrap),
              new XElement(XmlStructure.FileEncoding, fmProperty.FileEncoding.HeaderName),
              new XElement(XmlStructure.UseFilters, fmProperty.FilterState),
              new XElement(XmlStructure.UsePattern, fmProperty.UsePattern),
              new XElement(XmlStructure.Font,
                new XElement(XmlStructure.Name, fmProperty.FontType.Name),
                new XElement(XmlStructure.Size, fmProperty.FontType.Size),
                new XElement(XmlStructure.Bold, fmProperty.FontType.Bold),
                new XElement(XmlStructure.Italic, fmProperty.FontType.Italic)));

        Pattern pattern = new Pattern
        {
          IsRegex = fmProperty.IsRegex,
          PatternString = fmProperty.PatternString
        };
        file.Add(AddSearchPatternToDoc(pattern));

        var filters = new XElement(XmlStructure.Filters);

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
      List<string> categories = (from x in fmDoc.Descendants(XmlStructure.File)
                                 let xElement = x.Element(XmlStructure.Category)
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
      var xElement = root.Element(XmlStructure.Category);

      if(xElement == null)
        return (null);

      string category = xElement.Value;

      return (string.IsNullOrEmpty(category) ? (null) : (category));
    }

    private Guid GetId(string sId, bool isFile = true)
    {
      if(!Guid.TryParse(sId, out Guid id))
        id = Guid.NewGuid();

      return (id);
    }

    private int GetOldId(string sId, bool isFile = true)
    {
      if(!int.TryParse(sId, out int id))
        id = -1;

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
      var xElement = root.Element(XmlStructure.Name);

      if(xElement == null)
        return (null);

      string name = xElement.Value;
      FontStyle fs = FontStyle.Regular;
      float size = 10;

      var element = root.Element(XmlStructure.Size);

      if(element != null && !float.TryParse(element.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out size))
        size = 10f;

      bool bold = false;

      var xElement1 = root.Element(XmlStructure.Bold);

      if(xElement1 != null && !bool.TryParse(xElement1.Value, out bold))
        bold = false;

      bool italic = false;

      var element1 = root.Element(XmlStructure.Italic);

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
      XElement docPart = new XElement(XmlStructure.Filter,
            new XElement(XmlStructure.Id, filter.Id),
            new XElement(XmlStructure.FilterName, filter.Description),
            new XElement(XmlStructure.FilterPattern, filter.Filter),
            new XElement(XmlStructure.Font,
              new XElement(XmlStructure.Name, filter.FilterFontType.Name),
              new XElement(XmlStructure.Size, filter.FilterFontType.Size),
              new XElement(XmlStructure.Bold, filter.FilterFontType.Bold),
              new XElement(XmlStructure.Italic, filter.FilterFontType.Italic)));

      return (docPart);
    }

    private FilterData GetFilter(XElement root)
    {
      var xElement = root.Element(XmlStructure.Id);

      if(xElement != null && GetId(xElement.Value) == Guid.Empty)
        return (null);

      var element = root.Element(XmlStructure.Id);

      if(element == null)
        return (null);

      var xElement1 = root.Element(XmlStructure.FilterPattern);

      if(xElement1 == null)
        return (null);

      var element1 = root.Element(XmlStructure.FilterName);

      if(element1 == null)
        return (null);

      FilterData filter = new FilterData
      {
        Id = GetId(element.Value),
        Filter = xElement1.Value,
        Description = element1.Value,
        FilterFontType = GetFont(root.Element(XmlStructure.Font))
      };
      return (filter);
    }

    private Pattern GetPattern(XElement pattern)
    {
      var patternString = pattern.Element(XmlStructure.PatternString);
      var patternIsRegex = pattern.Element(XmlStructure.IsRegex);

      if(patternIsRegex == null || patternString == null)
        return (null);

      Pattern xmlPattern = new Pattern
      {
        PatternString = patternString.Value,
        IsRegex = StringToBool(patternIsRegex.Value)
      };
      return (xmlPattern);
    }

    private XElement AddFiltersToRoot(ObservableCollection<FilterData> filters)
    {
      var filtersElement = new XElement(XmlStructure.Filters);

      foreach(var item in filters)
      {
        filtersElement.Add(AddFilterToDoc(item));
      }
      return (filtersElement);
    }

    private XElement AddSearchPatternToDoc(Pattern pattern)
    {
      var patternElement = new XElement(XmlStructure.SearchPattern);

      patternElement.Add(new XElement(XmlStructure.IsRegex, pattern.IsRegex));
      patternElement.Add(new XElement(XmlStructure.PatternString, pattern.PatternString));

      return (patternElement);
    }

    #endregion
  }
}
