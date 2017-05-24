using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.Template.TextEditor.Utils;


namespace Org.Vs.TailForWin.Template.TextEditor.Data
{
  /// <summary>
  /// LogViewDataTemplateSelector
  /// </summary>
  public class LogViewerDataTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Show linenumbers and logmessage
    /// </summary>
    public DataTemplate LineNumberTemplate
    {
      get;
      set;
    }

    /// <summary>
    /// Show datetime and logmessage
    /// </summary>
    public DataTemplate DateTimeTemplate
    {
      get;
      set;
    }

    /// <summary>
    /// Show logmessage only
    /// </summary>
    public DataTemplate DefaultTemplate
    {
      get;
      set;
    }

    /// <summary>
    /// Show datetime, linenumbers and logmessage
    /// </summary>
    public DataTemplate DateTimeLineNumbersTemplate
    {
      get;
      set;
    }

    /// <summary>
    /// Select template state
    /// </summary>
    /// <param name="item">Item</param>
    /// <param name="container">Container</param>
    /// <returns>DataTemplate</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      switch(TemplateData.State)
      {
      case TemplateData.TemplateStates.ShowDateTime:

        return DateTimeTemplate;

      case TemplateData.TemplateStates.ShowDateTimeLineNumber:

        return DateTimeLineNumbersTemplate;

      case TemplateData.TemplateStates.ShowLineNumber:

        return LineNumberTemplate;

      default:

        return DefaultTemplate;
      }
    }
  }
}
