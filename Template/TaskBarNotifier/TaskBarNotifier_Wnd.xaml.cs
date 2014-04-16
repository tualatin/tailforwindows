using System;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows;


namespace TailForWin.Template.TaskBarNotifier
{
  /// <summary>
  /// Interaction logic for TaskBarNotifier_Wnd.xaml
  /// </summary>
  public partial class TaskBarNotifier_Wnd : TaskBarNotifier
  {
    private ObservableCollection<NotifyObject> notifyContent;


    public TaskBarNotifier_Wnd ()
    {
      InitializeComponent ( );
      lblAppName.Content = TailForWin.Data.LogFile.APPLICATION_CAPTION;
      DataContext = TailForWin.Controller.SettingsHelper.TailSettings.AlertSettings.PopupWndSettings;
    }

    public ObservableCollection<NotifyObject> NotifyContent
    {
      get
      {
        if (notifyContent == null)
          NotifyContent = new ObservableCollection<NotifyObject> ( );

        return (notifyContent);
      }
      set
      {
        notifyContent = value;
      }
    }

    private void Item_Click (object sender, EventArgs e)
    {
      Hyperlink hyperlink = sender as Hyperlink;

      if (hyperlink == null)
        return;

      NotifyObject notifyObject = hyperlink.Tag as NotifyObject;

      if (notifyObject != null)
        MessageBox.Show ("Clicked!");
    }

    private void HideButton_Click (object sender, EventArgs e)
    {
      ForceHidden ( );
    }
  }
}
