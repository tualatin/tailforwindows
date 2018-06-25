using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using log4net;
using Org.Vs.TailForWin.Business.Services.Data;
using Org.Vs.TailForWin.UI.Extensions;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule.LogWindowUserControl.Behaviors
{
  /// <summary>
  /// <see cref="LogEntry"/> scrollbehavior
  /// </summary>
  public class LogEntryScrollBehavior : Behavior<LogWindowListBox>
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(LogEntryScrollBehavior));

    /// <summary>
    /// This stores the <see cref="LogEntryScrollBehavior"/> for each GridSplitter so we can unregister it.
    /// </summary>
    private static readonly Dictionary<LogWindowListBox, LogEntryScrollBehavior> AttachedControls = new Dictionary<LogWindowListBox, LogEntryScrollBehavior>();
    private readonly LogWindowListBox _listBox;
    private ScrollViewer _scrollViewer;
    private SplitWindowControl _splitWindow;
    private double _currentHeight;

    /// <summary>
    /// Identifies the IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(LogEntryScrollBehavior),
      new UIPropertyMetadata(false, IsEnabledChanged));

    /// <summary>
    /// Gets the value of the IsEnabled attached property that indicates
    /// whether a selection rectangle can be used to select items or not.
    /// </summary>
    /// <param name="obj">Object on which to get the property.</param>
    /// <returns>
    /// true if items can be selected by a selection rectangle; otherwise, false.
    /// </returns>
    public static bool GetEnabled(DependencyObject obj) => (bool) obj.GetValue(EnabledProperty);

    /// <summary>
    /// Sets the value of the IsEnabled attached property that indicates
    /// whether a selection rectangle can be used to select items or not.
    /// </summary>
    /// <param name="obj">Object on which to set the property.</param>
    /// <param name="value">Value to set.</param>
    public static void SetEnabled(DependencyObject obj, bool value) => obj.SetValue(EnabledProperty, value);

    private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is LogWindowListBox listBox) )
        return;

      if ( (bool) e.NewValue )
      {
        AttachedControls.Add(listBox, new LogEntryScrollBehavior(listBox));
      }
      else
      {
        if ( !AttachedControls.TryGetValue(listBox, out var scrollBehavior) )
          return;

        AttachedControls.Remove(listBox);
        scrollBehavior.Unregister();
      }
    }

    private LogEntryScrollBehavior(LogWindowListBox listBox)
    {
      _listBox = listBox;

      if ( _listBox.IsLoaded )
      {
        InitializeComponents();
        Register();
      }
      else
      {
        _listBox.Loaded += OnLoaded;
      }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      InitializeComponents();
      Register();

      _listBox.Loaded -= OnLoaded;
    }

    private void Register() => _listBox.SizeChanged += OnSizeChanged;


    private void InitializeComponents()
    {
      _scrollViewer = _listBox.Descendents().OfType<ScrollViewer>().FirstOrDefault();
      _splitWindow = _listBox.Ancestors().OfType<SplitWindowControl>().FirstOrDefault();


    }

    private void Unregister() => _listBox.SizeChanged -= OnSizeChanged;

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      var listBoxPair = AttachedControls.FirstOrDefault(p => Equals(p.Key, sender as LogWindowListBox));
      listBoxPair.Value._currentHeight = e.NewSize.Height;

      double height = GetControlTextHeight(listBoxPair.Key);
      double totalHeight = GetTotalHeight(listBoxPair.Value._splitWindow.LogEntries.Count, height);
      int index = (int) Math.Round(listBoxPair.Value._currentHeight / height, 0, MidpointRounding.ToEven);

      //var logEntries = listBoxPair.Value._splitWindow.LogEntries.Take(index + 1).ToList();
      //listBoxPair.Value._listBox.ItemsSource = logEntries;
    }

    private double GetControlTextHeight(Control listBox) => Math.Ceiling(listBox.FontSize * listBox.FontFamily.LineSpacing);

    private double GetTotalHeight(int itemsCount, double height)
    {
      double totalHeight = 0;

      for ( int i = 0; i < itemsCount; i++ )
      {
        totalHeight += height;
      }
      return totalHeight;
    }
  }
}
