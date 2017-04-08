using System;
using log4net;


namespace Org.Vs.TailForWin.Logging
{
  /// <summary>
  /// log4net Logger
  /// </summary>
  public class Logger
  {
    readonly string sourceName;
    private readonly ILog logger;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sourceName">Source name</param>
    public Logger(string sourceName)
    {
      this.sourceName = sourceName;
      logger = LogManager.GetLogger(sourceName);
    }


    /// <summary>
    /// the name of this <code>Logger</code> instance.
    /// </summary>
    public string Name => sourceName;

    /// <summary>
    /// Is the logger instance enabled for the TRACE level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the TRACE level, false otherwise.</returns>
    public bool IsTraceEnabled()
    {
      return (logger.IsTraceEnabled());
    }

    /// <summary>
    /// Log a message at the TRACE level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Trace(string msg)
    {
      logger.Trace(msg);
    }

    /// <summary>
    /// Log a message at the TRACE level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the TRACE level.
    /// </summary>
    /// <param name="format">format the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Trace(string format, object[] argArray)
    {
      logger.Trace(format, argArray);
    }

    /// <summary>
    /// Log an exception (throwable) at the TRACE level with an accompanying message. 
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="e">the exception (throwable) to log</param>
    public void Trace(string msg, Exception e)
    {
      logger.Trace(e, msg);
    }

    /// <summary>
    /// Is the logger instance enabled for the DEBUG level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the DEBUG level, false otherwise</returns>
    public bool IsDebugEnabled()
    {
      return (logger.IsDebugEnabled);
    }

    /// <summary>
    /// Log a message at the DEBUG level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Debug(string msg)
    {
      logger.Debug(msg);
    }

    /// <summary>
    /// Log a message at the DEBUG level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the DEBUG level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Debug(String format, object[] argArray)
    {
      logger.Debug(string.Format(format, argArray));
    }

    /// <summary>
    /// Log an exception (throwable) at the DEBUG level with an accompanying message.
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log</param>
    public void Debug(string msg, Exception t)
    {
      logger.Debug(msg, t);
    }

    /// <summary>
    /// Is the logger instance enabled for the INFO level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the INFO level, false otherwise.</returns>
    public bool IsInfoEnabled()
    {
      return (logger.IsInfoEnabled);
    }

    /// <summary>
    /// Log a message at the INFO level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Info(String msg)
    {
      logger.Info(msg);
    }

    /// <summary>
    /// Log a message at the INFO level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the INFO level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Info(string format, object[] argArray)
    {
      logger.Info(string.Format(format, argArray));
    }

    /// <summary>
    /// Log an exception (throwable) at the INFO level with an accompanying message. 
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log </param>
    public void Info(string msg, Exception t)
    {
      logger.Info(msg, t);
    }

    /// <summary>
    /// Is the logger instance enabled for the WARN level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the WARN level, false otherwise.</returns>
    public bool IsWarnEnabled()
    {
      return (logger.IsWarnEnabled);
    }

    /// <summary>
    /// Log a message at the WARN level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Warn(string msg)
    {
      logger.Warn(msg);
    }

    /// <summary>
    /// Log a message at the WARN level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the WARN level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Warn(string format, object[] argArray)
    {
      logger.Warn(string.Format(format, argArray));
    }

    /// <summary>
    /// Log an exception (throwable) at the WARN level with an accompanying message. 
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log </param>
    public void Warn(string msg, Exception t)
    {
      logger.Warn(msg, t);
    }

    /// <summary>
    /// Is the logger instance enabled for the ERROR level?
    /// </summary>
    /// <returns>True if this Logger is enabled for the ERROR level, false otherwise.</returns>
    public bool IsErrorEnabled()
    {
      return (logger.IsErrorEnabled);
    }

    /// <summary>
    /// Log a message at the ERROR level.
    /// </summary>
    /// <param name="msg">the message string to be logged</param>
    public void Error(string msg)
    {
      logger.Error(msg);
    }

    /// <summary>
    /// Log a message at the ERROR level according to the specified format and arguments.
    /// This form avoids superfluous object creation when the logger is disabled for the ERROR level.
    /// </summary>
    /// <param name="format">the format string</param>
    /// <param name="argArray">an array of arguments</param>
    public void Error(string format, object[] argArray)
    {
      logger.Error(string.Format(format, argArray));
    }

    /// <summary>
    /// Log an exception (throwable) at the ERROR level with an accompanying message.
    /// </summary>
    /// <param name="msg">the message accompanying the exception</param>
    /// <param name="t">the exception (throwable) to log</param>
    public void Error(string msg, Exception t)
    {
      logger.Error(msg, t);
    }

    private string Format(string msg, Exception t)
    {
      if(t == null)
        return (msg);

      return ($"\"{msg}\"\nstack trace:\n\"{t.StackTrace}\"");
    }
  }
}