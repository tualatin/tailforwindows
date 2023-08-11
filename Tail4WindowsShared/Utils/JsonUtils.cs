using System.IO;
using Newtonsoft.Json;

namespace Org.Vs.Tail4Win.Shared.Utils
{
  /// <summary>
  /// Some JSON utils
  /// </summary>
  public static class JsonUtils
  {
    /// <summary>
    /// Write JSON to file
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="element">Element</param>
    /// <param name="file">Path of file</param>
    public static void WriteJsonFile<T>(T element, string file)
    {
      using ( FileStream fs = File.Open(file, FileMode.Create) )
      using ( var sw = new StreamWriter(fs) )
      using ( JsonWriter jw = new JsonTextWriter(sw) )
      {
        jw.Formatting = Formatting.Indented;
        var serializer = new JsonSerializer
        {
          NullValueHandling = NullValueHandling.Ignore
        };
        serializer.Serialize(jw, element);
      }
    }

    /// <summary>
    /// Reads a JSON file
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    /// <param name="file">Path of file</param>
    /// <returns>Generic type</returns>
    public static T ReadJsonFile<T>(string file)
    {
      using ( StreamReader sr = File.OpenText(file) )
      {
        var serializer = new JsonSerializer();
        var serializedObject = (T) serializer.Deserialize(sr, typeof(T));

        return serializedObject;
      }
    }
  }
}
