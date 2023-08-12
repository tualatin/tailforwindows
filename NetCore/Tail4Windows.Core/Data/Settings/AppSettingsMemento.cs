using Org.Vs.Tail4Win.Core.Utils;

namespace Org.Vs.Tail4Win.Core.Data.Settings
{
  /// <summary>
  /// App wide settings
  /// </summary>
  public partial class AppSettings
  {
    /// <summary>
    /// Save data to memento
    /// </summary>
    /// <returns>Copy of <see cref="EnvironmentSettings"/></returns>
    public AppSettingsMemento SaveToMemento() => new AppSettingsMemento(this);

    /// <summary>
    /// Roll object back to state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    public void RestoreFromMemento(AppSettingsMemento memento)
    {
      Arg.NotNull(memento, nameof(memento));

      IsPortable = memento.IsPortable;
    }

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class AppSettingsMemento
    {
      /// <summary>
      /// Internal constructor
      /// </summary>
      /// <param name="obj"><see cref="AppSettings"/></param>
      internal AppSettingsMemento(AppSettings obj) => IsPortable = obj.IsPortable;

      /// <summary>
      /// Save settings in user roaming path or use it as portable app
      /// </summary>
      public bool IsPortable
      {
        get;
      }
    }
  }
}
