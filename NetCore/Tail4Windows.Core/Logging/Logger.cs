using log4net;

namespace Org.Vs.TailForWin.Core.Logging
{
  /// <summary>
  /// log4net Logger
  /// </summary>
  public class Logger
  {
    readonly string _sourceName;
    private readonly ILog _logger;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sourceName">Source name</param>
    public Logger(string sourceName)
    {
      _sourceName = sourceName;
      _logger = LogManager.GetLogger(sourceName);
    }


    /// <summary>
    /// the name of this <code>Logger</code> instance.
    /// </summary>
    public string Name => _sourceName;

    /// <summary>
    /// Is the logger instance enabled for the TRACE level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the TRACE level, false otherwise.</returns>
    public bool IsTraceEnabled() => _logger.IsTraceEnabled();

    /// <summary>
    /// Log a message at the TRACE level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Trace(string msg) => _logger.Trace(msg);

    /// <summary>
    /// Log a message at the TRACE level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the TRACE level.
    /// </summary>
    /// <param name="format">format the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Trace(string format, object[] argArray) => _logger.Trace(format, argArray);

    /// <summary>
    /// Log an exception (throwable) at the TRACE level with an accompanying message. 
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="e">the exception (throwable) to log</param>
    public void Trace(string msg, Exception e) => _logger.Trace(e, msg);

    /// <summary>
    /// Is the logger instance enabled for the DEBUG level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the DEBUG level, false otherwise</returns>
    public bool IsDebugEnabled() => _logger.IsDebugEnabled;

    /// <summary>
    /// Log a message at the DEBUG level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Debug(string msg) => _logger.Debug(msg);

    /// <summary>
    /// Log a message at the DEBUG level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the DEBUG level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Debug(string format, object[] argArray) => _logger.Debug(string.Format(format, argArray));

    /// <summary>
    /// Log an exception (throwable) at the DEBUG level with an accompanying message.
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log</param>
    public void Debug(string msg, Exception t) => _logger.Debug(msg, t);

    /// <summary>
    /// Is the logger instance enabled for the INFO level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the INFO level, false otherwise.</returns>
    public bool IsInfoEnabled() => _logger.IsInfoEnabled;

    /// <summary>
    /// Log a message at the INFO level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Info(string msg) => _logger.Info(msg);

    /// <summary>
    /// Log a message at the INFO level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the INFO level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Info(string format, object[] argArray) => _logger.Info(string.Format(format, argArray));

    /// <summary>
    /// Log an exception (throwable) at the INFO level with an accompanying message. 
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log </param>
    public void Info(string msg, Exception t) => _logger.Info(msg, t);

    /// <summary>
    /// Is the logger instance enabled for the WARN level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the WARN level, false otherwise.</returns>
    public bool IsWarnEnabled() => _logger.IsWarnEnabled;

    /// <summary>
    /// Log a message at the WARN level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Warn(string msg) => _logger.Warn(msg);

    /// <summary>
    /// Log a message at the WARN level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the WARN level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Warn(string format, object[] argArray) => _logger.Warn(string.Format(format, argArray));

    /// <summary>
    /// Log an exception (throwable) at the WARN level with an accompanying message. 
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log </param>
    public void Warn(string msg, Exception t) => _logger.Warn(msg, t);

    /// <summary>
    /// Is the logger instance enabled for the ERROR level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the ERROR level, false otherwise.</returns>
    public bool IsErrorEnabled() => _logger.IsErrorEnabled;

    /// <summary>
    /// Log a message at the ERROR level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Error(string msg) => _logger.Error(msg);

    /// <summary>
    /// Log a message at the ERROR level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the ERROR level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Error(string format, object[] argArray) => _logger.Error(string.Format(format, argArray));

    /// <summary>
    /// Log an exception (throwable) at the ERROR level with an accompanying message.
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log</param>
    public void Error(string msg, Exception t) => _logger.Error(msg, t);

    private string Format(string msg, Exception t) => t == null ? msg : $"\"{msg}\"\nstack trace:\n\"{t.StackTrace}\"";
  }
}
