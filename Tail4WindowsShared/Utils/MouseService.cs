using System.Windows.Input;
using System.Windows.Threading;

namespace Org.Vs.Tail4Win.Shared.Utils
{
  /// <summary>
  /// Contains helper methods for UI, so far just one for showing a waitcursor
  /// </summary>
  public static class MouseService
  {
    /// <summary>
    ///   A value indicating whether the UI is currently busy
    /// </summary>
    private static bool isBusy;

    /// <summary>
    /// Sets the busystate as busy.
    /// </summary>
    public static void SetBusyState() => SetBusyState(true);

    /// <summary>
    /// Sets the busystate to busy or not busy.
    /// </summary>
    /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
    private static void SetBusyState(bool busy)
    {
      if ( busy == isBusy )
        return;

      isBusy = busy;
      Mouse.OverrideCursor = busy ? Cursors.Wait : null;

      if ( isBusy )
      {
        // ReSharper disable once ObjectCreationAsStatement
        new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, DispatcherTimerTick, System.Windows.Application.Current.Dispatcher);
      }
    }

    /// <summary>
    /// Handles the Tick event of the dispatcherTimer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private static void DispatcherTimerTick(object sender, EventArgs e)
    {
      if ( !(sender is DispatcherTimer dispatcherTimer) )
        return;

      SetBusyState(false);
      dispatcherTimer.Stop();
    }
  }
}
