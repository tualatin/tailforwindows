using System;
using System.IO;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Controller
{
  /// <summary>
  /// History structure controller
  /// </summary>
  public class HistoryStructureController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(HistoryStructureController));

    private const string XMLROOT = "historyFile";
    private XDocument historyDoc;
    private readonly string historyFile;

    /// <summary>
    /// Wrap around in search dialogue
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }


    /// <summary>
    /// Standard history structure controller
    /// </summary>
    public HistoryStructureController()
    {
      historyFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\History.xml";
    }

    /// <summary>
    /// Read find history section in XML file
    /// </summary>
    public void ReadFindHistory(ref ObservableDictionary<string, string> words)
    {
      try
      {
        if(!File.Exists(historyFile))
          return;

        historyDoc = XDocument.Load(historyFile);

        if(historyDoc.Root == null)
          return;

        XElement findHistoryRoot = historyDoc.Root.Element("FindHistory");

        if(findHistoryRoot == null)
          return;

        string wrapAround = findHistoryRoot.Attribute("wrap").Value;
        Wrap = bool.TryParse(wrapAround, out bool wrap) && wrap;

        foreach(XElement find in findHistoryRoot.Elements("Find"))
        {
          words.Add(find.Attribute("name").Value, find.Attribute("name").Value);
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Save find history attribute wrap
    /// </summary>
    public XElement SaveFindHistoryWrap()
    {
      if(!File.Exists(historyFile))
        historyDoc = new XDocument(new XElement(XMLROOT));

      try
      {
        if(historyDoc.Root != null)
        {
          XElement findHistoryRoot = historyDoc.Root.Element("FindHistory");

          if(findHistoryRoot != null)
            findHistoryRoot.Attribute("wrap").Value = Wrap.ToString();
          else
          {
            findHistoryRoot = new XElement("FindHistory");
            findHistoryRoot.Add(new XAttribute("wrap", Wrap.ToString()));
            historyDoc.Root.Add(findHistoryRoot);
          }

          historyDoc.Save(historyFile, SaveOptions.None);

          return (findHistoryRoot);
        }
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return (null);
    }

    /// <summary>
    /// Save find history attribute name
    /// </summary>
    /// <param name="searchWord">Find what word</param>
    public void SaveFindHistoryName(string searchWord)
    {
      if(!File.Exists(historyFile))
        historyDoc = new XDocument(new XElement(XMLROOT));

      try
      {
        if(historyDoc.Root != null)
        {
          XElement findHistoryRoot = historyDoc.Root.Element("FindHistory") ?? SaveFindHistoryWrap();
          XElement findHistoryFind = new XElement("Find");
          findHistoryFind.Add(new XAttribute("name", searchWord));
          findHistoryRoot.Add(findHistoryFind);
        }

        historyDoc.Save(historyFile, SaveOptions.None);
      }
      catch(Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }
  }
}
