using System.Windows.Forms;
using System;
using System.Drawing;


namespace TailForWin.Utils
{
  public class WinTrayIcon : IDisposable
  {
    private NotifyIcon notifyIcon;
    private ContextMenu contextMenu = new ContextMenu ( );


    public void Dispose ()
    {
      if (notifyIcon == null)
        return;

      notifyIcon.Dispose ( );
      notifyIcon = null;

      if (contextMenu == null)
        return;

      contextMenu.Dispose ( );
      contextMenu = null;
    }

    public WinTrayIcon (Icon icon, string iconText)
    {
      try
      {
        Init (icon, iconText);
      }
      catch (Exception ex)
      {
#if DEBUG
        ErrorLog.WriteLog (ErrorFlags.Debug, "WinTrayIcon Constructor", string.Format ("Exception: {0}", ex));
#endif
      }
    }

    private void Init (Icon icon, string text)
    {
      notifyIcon = new NotifyIcon ( )
      {
        Visible = true,
        ContextMenu = contextMenu,
        Icon = icon,
        Text = text,
      };
    }

    public void CreateMenuItem (string name)
    {
      MenuItem menuItem = new MenuItem ( )
      {
        Index = 1,
        Name = name,
        Text = "&" + name
      };

      contextMenu.MenuItems.Add (menuItem);
    }

    public void CreateMenuItem (string name, bool check)
    {
      MenuItem menuItem = new MenuItem ( )
      {
        Index = 2,
        Name = name,
        Text = "&" + name,
        Checked = check
      };

      menuItem.Click += (sender, e) =>
      {
        MenuItem m = sender as MenuItem;
        m.Checked = !m.Checked;
      };

      contextMenu.MenuItems.Add (menuItem);
    }

    public NotifyIcon NotifyIcon
    {
      get
      {
        return (notifyIcon);
      }
    }
  }
}
