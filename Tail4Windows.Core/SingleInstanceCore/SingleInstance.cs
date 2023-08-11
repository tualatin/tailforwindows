using System.Reflection;
using TinyIpc.Messaging;

namespace Org.Vs.Tail4Win.Core.SingleInstanceCore
{
  /// <summary>
  /// See https://github.com/soheilkd/SingleInstanceCore
  /// </summary>
  public static class SingleInstance
  {
    private const string ChannelNameSuffix = ":SingeInstanceIPCChannel";

    //For detecting if mutex is locked (first instance is already up then)
    private static Mutex singleMutex;

    //Message bus for communication between instances
    private static TinyMessageBus messageBus;

    /// <summary>
    /// Intended to be on app startup
    /// Initializes service if the call is from first instance.
    /// Signals the first instance if it already exists
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="appGuid">A unique name for IPC channel</param>
    /// <returns>Whether the call is from application's first instance</returns>
    public static bool InitializeAsFirstInstance<T>(this T instance, Guid appGuid) where T : ISingleInstance
    {
      var commandLineArgs = GetCommandLineArgs();
      var applicationIdentifier = $"{Assembly.GetEntryAssembly()?.GetName().Name}{appGuid}";
      var channelName = $"{applicationIdentifier}{ChannelNameSuffix}";
      singleMutex = new Mutex(true, applicationIdentifier, out var firstInstance);

      if ( firstInstance )
        CreateRemoteService(instance, channelName);
      else
        SignalFirstInstance(channelName, commandLineArgs);

      return firstInstance;
    }

    private static void SignalFirstInstance(string channelName, IList<string> commandLineArgs)
    {
      var bus = GetTinyMessageBus(channelName);
      var serializedArgs = commandLineArgs.Serialize();
      bus?.PublishAsync(serializedArgs).Wait();

      WaitTillMessageGetsPublished(bus);
    }

    private static TinyMessageBus GetTinyMessageBus(string channelName, int tryCount = 50)
    {
      int tries = 0;
      var minMessageAge = TimeSpan.FromSeconds(30);

      while ( tries++ < tryCount )
      {
        try
        {
          var bus = new TinyMessageBus(channelName, minMessageAge);

          return bus;
        }
        catch
        {
          // Nothing
        }
      }
      return null;
    }

    private static void WaitTillMessageGetsPublished(ITinyMessageBus bus)
    {
      if ( bus == null )
        return;

      while ( bus.MessagesPublished != 1 )
      {
        Thread.Sleep(10);
      }
    }

    private static void CreateRemoteService(ISingleInstance instance, string channelName)
    {
      messageBus = new TinyMessageBus(channelName);
      messageBus.MessageReceived += (_, e) =>
      {
        instance.OnInstanceInvoked(e.Message.Select(p => p).ToArray().Deserialize<string[]>());
      };
    }

    private static string[] GetCommandLineArgs()
    {
      var args = Environment.GetCommandLineArgs();

      return args;
    }

    public static void Cleanup()
    {
      if ( messageBus != null )
      {
        messageBus.Dispose();
        messageBus = null;
      }

      if ( singleMutex == null )
        return;

      singleMutex.Close();
      singleMutex = null;
    }
  }
}
