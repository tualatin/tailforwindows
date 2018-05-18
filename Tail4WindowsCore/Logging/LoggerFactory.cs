using System;


namespace Org.Vs.TailForWin.Core.Logging
{
  /// <summary>
  /// log4net LoggerFactory
  /// </summary>
  public static class LoggerFactory
  {
    private static string sourceName;
    private static Type sourceType;


    /// <summary>
    /// Return a logger named according to the name parameter using the statically bound ILoggerFactory instance.
    /// </summary>
    /// <param name="scName">The name of the logger.</param>
    /// <returns>logger</returns>
    public static Logger GetLogger(string scName)
    {
      LoggerFactory.sourceName = scName;

      return new Logger(LoggerFactory.sourceName);
    }

    /// <summary>
    /// Return a logger named corresponding to the class passed as parameter, using the statically bound {@link ILoggerFactory} instance.
    /// </summary>
    /// <param name="scType">the returned logger will be named after clazz</param>
    /// <returns>logger</returns>
    public static Logger GetLogger(Type scType)
    {
      LoggerFactory.sourceType = scType;

      return GetLogger(LoggerFactory.sourceType.Name);
    }

    /// <summary>
    /// Return a logger named corresponding to the class passed as parameter.
    /// </summary>
    /// <typeparam name="T">the returned logger will be named after type</typeparam>
    /// <returns>logger</returns>
    public static Logger GetLogger<T>()
    {
      sourceType = typeof(T);

      return GetLogger(sourceType.Name);
    }
  }
}
