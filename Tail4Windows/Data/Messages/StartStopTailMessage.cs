using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.GlobalHighlightModule.Enums;

namespace Org.Vs.TailForWin.Data.Messages
{
  public class StartStopTailMessage
  {
    /// <summary>
    /// Current tail state
    /// </summary>
    public EGlobalFilterState CurrentState
    {
      get;
    }

    public StartStopTailMessage(EGlobalFilterState state) => CurrentState = state;
  }
}
