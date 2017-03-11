using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Data.XmlNames;
using Org.Vs.TailForWin.Interfaces;


namespace Org.Vs.TailForWin.Controller
{
  /// <summary>
  /// Default pattern structure controller
  /// </summary>
  public class DefaultPatternStructureController : IDefaultPatternStructureController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(DefaultPatternStructureController));

    private const string XMLROOT = "defaultPatternFile";
    private XDocument defaultPatternDoc;
    private readonly string patternFile;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public DefaultPatternStructureController()
    {
      patternFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\DefaultPatterns.xml";
    }

    /// <summary>
    /// Reads the default pattern XML file
    /// </summary>
    /// <returns>List of default patterns</returns>
    public List<Pattern> ReadDefaultPatternFile()
    {
      if(!File.Exists(patternFile))
        return (null);

      defaultPatternDoc = XDocument.Load(patternFile);

      if(defaultPatternDoc.Root == null)
        return (null);

      List<Pattern> defaultPatterns = new List<Pattern>();

      foreach(XElement xe in defaultPatternDoc.Root.Descendants(XmlStructure.Pattern))
      {
        Pattern newPattern = GetPattern(xe);

        if(newPattern == null)
          continue;

        defaultPatterns.Add(newPattern);
      }
      return (defaultPatterns);
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

    private bool StringToBool(string boolean)
    {
      if(!bool.TryParse(boolean, out bool outValue))
        outValue = false;

      return (outValue);
    }
  }
}
