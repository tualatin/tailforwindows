using System.Reflection;
using System.Windows;
using log4net;

namespace Org.Vs.Tail4Win.Shared.Utils
{
  /// <summary>
  /// Single instance service
  /// </summary>
  public class SingleInstance : IDisposable
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SingleInstance));

    /// <summary>
    /// Argument handler
    /// </summary>
    /// <param name="args"></param>
    public delegate void ArgsHandler(string[] args);

    /// <summary>
    /// Arguments received
    /// </summary>
    public event ArgsHandler ArgsReceived;

    private readonly Guid _appGuid;
    private readonly Mutex _mutex;
    private bool _owned;
    private Window _window;

    private class Bridge
    {
      public event Action<Guid> BringToFront;
      public event Action<Guid, string[]> ProcessArgs;

      /// <summary>
      /// Bring to front
      /// </summary>
      /// <param name="appGuid"><see cref="Guid"/></param>
      public void OnBringToFront(Guid appGuid) => BringToFront?.Invoke(appGuid);

      /// <summary>
      /// On process arguments
      /// </summary>
      /// <param name="appGuid"><see cref="Guid"/></param>
      /// <param name="args">Arguments</param>
      public void OnProcessArgs(Guid appGuid, string[] args) => ProcessArgs?.Invoke(appGuid, args);

      static Bridge()
      {
      }

      /// <summary>
      /// Instance
      /// </summary>
      public static Bridge Instance
      {
        get;
      } = new Bridge();
    }

    private class RemotableObject : MarshalByRefObject
    {
      /// <summary>
      /// Bring to fron
      /// </summary>
      /// <param name="appGuid"><see cref="Guid"/></param>
      public void BringToFront(Guid appGuid) => Bridge.Instance.OnBringToFront(appGuid);

      /// <summary>
      /// Process arguments
      /// </summary>
      /// <param name="appGuid"><see cref="Guid"/></param>
      /// <param name="args">Arguments</param>
      public void ProcessArguments(Guid appGuid, string[] args) => Bridge.Instance.OnProcessArgs(appGuid, args);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="appGuid"><see cref="Guid"/></param>
    public SingleInstance(Guid appGuid)
    {
      _appGuid = appGuid;
      var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

      Bridge.Instance.BringToFront += BringToFront;
      Bridge.Instance.ProcessArgs += ProcessArgs;

      _mutex = new Mutex(true, assemblyName + _appGuid, out _owned);
    }

    /// <summary>
    /// Release all resources used by <see cref="SingleInstance"/>
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if ( !disposing )
        return;

      if ( _owned ) // always release a mutex if you own it
      {
        _owned = false;
        _mutex.ReleaseMutex();
      }
    }

    /// <summary>
    /// Run application
    /// </summary>
    /// <param name="showWindow">Show window</param>
    /// <param name="args">Arguments</param>
    public void Run(Func<Window> showWindow, string[] args)
    {
      if ( _owned )
      {
        // show the main app window
        _window = showWindow();
        // and start the service
        StartService();
        ProcessArgs(_appGuid, args);
      }
      else
      {
        SendCommandLineArgs(args);
        Application.Current.Shutdown();
      }
    }

    private void StartService()
    {
      try
      {
        IpcServerChannel channel = new IpcServerChannel("pvp");
        ChannelServices.RegisterChannel(channel, false);

        RemotingConfiguration.RegisterActivatedServiceType(typeof(RemotableObject));
      }
      catch
      {
        // log it
      }
    }

    private void BringToFront(Guid appGuid)
    {
      if ( appGuid == _appGuid )
      {
        _window.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
          _window.Show();

          if ( _window.WindowState == WindowState.Minimized )
            _window.WindowState = WindowState.Normal;

          _window.Activate();
          _window.Focus();
        });
      }
    }

    private void ProcessArgs(Guid appGuid, string[] args)
    {
      if ( appGuid == _appGuid && ArgsReceived != null )
      {
        _window.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
          ArgsReceived(args);
        });
      }
    }

    private void SendCommandLineArgs(string[] args)
    {
      try
      {
        IpcClientChannel channel = new IpcClientChannel();

        ChannelServices.RegisterChannel(channel, false);
        RemotingConfiguration.RegisterActivatedClientType(typeof(RemotableObject), "ipc://pvp");

        RemotableObject proxy = new RemotableObject();

        proxy.BringToFront(_appGuid);
        proxy.ProcessArguments(_appGuid, args);
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", MethodBase.GetCurrentMethod()?.Name, ex.GetType().Name);
      }
    }
  }
}
