using System;


namespace Org.Vs.TailForWin.Logging
{
  /// <summary>
  /// log4net LoggerFactory
  /// </summary>
  public class LoggerFactory
  {
    private static string sourceName = null;
    private static Type sourceType = null;


    /// <summary>
    /// Return a logger named according to the name parameter using the statically bound ILoggerFactory instance.
    /// </summary>
    /// <param name="sourceName">The name of the logger.</param>
    /// <returns>logger</returns>
    public static Logger GetLogger (string sourceName)
    {
      LoggerFactory.sourceName = sourceName;

      return (new Logger(LoggerFactory.sourceName));
    }

    /// <summary>
    /// Return a logger named corresponding to the class passed as parameter, using the statically bound {@link ILoggerFactory} instance.
    /// </summary>
    /// <param name="sourceType">the returned logger will be named after clazz</param>
    /// <returns>logger</returns>
    public static Logger GetLogger (Type sourceType)
    {
      LoggerFactory.sourceType = sourceType;

      return (GetLogger(LoggerFactory.sourceType.Name));
    }

    /// <summary>
    /// Return a logger named corresponding to the class passed as parameter.
    /// </summary>
    /// <typeparam name="T">the returned logger will be named after type</typeparam>
    /// <returns>logger</returns>
    public static Logger GetLogger<T> ()
    {
      LoggerFactory.sourceType = typeof(T);

      return (GetLogger(LoggerFactory.sourceType.Name));
    }
  }
}
