using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Business.Data;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// ListBoxCopy
  /// </summary>
  public class ListBoxCopy
  {
    private static bool addDateTime;

    #region Properties

    /// <summary>
    /// AutoCopy property
    /// </summary>
    public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached("AutoCopy", typeof(bool), typeof(ListBoxCopy), new UIPropertyMetadata(AutoCopyChanged));

    /// <summary>
    /// Get current automatically copy setting
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <returns>Returns current setting</returns>
    public static bool GetAutoCopy(DependencyObject obj) => (bool) obj.GetValue(AutoCopyProperty);

    /// <summary>
    /// Set automatically copy
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <param name="value">Value</param>
    public static void SetAutoCopy(DependencyObject obj, bool value) => obj.SetValue(AutoCopyProperty, value);

    /// <summary>
    /// AddDateTime property
    /// </summary>
    public static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.RegisterAttached("AddDateTime", typeof(bool), typeof(ListBoxCopy),
      new UIPropertyMetadata(AddDateTimeChanged));

    /// <summary>
    /// Get current DateTime setting
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <returns>Returns current setting</returns>
    public static bool GetAddDateTime(DependencyObject obj) => (bool) obj.GetValue(AddDateTimeProperty);

    /// <summary>
    /// Add additional DateTime
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <param name="value">Value</param>
    public static void SetAddDateTime(DependencyObject obj, bool value) => obj.SetValue(AddDateTimeProperty, value);

    #endregion

    private static void AutoCopyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if ( !(obj is ListBox listBox) )
        return;

      if ( !(bool) e.NewValue )
        return;

      void Handler(object sender, ExecutedRoutedEventArgs arg)
      {
        if ( listBox.SelectedItem == null )
          return;

        var items = listBox.SelectedItems;
        var sb = new StringBuilder();

        foreach ( LogEntry item in items )
        {
          if ( addDateTime )
            sb.Append($"{item.DateTime} - {item.Message}\n");
          else
            sb.Append(item.Message + "\n");
        }

        Clipboard.SetDataObject(sb.ToString());
      }

      var command = new RoutedCommand("Copy", typeof(ListBox));
      command.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Copy"));
      listBox.CommandBindings.Add(new CommandBinding(command, Handler));
    }

    private static void AddDateTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) => addDateTime = (bool) e.NewValue;
  }
}
