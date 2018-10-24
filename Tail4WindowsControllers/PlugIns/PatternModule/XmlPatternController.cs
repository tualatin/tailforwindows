using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Data;
using Org.Vs.TailForWin.Controllers.PlugIns.PatternModule.Interfaces;
using Org.Vs.TailForWin.Core.Data.XmlNames;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Controllers.PlugIns.PatternModule
{
  /// <summary>
  /// XML default pattern controller
  /// </summary>
  public class XmlPatternController : IXmlPattern
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(XmlPatternController));

    private readonly string _xmlPatternFile;
    private XDocument _xmlDocument;


    /// <summary> 
    /// Standard constructor
    /// </summary>
    public XmlPatternController() => _xmlPatternFile = CoreEnvironment.ApplicationPath + @"\DefaultPatterns.xml";

    /// <summary>
    /// Constructor for testing purposes
    /// </summary>
    /// <param name="path">Path of XML file</param>
    public XmlPatternController(string path) => _xmlPatternFile = path;

    /// <summary>
    /// Reads default patterns for SmartWatch
    /// </summary>
    /// <returns>List of default patterns</returns>
    public async Task<List<PatternData>> ReadDefaultPatternsAsync() => await Task.Run(() => ReadDefaultPatterns()).ConfigureAwait(false);

    private List<PatternData> ReadDefaultPatterns()
    {
      var defaultPatterns = new List<PatternData>();

      try
      {
        if ( !File.Exists(_xmlPatternFile) )
          return null;

        LOG.Trace("Read DefaultPatterns");

        _xmlDocument = XDocument.Load(_xmlPatternFile);
        Parallel.ForEach(_xmlDocument.Root?.Descendants(XmlNames.Pattern) ?? throw new InvalidOperationException(), f =>
        {
          var pattern = GetPattern(f);

          if ( pattern != null )
            defaultPatterns.Add(pattern);
        });
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return defaultPatterns;
    }

    private PatternData GetPattern(XContainer xPattern)
    {
      var patternString = xPattern.Element(XmlBaseStructure.PatternString);
      var patternIsRegex = xPattern.Element(XmlBaseStructure.IsRegex)?.Value.ConvertToBool();

      return patternString == null || !patternIsRegex.HasValue
        ? null
        : new PatternData
      {
        PatternString = patternString.Value,
        IsRegex = (bool) patternIsRegex
      };
    }
  }
}
