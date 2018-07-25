using System;
using System.Windows;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.UI.Behaviors.Base
{
  /// <summary>
  /// BehaviorBase
  /// </summary>
  /// <typeparam name="T">Type of behavior</typeparam>
  public abstract class BehaviorBase<T> : Behavior<T> where T : FrameworkElement
  {
    private bool _isSetup;
    private bool _isHookedUp;
    private WeakReference _weakTarget;

    /// <summary>
    /// Setup <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected abstract void OnSetup();

    /// <summary>
    /// Release all resource used by <see cref="BehaviorBase{T}"/>
    /// </summary>
    protected abstract void OnCleanup();

    /// <summary>
    /// <see cref="BehaviorBase{T}"/> changed
    /// </summary>
    protected override void OnChanged()
    {
      var target = AssociatedObject;

      if ( target != null )
        HookupBehavior(target);
      else
        UnHookupBehavior();
    }

    private void OnTargetLoaded(object sender, RoutedEventArgs e) => SetupBehavior();

    private void OnTargetUnloaded(object sender, RoutedEventArgs e) => CleanupBehavior();

    private void HookupBehavior(T target)
    {
      if ( _isHookedUp )
        return;

      _weakTarget = new WeakReference(target);
      _isHookedUp = true;

      target.Unloaded += OnTargetUnloaded;
      target.Loaded += OnTargetLoaded;

      SetupBehavior();
    }

    private void UnHookupBehavior()
    {
      if ( !_isHookedUp )
        return;

      _isHookedUp = false;
      var target = AssociatedObject ?? (T) _weakTarget.Target;

      if ( target != null )
      {
        target.Unloaded -= OnTargetUnloaded;
        target.Loaded -= OnTargetLoaded;
      }

      CleanupBehavior();
    }

    private void SetupBehavior()
    {
      if ( _isSetup )
        return;

      _isSetup = true;

      OnSetup();
    }

    private void CleanupBehavior()
    {
      if ( !_isSetup )
        return;

      _isSetup = false;

      OnCleanup();
    }
  }
}
