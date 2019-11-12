using System;
using System.Windows;
using System.Windows.Controls;
using Org.Vs.TailForWin.UI.Interfaces;


namespace Org.Vs.TailForWin.Ui.Utils.Utils
{
  /// <summary>
  /// File Drag and Drop helper
  /// </summary>
  public class FileDragDropHelper
  {
    /// <summary>
    /// Is file Drag and Drop enabled
    /// </summary>
    /// <param name="sender"><see cref="DependencyObject"/></param>
    /// <returns><c>True</c> if it is enabled, otherwise <c>False</c></returns>
    public static bool GetIsFileDragDropEnabled(DependencyObject sender) => (bool) sender.GetValue(IsFileDragDropEnabledProperty);

    /// <summary>
    /// Set Drag and Drop
    /// </summary>
    /// <param name="sender"><see cref="DependencyObject"/></param>
    /// <param name="value">To enable <c>True</c>, otherwise <c>False</c></param>
    public static void SetIsFileDragDropEnabled(DependencyObject sender, bool value) => sender.SetValue(IsFileDragDropEnabledProperty, value);

    /// <summary>
    /// Gets file Drag and Drop target
    /// </summary>
    /// <param name="sender"><see cref="DependencyObject"/></param>
    /// <returns>If success <c>True</c>, otherwise <c>False</c></returns>
    public static bool GetFileDragDropTarget(DependencyObject sender) => (bool) sender.GetValue(FileDragDropTargetProperty);

    /// <summary>
    /// Set file Drag and Drop target
    /// </summary>
    /// <param name="sender"><see cref="DependencyObject"/></param>
    /// <param name="value"><see cref="bool"/></param>
    public static void SetFileDragDropTarget(DependencyObject sender, bool value) => sender.SetValue(FileDragDropTargetProperty, value);

    /// <summary>
    /// Is file Drag and Drop enabled property
    /// </summary>
    public static readonly DependencyProperty IsFileDragDropEnabledProperty = DependencyProperty.RegisterAttached("IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper), new PropertyMetadata(OnFileDragDropEnabled));

    /// <summary>
    /// File Drag and Drop target property
    /// </summary>
    public static readonly DependencyProperty FileDragDropTargetProperty = DependencyProperty.RegisterAttached("FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

    private static void OnFileDragDropEnabled(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if ( e.NewValue == e.OldValue )
        return;

      if ( sender is Control control )
        control.Drop += OnDrop;
    }

    private static void OnDrop(object sender, DragEventArgs e)
    {
      if ( !(sender is DependencyObject d) )
        return;

      var target = d.GetValue(FileDragDropTargetProperty);

      if ( target is IFileDragDropTarget fileTarget )
      {
        if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
          fileTarget.OnFileDrop((string[]) e.Data.GetData(DataFormats.FileDrop));

        e.Handled = true;
      }
      else
      {
        throw new Exception("FileDragDropTarget object must be of type IFileDragDropTarget");
      }
    }
  }
}
