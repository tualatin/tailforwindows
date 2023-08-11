using System.Text.Json;

namespace Org.Vs.Tail4Win.Core.Extensions
{
  internal static class SerializationExtensions
  {
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
      PropertyNamingPolicy = null,
      AllowTrailingCommas = true
    };

    internal static byte[] Serialize<T>(this T obj) => JsonSerializer.SerializeToUtf8Bytes(obj, SerializerOptions);

    internal static T Deserialize<T>(this byte[] data) => JsonSerializer.Deserialize<T>(data, SerializerOptions);
  }
}
