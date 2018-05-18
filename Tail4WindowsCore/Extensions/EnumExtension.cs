using System;
using System.ComponentModel;
using System.Reflection;


namespace Org.Vs.TailForWin.Core.Extensions
{
  /// <summary>
  /// Enum extension
  /// </summary>
  public static class EnumExtension
  {
    /// <summary>
    /// Get a Enum description
    /// </summary>
    /// <param name="value">Enum value</param>
    /// <returns>Description of Enum</returns>
    public static string GetEnumDescription(this Enum value)
    {
      FieldInfo fi = value.GetType().GetField(value.ToString());
      DescriptionAttribute[] attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

      return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }

    /// <summary>
    /// Get enum by description
    /// </summary>
    /// <typeparam name="T">Type of Enum</typeparam>
    /// <param name="value">Description string</param>
    /// <returns>Enum type</returns>
    public static T GetEnumByDescription<T>(this string value)
    {
      var type = typeof(T);

      if( !type.IsEnum )
        throw new InvalidOperationException();

      foreach( var field in type.GetFields() )
      {
        if( Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute )
        {
          if( string.CompareOrdinal(attribute.Description, value) == 0 )
            return (T) field.GetValue(null);
        }
        else
        {
          if( string.CompareOrdinal(field.Name, value) == 0 )
            return (T) field.GetValue(null);
        }
      }
      throw new ArgumentException(nameof(value));
    }
  }
}
