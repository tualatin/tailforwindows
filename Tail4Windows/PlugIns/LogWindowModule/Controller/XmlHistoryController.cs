﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Interfaces;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Data;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Controller
{
  /// <summary>
  /// XML history controller
  /// </summary>
  public class XmlHistoryController : IXmlSearchHistory<QueueSet<string>>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlHistoryController));

    private readonly string _historyFile;
    private XDocument _xmlDocument;

    private const int MaxQueueSize = 15;

    /// <summary>
    /// Wrap at the end of search: Not implemented!
    /// </summary>
    public bool Wrap
    {
      get;
      set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public XmlHistoryController() => _historyFile = EnvironmentContainer.ApplicationPath + @"\LogfileHistory.xml";

    /// <summary>
    /// Read XML file
    /// </summary>
    /// <returns>Task</returns>
    public async Task<QueueSet<string>> ReadXmlFileAsync()
    {
      if ( !File.Exists(_historyFile) )
        return new QueueSet<string>(MaxQueueSize);

      LOG.Trace("Read history");

      return await Task.Run(() => ReadXmlFile()).ConfigureAwait(false);
    }

    private QueueSet<string> ReadXmlFile()
    {
      var history = new QueueSet<string>(MaxQueueSize);

      try
      {
        _xmlDocument = XDocument.Load(_historyFile);
        var historyRoot = _xmlDocument.Root?.Element(XmlNames.LogfileHistory);

        if ( historyRoot == null )
          return history;

        Parallel.ForEach(historyRoot.Elements(XmlNames.History), f => history.Add(f.Attribute(XmlBaseStructure.Name)?.Value));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return history;
    }

    /// <summary>
    /// Save search word as XML attribute
    /// </summary>
    /// <param name="searchWord">Search text to save into XML file</param>
    /// <returns>Task</returns>
    public async Task SaveSearchHistoryAsync(string searchWord)
    {
      LOG.Trace("Save history");

      await Task.Run(() => SaveSearchHistory(searchWord)).ConfigureAwait(false);
    }

    private void SaveSearchHistory(string fileName)
    {
      if ( string.IsNullOrWhiteSpace(fileName) )
        return;

      if ( !File.Exists(_historyFile) )
        _xmlDocument = new XDocument(new XElement(XmlNames.HistoryXmlRoot));

      try
      {
        var root = _xmlDocument.Root?.Element(XmlNames.LogfileHistory);

        if ( root == null )
        {
          root = new XElement(XmlNames.LogfileHistory);
          _xmlDocument.Root?.Add(root);
        }

        var find = new XElement(XmlNames.History);
        find.Add(new XAttribute(XmlBaseStructure.Name, fileName));
        root.Add(find);

        _xmlDocument.Save(_historyFile, SaveOptions.None);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }

    /// <summary>
    /// Save search history wrap as XML attribute: Not implemented!
    /// </summary>
    /// <returns>XML element, if an error occurred, <c>null</c></returns>
    public Task<XElement> SaveSearchHistoryWrapAttributeAsync() => throw new NotImplementedException();
  }
}
