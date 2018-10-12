using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Org.Vs.TailForWin.Controllers.UI.Vml.Attributes;


namespace Org.Vs.TailForWin.Controllers.UI.Vml
{
  /// <summary>
  /// View model locator
  /// </summary>
  public static class ViewModelLocator
  {
    /// <summary>
    /// Gets automatic locator
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <returns><c>True</c> or <c>False</c></returns>
    public static bool GetIsAutomaticLocator(DependencyObject obj) => (bool) obj.GetValue(IsAutomaticLocatorProperty);

    /// <summary>
    /// Sets automatic locator
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <param name="value">Value as <see cref="bool"/></param>
    public static void SetIsAutomaticLocator(DependencyObject obj, bool value) => obj.SetValue(IsAutomaticLocatorProperty, value);

    /// <summary>
    /// IsAutomaticLocator property
    /// </summary>
    public static readonly DependencyProperty IsAutomaticLocatorProperty = DependencyProperty.RegisterAttached("IsAutomaticLocator", typeof(bool),
      typeof(ViewModelLocator), new PropertyMetadata(false, IsAutomaticLocatorChanged));

    private static void IsAutomaticLocatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is FrameworkElement callOwner) )
        return;

      object userControl = GetInstanceOf(callOwner.GetType());
      callOwner.DataContext = userControl;
    }

    private static object GetInstanceOf(Type dependencyPropertyType)
    {
      Assembly assembly = dependencyPropertyType.Assembly;
      var assemblyTypes = assembly.GetTypes();
      string classNameDef = $"{dependencyPropertyType.Name}ViewModel";

      Type userControlType = assemblyTypes.FirstOrDefault(p => p.Name.Contains(classNameDef) && p.CustomAttributes != null &&
                                                    p.CustomAttributes.Any(a => a.AttributeType == typeof(LocatorAttribute)));

      if ( userControlType == null )
        userControlType = assemblyTypes.FirstOrDefault(a => a.Name.Contains(classNameDef) && !a.Attributes.HasFlag(TypeAttributes.Abstract));

      if ( userControlType == null )
        throw new ArgumentException($"Not exist a type {classNameDef} in the assembly {assembly.FullName}");

      return Activator.CreateInstance(userControlType);
    }
  }
}
