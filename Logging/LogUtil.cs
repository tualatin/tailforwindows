using System;
using System.Text;


namespace Org.Vs.TailForWin.Logging
{
  /// <summary>
  /// log4net LogUtil
  /// </summary>
  public class LogUtil
  {
    private static readonly Logger LOG = LoggerFactory.GetLogger<LogUtil>();


    private LogUtil()
    {
    }

    /// <summary>
    /// Creates a comma separated list of types and values for an array of objects.
    /// </summary>
    /// <param name="args">The array. Can be <code>null</code>.</param>
    /// <returns>The list. Never <code>null</code> and never empty.</returns>
    public static string TypesToString(object[] args)
    {
      string result = "null";

      if(args != null)
      {
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < args.Length; i++)
        {
          object arg = args[i];

          if(sb.Length > 0)
            sb.Append(", ");

          if(arg == null)
          {
            sb.Append("null");
          }
          else
          {
            sb.Append(arg.GetType().Name).Append("=");

            try
            {
              sb.Append(arg.ToString());
            }
            catch(SystemException e) //NOCS: IllegalCatch, because: toString() methods may be buggy, but anyway there should be logged a message
            {
              Warn(LOG, e, "Could not transform argument {0} to string. Using its class name instead.");
              sb.Append(arg.GetType().Name);
            }
          }
        }

        sb.Append("[").Append(result).Append("]");
        result = sb.ToString();
      }
      return (result);
    }

    private static string Format(string message, params object[] args)
    {
      //if ( ArrayUtil.isEmpty(args) )
      //{
      //  return message;
      //}

      //MessageFormat mf = new MessageFormat(StringUtil.replace(message, "'", "''"));
      //Format[] formats = mf.getFormatsByArgumentIndex();
      //// fix bug from Sun with wrong space in grouping separator
      //// see http://bugs.sun.com/view_bug.do?bug_id=6318800
      //for ( int i = 0; i < formats.length; i++ )
      //{
      //  if ( formats[i] instanceof DecimalFormat )
      //  {
      //    DecimalFormat decimalFormat = (DecimalFormat) formats[i];
      //    DecimalFormatSymbols decimalFormatSymbols = decimalFormat.getDecimalFormatSymbols();
      //    if ( decimalFormatSymbols.getGroupingSeparator() == '\u00A0' )
      //    {
      //      decimalFormatSymbols.setGroupingSeparator(' ');
      //      decimalFormat.setDecimalFormatSymbols(decimalFormatSymbols);
      //      mf.setFormatByArgumentIndex(i, decimalFormat);
      //    }
      //  }
      //}

      //return mf.format(args);

      return (string.Format(message, args));
    }

    /// <summary>Forwards to #debug(Logger, Throwable, String, params object[]) with no error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code></param>
    /// <param name="message">Message to log</param>
    /// <param name="args">an array of arguments</param>
    public static void Debug(Logger logger, string message, params object[] args)
    {
      Debug(logger, null, message, args);
    }

    /// <summary>
    /// Logs a message on DEBUG level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code></param>
    /// <param name="error">The error. Can be <code>null</code>.</param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Debug(Logger logger, Exception error, string message, params object[] args)
    {
      if(logger.IsDebugEnabled())
        logger.Debug(Format(message, args), error);
    }

    /// <summary>
    /// Forwards to #info(Logger, Throwable, String, params object[]) with no error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code></param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Info(Logger logger, string message, params object[] args)
    {
      Info(logger, null, message, args);
    }

    /// <summary>
    /// Logs a message on INFO level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code>.</param>
    /// <param name="error">The error. Can be <code>null</code>.</param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Info(Logger logger, Exception error, string message, params object[] args)
    {
      if(logger.IsInfoEnabled())
        logger.Info(Format(message, args), error);
    }

    /// <summary>
    /// Forwards to #warn(Logger, Throwable, string, params object[]) with no error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code></param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Warn(Logger logger, string message, params object[] args)
    {
      Warn(logger, null, message, args);
    }

    /// <summary>
    /// Logs a message on WARN level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code>.</param>
    /// <param name="error">The error. Can be <code>null</code>.</param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Warn(Logger logger, Exception error, string message, params object[] args)
    {
      if(logger.IsWarnEnabled())
        logger.Warn(Format(message, args), error);
    }

    /// <summary>
    /// Logs a message on TRACE level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code>.</param>
    /// <param name="error">The error. Can be <code>null</code>.</param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Trace(Logger logger, Exception error, string message, params object[] args)
    {
      if(logger.IsTraceEnabled())
        logger.Trace(Format(message, args), error);
    }

    /// <summary>
    /// Logs a message on TRACE level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code></param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Trace(Logger logger, string message, params object[] args)
    {
      Trace(logger, null, message, args);
    }

    /// <summary>
    /// Logs a message on ERROR level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code></param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Error(Logger logger, string message, params object[] args)
    {
      Error(logger, null, message, args);
    }

    /// <summary>
    /// Logs a message on ERROR level with or without error.
    /// </summary>
    /// <param name="logger">The logger. Not <code>null</code>.</param>
    /// <param name="error">The error. Can be <code>null</code><./param>
    /// <param name="message">The logged message. Not <code>null</code> and not empty. Characters are given unescaped. Placeholders are given as {0}, {1}, etc.</param>
    /// <param name="args">Arguments for the error message. Can be <code>null</code> or empty.</param>
    public static void Error(Logger logger, Exception error, string message, params object[] args)
    {
      if(logger.IsErrorEnabled())
        logger.Error(Format(message, args), error);
    }
  }
}
