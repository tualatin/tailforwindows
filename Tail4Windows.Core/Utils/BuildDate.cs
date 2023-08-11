using System.Reflection;
using Org.Vs.Tail4Win.Core.Attributes;
using Org.Vs.Tail4Win.Core.Controllers;
using Org.Vs.Tail4Win.Core.Extensions;

namespace Org.Vs.Tail4Win.Core.Utils
{
  /// <summary>
  /// Build date of assembly
  /// </summary>
  public class BuildDate
  {
    /// <summary>
    /// Gets the build date by assembly
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string GetBuildDateByAssembly(Assembly assembly)
    {
      var buildAttribute = Attribute.GetCustomAttribute(assembly, typeof(BuildDateAttribute)) as BuildDateAttribute;

      if ( !DateTime.TryParse(buildAttribute?.Date, out var dt) )
        return string.Empty;

      string format = $"{SettingsHelperController.CurrentSettings.DefaultDateFormat.GetEnumDescription()} " +
                      $"{SettingsHelperController.CurrentSettings.DefaultTimeFormat.GetEnumDescription()}";

      return dt.ToString(format);

    }
  }
}
