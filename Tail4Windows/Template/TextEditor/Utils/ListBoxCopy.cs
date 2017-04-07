using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Template.TextEditor.Data;


namespace Org.Vs.TailForWin.Template.TextEditor.Utils
{
  /// <summary>
  /// ListBoxCopy
  /// </summary>
  public class ListBoxCopy
  {
    private static bool addDateTime;


    #region Properties

    public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached("AutoCopy", typeof(bool), typeof(ListBoxCopy), new UIPropertyMetadata(AutoCopyChanged));

    /// <summary>
    /// Get current automatically copy setting
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <returns>Returns current setting</returns>
    public static bool GetAutoCopy(DependencyObject obj)
    {
      return ((bool) obj.GetValue(AutoCopyProperty));
    }

    /// <summary>
    /// Set automatically copy
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <param name="value">Value</param>
    public static void SetAutoCopy(DependencyObject obj, bool value)
    {
      obj.SetValue(AutoCopyProperty, value);
    }

    public static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.RegisterAttached("AddDateTime", typeof(bool), typeof(ListBoxCopy), new UIPropertyMetadata(AddDateTimeChanged));

    /// <summary>
    /// Get current DateTime setting
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <returns>Returns current setting</returns>
    public static bool GetAddDateTime(DependencyObject obj)
    {
      return ((bool) obj.GetValue(AddDateTimeProperty));
    }

    /// <summary>
    /// Add additional DateTime
    /// </summary>
    /// <param name="obj">Sender</param>
    /// <param name="value">Value</param>
    public static void SetAddDateTime(DependencyObject obj, bool value)
    {
      obj.SetValue(AddDateTimeProperty, value);
    }

    #endregion

    private static void AutoCopyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var listBox = obj as ListBox;

      if(listBox != null)
      {
        if((bool) e.NewValue)
        {
          void Handler(object sender, ExecutedRoutedEventArgs arg)
          {
            if(listBox.SelectedItem != null)
            {
              var items = listBox.SelectedItems;
              StringBuilder sb = new StringBuilder();

              foreach(LogEntry item in items)
              {
                if(addDateTime)
                  sb.Append($"{item.DateTime} - {item.Message}\n");
                else
                  sb.Append(item.Message + "\n");
              }

              Clipboard.SetDataObject(sb.ToString());
            }
          }

          var command = new RoutedCommand("Copy", typeof(ListBox));
          command.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control, "Copy"));
          listBox.CommandBindings.Add(new CommandBinding(command, Handler));
        }
      }
    }

    private static void AddDateTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      addDateTime = (bool) e.NewValue;
    }
  }
}
