using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace Org.Vs.TailForWin.UserControls
{
  /// <summary>
  /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
  ///
  /// Step 1a) Using this custom control in a XAML file that exists in the current project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:Org.Vs.TailForWin.UserControls"
  ///
  ///
  /// Step 1b) Using this custom control in a XAML file that exists in a different project.
  /// Add this XmlNamespace attribute to the root element of the markup file where it is 
  /// to be used:
  ///
  ///     xmlns:MyNamespace="clr-namespace:Org.Vs.TailForWin;assembly=Org.Vs.TailForWin"
  ///
  /// You will also need to add a project reference from the project where the XAML file lives
  /// to this project and Rebuild to avoid compilation errors:
  ///
  ///     Right click on the target project in the Solution Explorer and
  ///     "Add Reference"->"Projects"->[Browse to and select this project]
  ///
  ///
  /// Step 2)
  /// Go ahead and use your control in the XAML file.
  ///
  ///     <MyNamespace:TailScrollBar/>
  ///
  /// </summary>
  [TemplatePart(Name = "PART_SplitterControl", Type = typeof(Border))]
  public class TailScrollBar : ScrollBar
  {
    static TailScrollBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TailScrollBar), new FrameworkPropertyMetadata(typeof(TailScrollBar)));
    }

    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <code>ApplyTemplate</code>.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
    }
  }
}
