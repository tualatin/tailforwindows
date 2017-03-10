using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using log4net;
using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;


namespace Org.Vs.TailForWin.UI
{
  /// <summary>
  /// Interaction logic for PageTail4Windows.xaml
  /// </summary>
  public partial class PageTail4Windows : Page
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(PageTail4Windows));

    #region Properties

    /// <summary>
    /// Set ToolTip detail text
    /// </summary>
    public string ToolTipDetailText
    {
      get
      {
        return (fancyToolTipTfW.ToolTipDetail);
      }
      set
      {
        fancyToolTipTfW.ToolTipDetail = value;
      }
    }

    /// <summary>
    /// Main window taskbar icon
    /// </summary>
    public NotifyIcon.TaskbarIcon MainWndTaskBarIcon
    {
      get
      {
        return (tbIcon);
      }
    }

    /// <summary>
    /// Set statusbar state item
    /// </summary>
    public StatusBarItem StatusBarState
    {
      get
      {
        return (stsBarState);
      }
    }

    /// <summary>
    /// Set statusbar encoding item
    /// </summary>
    public StatusBarItem StatusBarEncoding
    {
      get
      {
        return (stsEncoding);
      }
    }

    /// <summary>
    /// Set statusbar lines read
    /// </summary>
    public StatusBarItem StatusBarLinesRead
    {
      get
      {
        return (stsLinesRead);
      }
    }

    /// <summary>
    /// Set statubar encode combobox (cbStsEncoding)
    /// </summary>
    public ComboBox StatusBarEncodeCb
    {
      get
      {
        return (cbStsEncoding);
      }
    }

    #endregion


    /// <summary>
    /// Standard constructor
    /// </summary>
    public PageTail4Windows()
    {
      InitializeComponent();
      InitializePage();
    }

    /// <summary>
    /// Add a control to main page
    /// </summary>
    /// <param name="control">Control</param>
    public void AddContentToPage(UIElement control)
    {
      if(control == null)
        return;

      MainContent.Children.Add(control);
    }

    #region HelperFunctions

    private void InitializePage()
    {
      SettingsHelper.ReadSettings();
      LogFile.InitObservableCollectionsRrtpfe();

      cbStsEncoding.DataContext = LogFile.FileEncoding;
      cbStsEncoding.DisplayMemberPath = "HeaderName";

      tbIcon.ToolTipText = Application.Current.FindResource("TrayIconReady") as string;
      fancyToolTipTfW.ApplicationText = LogFile.APPLICATION_CAPTION;
      tbIcon.TrayMouseDoubleClick += DoubleClickNotifyIcon;
    }

    private static void DoubleClickNotifyIcon(object sender, EventArgs e)
    {
      LogFile.BringMainWindowToFront();
    }

    #endregion
  }
}
