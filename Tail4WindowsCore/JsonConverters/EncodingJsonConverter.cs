using System;
using System.Linq;
using System.Text;
using log4net;
using Newtonsoft.Json;


namespace Org.Vs.TailForWin.Core.JsonConverters
{
  /// <summary>
  /// File encoding JSON converter
  /// </summary>
  public class EncodingJsonConverter : JsonConverter
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(EncodingJsonConverter));

    /// <summary>
    /// Writes the JSON representation of the object
    /// </summary>
    /// <param name="writer"><see cref="JsonWriter"/></param>
    /// <param name="value">Object to write</param>
    /// <param name="serializer"><see cref="JsonSerializer"/></param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      switch ( value )
      {
      case null:

        writer.WriteNull();
        break;

      case Encoding encoding:

        writer.WriteValue(encoding.HeaderName);
        break;

      default:

        writer.WriteValue(string.Empty);
        break;
      }
    }

    /// <summary>
    /// Reads the JSON representation of the object
    /// </summary>
    /// <param name="reader"><see cref="JsonReader"/></param>
    /// <param name="objectType">Object type</param>
    /// <param name="existingValue">Value to read</param>
    /// <param name="serializer"><see cref="JsonSerializer"/></param>
    /// <returns>The object value.</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      try
      {
        Encoding encoding = null;

        foreach ( Encoding encode in Encoding.GetEncodings().Select(p => p.GetEncoding()) )
        {
          if ( string.Compare(encode.HeaderName, reader.Value as string, StringComparison.Ordinal) == 0 )
          {
            encoding = encode;
            break;
          }

          encoding = Encoding.UTF8;
        }

        return encoding;
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return null;
    }

    /// <summary>
    /// Determines whether this instance can convert the specified object type
    /// </summary>
    /// <param name="objectType">Object type</param>
    /// <returns><c>True</c> if can convert, otherwise <c>False</c></returns>
    public override bool CanConvert(Type objectType) => throw new NotImplementedException();
  }
}
