using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.Behaviors
{
  /// <summary>
  /// LogEntry items
  /// </summary>
  public class LogEntryItems : NotifyMaster
  {
    /// <summary>
    /// This stores the <see cref="LogEntryItems"/> for each LogEntry so we can unregister it.
    /// </summary>
    private static readonly Dictionary<SplitWindowControl, LogEntryItems> AttachedControls = new Dictionary<SplitWindowControl, LogEntryItems>();

    private readonly ObservableCollection<LogEntry> _logEntries;

    /// <summary>
    /// LogEntries property
    /// </summary>
    public static readonly DependencyProperty LogEntriesProperty = DependencyProperty.RegisterAttached("LogEntries", typeof(ObservableCollection<LogEntry>),
      typeof(LogEntryItems), new FrameworkPropertyMetadata(new ObservableCollection<LogEntry>(), LogEntriesChangedCallback));

    /// <summary>
    /// Get LogEntries value
    /// </summary>
    /// <param name="obj">Dependency object</param>
    /// <returns>LogEntries</returns>
    public static ObservableCollection<LogEntry> GetLogEntries(DependencyObject obj) => (ObservableCollection<LogEntry>) obj.GetValue(LogEntriesProperty);

    /// <summary>
    /// Set LogEntries vlaue
    /// </summary>
    /// <param name="obj">Dependency object</param>
    /// <param name="value">Value</param>
    public static void SetLogEntries(DependencyObject obj, ObservableCollection<LogEntry> value) => obj.SetValue(LogEntriesProperty, value);

    private static void LogEntriesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is SplitWindowControl splitWindow) )
        return;

      if ( e.NewValue is ObservableCollection<LogEntry> entries )
      {
        AttachedControls.Add(splitWindow, new LogEntryItems(entries));
      }
      else
      {
        if ( !AttachedControls.TryGetValue(splitWindow, out var entry) )
          return;

        AttachedControls.Remove(splitWindow);
        entry.Unregister();
      }
    }

    private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      foreach ( var attachedControl in AttachedControls )
      {
        if ( attachedControl.Value._logEntries == sender as ObservableCollection<LogEntry> )
        {
          var test = attachedControl.Value._logEntries;

          switch ( e.Action )
          {
          case NotifyCollectionChangedAction.Add:

            break;

          case NotifyCollectionChangedAction.Remove:

            break;

          case NotifyCollectionChangedAction.Reset:

            //entries.Clear();
            break;
          }
        }
      }
    }

    private LogEntryItems(ObservableCollection<LogEntry> entries)
    {
      _logEntries = entries;
      //Register();
    }

    private void Register() => _logEntries.CollectionChanged += OnCollectionChanged;

    private void Unregister() => _logEntries.CollectionChanged -= OnCollectionChanged;
  }
}
