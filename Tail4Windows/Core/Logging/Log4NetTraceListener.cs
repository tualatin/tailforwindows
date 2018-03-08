using log4net;


namespace Org.Vs.TailForWin.Core.Logging
{
  /// <summary>
  /// Log4Net trace listener
  /// </summary>
  public class Log4NetTraceListener : System.Diagnostics.TraceListener
  {
    // ReSharper disable once InconsistentNaming
    private readonly ILog LOG;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public Log4NetTraceListener() => LOG = LogManager.GetLogger("System.Diagnostics.Redirection");

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="log">Reference to ILog</param>
    public Log4NetTraceListener(ILog log) => LOG = log;

    /// <summary>
    /// Register new logger
    /// </summary>
    public static void RegisterToTrace() => System.Diagnostics.Trace.Listeners.Add(new Log4NetTraceListener());

    /// <summary>
    /// Write a debug message into log
    /// </summary>
    /// <param name="message">Message to write</param>
    public override void Write(string message) => LOG?.Debug(message);

    /// <summary>
    /// Write a debug message into log
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="category">Category</param>
    public override void Write(string message, string category) => LOG?.Debug($"{category} - {message}");

    /// <summary>
    /// Write a line as debug into log
    /// </summary>
    /// <param name="message">Message to write</param>
    public override void WriteLine(string message) => LOG?.Debug(message);

    /// <summary>
    /// Write a line as debug into log
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="category">Category</param>
    public override void WriteLine(string message, string category) => LOG?.Debug($"{category} - {message}");

    /// <summary>
    /// Write a debug message into log
    /// </summary>
    /// <param name="o">Object to write</param>
    public override void Write(object o) => LOG?.Debug(o?.ToString() ?? "NULL");

    /// <summary>
    /// Write a debug message into log
    /// </summary>
    /// <param name="o">Object to write</param>
    /// <param name="category">Category</param>
    public override void Write(object o, string category)
    {
      LOG?.Debug($"{category} - {o?.ToString() ?? "NULL"}");
    }

    /// <summary>
    /// Write a line as debug into log
    /// </summary>
    /// <param name="o">Object to write</param>
    public override void WriteLine(object o) => LOG?.Debug(o?.ToString() ?? "NULL");

    /// <summary>
    /// Write a line as debug into log
    /// </summary>
    /// <param name="o">Object to write</param>
    /// <param name="category">Category</param>
    public override void WriteLine(object o, string category) => LOG?.Debug($"{category} - {o?.ToString() ?? "NULL"}");
  }
}
