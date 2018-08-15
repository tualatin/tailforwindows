using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.Core.Enums;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Scrollviewer synchronizer
  /// </summary>
  public sealed class ScrollSynchronizer
  {
    #region Constant(s)

    private const string VerticalScrollGroupPropertyName = "VerticalScrollGroup";
    private const string HorizontalScrollGroupPropertyName = "HorizontalScrollGroup";
    private const string ScrollSyncTypePropertyName = "ScrollSyncType";

    #endregion

    #region Variable(s)

    private static readonly Dictionary<string, OffSetContainer> VerticalScrollGroups = new Dictionary<string, OffSetContainer>();
    private static readonly Dictionary<string, OffSetContainer> HorizontalScrollGroups = new Dictionary<string, OffSetContainer>();
    private static readonly Dictionary<ScrollViewer, EScrollSyncType> RegisteredScrollViewers = new Dictionary<ScrollViewer, EScrollSyncType>();

    #endregion

    /// <summary>
    /// Horizontal scroll group property
    /// </summary>
    public static readonly DependencyProperty HorizontalScrollGroupProperty = DependencyProperty.RegisterAttached(HorizontalScrollGroupPropertyName, typeof(string),
        typeof(ScrollSynchronizer), new PropertyMetadata(string.Empty, OnHorizontalScrollGroupChanged));

    /// <summary>
    /// Vertical scroll group property
    /// </summary>
    public static readonly DependencyProperty VerticalScrollGroupProperty = DependencyProperty.RegisterAttached(VerticalScrollGroupPropertyName, typeof(string),
        typeof(ScrollSynchronizer), new PropertyMetadata(string.Empty, OnVerticalScrollGroupChanged));

    /// <summary>
    /// Scroll sync type <see cref="EScrollSyncType"/> property
    /// </summary>
    public static readonly DependencyProperty ScrollSyncTypeProperty = DependencyProperty.RegisterAttached(ScrollSyncTypePropertyName, typeof(EScrollSyncType),
        typeof(ScrollSynchronizer), new PropertyMetadata(EScrollSyncType.None, OnScrollSyncTypeChanged));

    /// <summary>
    /// Set vertical scroll group
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <param name="verticalScrollGroup">Name of vertical scroll group</param>
    public static void SetVerticalScrollGroup(DependencyObject obj, string verticalScrollGroup) => obj.SetValue(VerticalScrollGroupProperty, verticalScrollGroup);

    /// <summary>
    /// Get vertical scroll group
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <returns>Name of vertical scroll group</returns>
    public static string GetVerticalScrollGroup(DependencyObject obj) => (string) obj.GetValue(VerticalScrollGroupProperty);

    /// <summary>
    /// Set horizontal scroll group
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <param name="horizontalScrollGroup">Name of horizontal scroll group</param>
    public static void SetHorizontalScrollGroup(DependencyObject obj, string horizontalScrollGroup) => obj.SetValue(HorizontalScrollGroupProperty, horizontalScrollGroup);

    /// <summary>
    /// Get horizontal scroll group
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <returns>Name of horizontal scroll group</returns>
    public static string GetHorizontalScrollGroup(DependencyObject obj) => (string) obj.GetValue(HorizontalScrollGroupProperty);

    /// <summary>
    /// Set scroll sync type <see cref="EScrollSyncType"/>
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <param name="scrollSyncType"><see cref="EScrollSyncType"/></param>
    public static void SetScrollSyncType(DependencyObject obj, EScrollSyncType scrollSyncType) => obj.SetValue(ScrollSyncTypeProperty, scrollSyncType);

    /// <summary>
    /// Get scroll sync type <see cref="EScrollSyncType"/>
    /// </summary>
    /// <param name="obj"><see cref="DependencyObject"/></param>
    /// <returns>Sync type of <see cref="EScrollSyncType"/></returns>
    public static EScrollSyncType GetScrollSyncType(DependencyObject obj) => (EScrollSyncType) obj.GetValue(ScrollSyncTypeProperty);

    #region Event Handler(s)

    private static void OnVerticalScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is ScrollViewer scrollViewer) )
        return;

      string newVerticalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.NewValue;
      string oldVerticalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.OldValue;

      RemoveFromVerticalScrollGroup(oldVerticalGroupName, scrollViewer);
      AddToVerticalScrollGroup(newVerticalGroupName, scrollViewer);

      EScrollSyncType currentScrollSyncValue = ReadSyncTypeDpValue(d, ScrollSyncTypeProperty);

      switch ( currentScrollSyncValue )
      {
      case EScrollSyncType.None:
      case EScrollSyncType.Horizontal:

        d.SetValue(ScrollSyncTypeProperty, EScrollSyncType.Vertical);
        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }

    private static void OnHorizontalScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is ScrollViewer scrollViewer) )
        return;

      string newHorizontalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.NewValue;
      string oldHorizontalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.OldValue;

      RemoveFromHorizontalScrollGroup(oldHorizontalGroupName, scrollViewer);
      AddToHorizontalScrollGroup(newHorizontalGroupName, scrollViewer);

      EScrollSyncType currentScrollSyncValue = ReadSyncTypeDpValue(d, ScrollSyncTypeProperty);

      switch ( currentScrollSyncValue )
      {
      case EScrollSyncType.None:

        d.SetValue(ScrollSyncTypeProperty, EScrollSyncType.Horizontal);
        break;

      case EScrollSyncType.Vertical:

        d.SetValue(ScrollSyncTypeProperty, EScrollSyncType.Both);
        break;

      default:

        throw new ArgumentOutOfRangeException();
      }
    }

    private static void OnScrollSyncTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ( !(d is ScrollViewer scrollViewer) )
        return;

      string verticalGroupName = ReadStringDpValue(d, VerticalScrollGroupProperty);
      string horizontalGroupName = ReadStringDpValue(d, HorizontalScrollGroupProperty);

      var scrollSyncType = EScrollSyncType.None;

      try
      {
        scrollSyncType = (EScrollSyncType) e.NewValue;
      }
      catch
      {
        // Nothing
      }

      switch ( scrollSyncType )
      {
      case EScrollSyncType.None:

        if ( !RegisteredScrollViewers.ContainsKey(scrollViewer) )
          return;

        RemoveFromVerticalScrollGroup(verticalGroupName, scrollViewer);
        RemoveFromHorizontalScrollGroup(horizontalGroupName, scrollViewer);
        RegisteredScrollViewers.Remove(scrollViewer);
        break;

      case EScrollSyncType.Horizontal:

        RemoveFromVerticalScrollGroup(verticalGroupName, scrollViewer);
        AddToHorizontalScrollGroup(horizontalGroupName, scrollViewer);

        if ( RegisteredScrollViewers.ContainsKey(scrollViewer) )
          RegisteredScrollViewers[scrollViewer] = EScrollSyncType.Horizontal;
        else
          RegisteredScrollViewers.Add(scrollViewer, EScrollSyncType.Horizontal);

        break;

      case EScrollSyncType.Vertical:

        RemoveFromHorizontalScrollGroup(horizontalGroupName, scrollViewer);
        AddToVerticalScrollGroup(verticalGroupName, scrollViewer);

        if ( RegisteredScrollViewers.ContainsKey(scrollViewer) )
          RegisteredScrollViewers[scrollViewer] = EScrollSyncType.Vertical;
        else
          RegisteredScrollViewers.Add(scrollViewer, EScrollSyncType.Vertical);

        break;

      case EScrollSyncType.Both:

        if ( RegisteredScrollViewers.ContainsKey(scrollViewer) )
        {
          if ( RegisteredScrollViewers[scrollViewer] == EScrollSyncType.Horizontal )
            AddToVerticalScrollGroup(verticalGroupName, scrollViewer);
          else if ( RegisteredScrollViewers[scrollViewer] == EScrollSyncType.Vertical )
            AddToHorizontalScrollGroup(horizontalGroupName, scrollViewer);

          RegisteredScrollViewers[scrollViewer] = EScrollSyncType.Both;
        }
        else
        {
          AddToHorizontalScrollGroup(horizontalGroupName, scrollViewer);
          AddToVerticalScrollGroup(verticalGroupName, scrollViewer);

          RegisteredScrollViewers.Add(scrollViewer, EScrollSyncType.Both);
        }
        break;
      }
    }

    #endregion

    private static void RemoveFromVerticalScrollGroup(string verticalGroupName, ScrollViewer scrollViewer)
    {
      if ( VerticalScrollGroups.ContainsKey(verticalGroupName) )
      {
        VerticalScrollGroups[verticalGroupName].ScrollViewers.Remove(scrollViewer);

        if ( VerticalScrollGroups[verticalGroupName].ScrollViewers.Count == 0 )
          VerticalScrollGroups.Remove(verticalGroupName);
      }

      scrollViewer.ScrollChanged -= ScrollViewerVerticalScrollChanged;
    }

    private static void AddToVerticalScrollGroup(string verticalGroupName, ScrollViewer scrollViewer)
    {
      if ( VerticalScrollGroups.ContainsKey(verticalGroupName) )
      {
        scrollViewer.ScrollToVerticalOffset(VerticalScrollGroups[verticalGroupName].Offset);
        VerticalScrollGroups[verticalGroupName].ScrollViewers.Add(scrollViewer);
      }
      else
      {
        VerticalScrollGroups.Add(verticalGroupName, new OffSetContainer
        {
          ScrollViewers = new List<ScrollViewer> { scrollViewer },
          Offset = scrollViewer.VerticalOffset
        });
      }

      scrollViewer.ScrollChanged += ScrollViewerVerticalScrollChanged;
    }

    private static void RemoveFromHorizontalScrollGroup(string horizontalGroupName, ScrollViewer scrollViewer)
    {
      if ( HorizontalScrollGroups.ContainsKey(horizontalGroupName) )
      {
        HorizontalScrollGroups[horizontalGroupName].ScrollViewers.Remove(scrollViewer);

        if ( HorizontalScrollGroups[horizontalGroupName].ScrollViewers.Count == 0 )
          HorizontalScrollGroups.Remove(horizontalGroupName);
      }

      scrollViewer.ScrollChanged -= ScrollViewerHorizontalScrollChanged;
    }

    private static void AddToHorizontalScrollGroup(string horizontalGroupName, ScrollViewer scrollViewer)
    {
      if ( HorizontalScrollGroups.ContainsKey(horizontalGroupName) )
      {
        scrollViewer.ScrollToHorizontalOffset(HorizontalScrollGroups[horizontalGroupName].Offset);
        HorizontalScrollGroups[horizontalGroupName].ScrollViewers.Add(scrollViewer);
      }
      else
      {
        HorizontalScrollGroups.Add(horizontalGroupName, new OffSetContainer
        {
          ScrollViewers = new List<ScrollViewer> { scrollViewer },
          Offset = scrollViewer.HorizontalOffset
        });
      }

      scrollViewer.ScrollChanged += ScrollViewerHorizontalScrollChanged;
    }

    private static string ReadStringDpValue(DependencyObject d, DependencyProperty dp)
    {
      object value = d.ReadLocalValue(dp);
      return value == DependencyProperty.UnsetValue ? string.Empty : value.ToString();
    }

    private static EScrollSyncType ReadSyncTypeDpValue(DependencyObject d, DependencyProperty dp)
    {
      object value = d.ReadLocalValue(dp);
      return value == DependencyProperty.UnsetValue ? EScrollSyncType.None : (EScrollSyncType) value;
    }

    #region Event Handler(s)

    private static void ScrollViewerVerticalScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( !(sender is ScrollViewer changedScrollViewer) )
        return;

      if ( e.VerticalChange.Equals(0) )
        return;

      string verticalScrollGroup = ReadStringDpValue((DependencyObject) sender, VerticalScrollGroupProperty);

      if ( !VerticalScrollGroups.ContainsKey(verticalScrollGroup) )
        return;

      VerticalScrollGroups[verticalScrollGroup].Offset = changedScrollViewer.VerticalOffset;

      foreach ( ScrollViewer scrollViewer in VerticalScrollGroups[verticalScrollGroup].ScrollViewers )
      {
        if ( scrollViewer.VerticalOffset.Equals(changedScrollViewer.VerticalOffset) )
          continue;

        scrollViewer.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
      }
    }

    private static void ScrollViewerHorizontalScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if ( !(sender is ScrollViewer changedScrollViewer) )
        return;

      if ( e.HorizontalChange.Equals(0) )
        return;

      string horizontalScrollGroup = ReadStringDpValue((DependencyObject) sender, HorizontalScrollGroupProperty);

      if ( !HorizontalScrollGroups.ContainsKey(horizontalScrollGroup) )
        return;

      HorizontalScrollGroups[horizontalScrollGroup].Offset = changedScrollViewer.HorizontalOffset;

      foreach ( ScrollViewer scrollViewer in HorizontalScrollGroups[horizontalScrollGroup].ScrollViewers )
      {
        if ( scrollViewer.HorizontalOffset.Equals(changedScrollViewer.HorizontalOffset) )
          continue;

        scrollViewer.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
      }
    }

    #endregion

    #region OffSetContainer class

    private class OffSetContainer
    {
      /// <summary>
      /// Offset
      /// </summary>
      public double Offset
      {
        get;
        set;
      }

      /// <summary>
      /// <see cref="List{T}"/> of <see cref="ScrollViewer"/>
      /// </summary>
      public List<ScrollViewer> ScrollViewers
      {
        get;
        set;
      }
    }

    #endregion
  }
}
