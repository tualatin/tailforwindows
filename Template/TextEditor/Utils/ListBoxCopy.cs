using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text;
using TailForWin.Template.TextEditor.Data;


namespace TailForWin.Template.TextEditor.Utils
{
  public class ListBoxCopy
  {
    private static bool addDateTime;


    #region Properties
    
    public static readonly DependencyProperty AutoCopyProperty = DependencyProperty.RegisterAttached ("AutoCopy", typeof (bool), typeof (ListBoxCopy), new UIPropertyMetadata (AutoCopyChanged));

    public static bool GetAutoCopy (DependencyObject obj)
    {
      return ((bool) obj.GetValue (AutoCopyProperty));
    }    

    public static void SetAutoCopy (DependencyObject obj, bool value)
    {
      obj.SetValue (AutoCopyProperty, value);
    }

    public static readonly DependencyProperty AddDateTimeProperty = DependencyProperty.RegisterAttached ("AddDateTime", typeof (bool), typeof (ListBoxCopy), new UIPropertyMetadata (AddDateTimeChanged));

    public static bool GetAddDateTime (DependencyObject obj)
    {
      return ((bool) obj.GetValue (AddDateTimeProperty));
    }

    public static void SetAddDateTime (DependencyObject obj, bool value)
    {
      obj.SetValue (AddDateTimeProperty, value);
    }

    #endregion

    private static void AutoCopyChanged (DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var listBox = obj as ListBox;

      if (listBox != null)
      {
        if ((bool) e.NewValue)
        {
          ExecutedRoutedEventHandler handler = (sender, arg) => 
              {
                if (listBox.SelectedItem != null)
                {
                  var items = listBox.SelectedItems;
                  StringBuilder sb = new StringBuilder ( );

                  foreach (LogEntry item in items)
                  {
                    if (addDateTime)
                      sb.Append (string.Format ("{0} - {1}\n", item.DateTime, item.Message));
                    else
                      sb.Append (item.Message + "\n");
                  }

                  Clipboard.SetDataObject (sb.ToString ( ));
                }
              };

          var command = new RoutedCommand ("Copy", typeof (ListBox));
          command.InputGestures.Add (new KeyGesture (Key.C, ModifierKeys.Control, "Copy"));
          listBox.CommandBindings.Add (new CommandBinding (command, handler));
        }
      }
    }

    private static void AddDateTimeChanged (DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      addDateTime = (bool) e.NewValue;
    }
  }
}
