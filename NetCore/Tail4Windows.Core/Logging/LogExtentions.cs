using log4net;
using log4net.Core;

namespace Org.Vs.TailForWin.Core.Logging
{
  /// <summary>
  /// ILogExtension
  /// </summary>
  public static class LogExtensions
  {
    /// <summary>
    /// Fatal
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Fatal(this ILog log, string message, params object[] args) => log.Fatal(null, message, args);

    /// <summary>
    /// Fatal
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Fatal(this ILog log, Exception exception, string message) => log.Fatal(exception, message, null);

    /// <summary>
    /// Fatal
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Fatal(this ILog log, Exception exception, string message, params object[] args) => log.Fatal(FormatMessage(message, args), exception);

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Error(this ILog log, string message, params object[] args) => log.Error(null, message, args);

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Error(this ILog log, Exception exception, string message) => log.Error(exception, message, null);

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Error(this ILog log, Exception exception, string message, params object[] args) => log.Error(FormatMessage(message, args), exception);

    /// <summary>
    /// Warn
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Warn(this ILog log, string message, params object[] args) => log.Warn(null, message, args);

    /// <summary>
    /// Warn
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Warn(this ILog log, Exception exception, string message) => log.Warn(exception, message, null);

    /// <summary>
    /// Warn
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Warn(this ILog log, Exception exception, string message, params object[] args) => log.Warn(FormatMessage(message, args), exception);

    /// <summary>
    /// Info
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Info(this ILog log, string message, params object[] args) => log.Info(null, message, args);

    /// <summary>
    /// Info
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Info(this ILog log, Exception exception, string message) => log.Info(exception, message, null);

    /// <summary>
    /// Info
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Info(this ILog log, Exception exception, string message, params object[] args) => log.Info(FormatMessage(message, args), exception);

    /// <summary>
    /// Debug
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Debug(this ILog log, string message, params object[] args) => log.Debug(null, message, args);

    /// <summary>
    /// Debug
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Debug(this ILog log, Exception exception, string message) => log.Debug(exception, message, null);

    /// <summary>
    /// Debug
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Debug(this ILog log, Exception exception, string message, params object[] args) => log.Debug(FormatMessage(message, args), exception);

    /// <summary>
    /// Trace
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    public static void Trace(this ILog log, string message) => log.Trace(null, message, null);

    /// <summary>
    /// Trace
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Trace(this ILog log, string message, params object[] args) => log.Trace(null, message, args);

    /// <summary>
    /// Trace
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Trace(this ILog log, Exception exception, string message) => log.Trace(exception, message, null);

    /// <summary>
    /// Trace
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Trace(this ILog log, Exception exception, string message, params object[] args) => log.Logger.Log(Type.GetType(log.Logger.Name), Level.Trace, FormatMessage(message, args), exception);

    /// <summary>
    /// Verbose
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    public static void Verbose(this ILog log, string message) => log.Verbose(null, message, null);

    /// <summary>
    /// Verbose
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Verbose(this ILog log, string message, params object[] args) => log.Verbose(null, message, args);

    /// <summary>
    /// Verbose
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    public static void Verbose(this ILog log, Exception exception, string message) => log.Verbose(exception, message, null);

    /// <summary>
    /// Verbose
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="exception">Exception</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void Verbose(this ILog log, Exception exception, string message, params object[] args) => log.Logger.Log(Type.GetType(log.Logger.Name), Level.Verbose, FormatMessage(message, args), exception);

    /// <summary>
    /// Is trace enabled
    /// </summary>
    /// <param name="log">Logger</param>
    /// <returns>If is enabled true, otherwise false</returns>
    public static bool IsTraceEnabled(this ILog log)
    {
      return log.Logger.IsEnabledFor(log4net.Core.Level.Trace);
    }

    /// <summary>
    /// Is verbose enabled
    /// </summary>
    /// <param name="log">Logger</param>
    /// <returns>If is enabled true, otherwise false</returns>
    public static bool IsVerboseEnabled(this ILog log) => log.Logger.IsEnabledFor(log4net.Core.Level.Verbose);

    /// <summary>
    /// Begin method
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="methodName">Name of method</param>
    public static void BeginMethod(this ILog log, string methodName) => log.Debug(methodName + " START");

    /// <summary>
    /// Begin method
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="methodName">Name of method</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void BeginMethod(this ILog log, string methodName, string message, params object[] args) => log.Debug($"{methodName} START - {message}", args);

    /// <summary>
    /// Begin method
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="logLevel">Log level</param>
    /// <param name="methodName">Name of method</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void BeginMethod(this ILog log, Level logLevel, string methodName, string message, params object[] args)
    {
      if ( Level.Error.Equals(logLevel) )
        log.Error($"{methodName} START - {message}", args);
      else if ( Level.Warn.Equals(logLevel) )
        log.Warn($"{methodName} START - {message}", args);
      else if ( Level.Info.Equals(logLevel) )
        log.Info($"{methodName} START - {message}", args);
      else if ( Level.Debug.Equals(logLevel) )
        log.Debug($"{methodName} START - {message}", args);
      else if ( Level.Trace.Equals(logLevel) )
        log.Trace($"{methodName} START - {message}", args);
      else
        log.Debug($"{methodName} START - {message}", args);
    }

    /// <summary>
    /// End method
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="methodName">Name of method</param>
    public static void EndMethod(this ILog log, string methodName) => log.Debug(methodName + " END");

    /// <summary>
    /// End method
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="methodName">Name of method</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void EndMethod(this ILog log, string methodName, string message, params object[] args) => log.Debug($"{methodName} END   - {message}", args);

    /// <summary>
    /// End method
    /// </summary>
    /// <param name="log">Logger</param>
    /// <param name="logLevel">Log level</param>
    /// <param name="methodName">Name of method</param>
    /// <param name="message">Message</param>
    /// <param name="args">Arguments</param>
    public static void EndMethod(this ILog log, Level logLevel, string methodName, string message, params object[] args)
    {
      if ( Level.Error.Equals(logLevel) )
        log.Error($"{methodName} END   - {message}", args);
      else if ( Level.Warn.Equals(logLevel) )
        log.Warn($"{methodName} END   - {message}", args);
      else if ( Level.Info.Equals(logLevel) )
        log.Info($"{methodName} END   - {message}", args);
      else if ( Level.Debug.Equals(logLevel) )
        log.Debug($"{methodName} END   - {message}", args);
      else if ( Level.Trace.Equals(logLevel) )
        log.Trace($"{methodName} END   - {message}", args);
      else
        log.Debug($"{methodName} END   - {message}", args);
    }

    private static string FormatMessage(string message, params object[] args) => args == null || args.Length < 1 ? message : string.Format(message, args);
  }
}
