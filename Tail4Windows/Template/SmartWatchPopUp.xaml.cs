using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using log4net;
using Org.Vs.TailForWin.Data;
using Org.Vs.TailForWin.Template.Events;


namespace Org.Vs.TailForWin.Template
{
  /// <summary>
  /// Interaction logic for SmartWatchPopUp.xaml
  /// </summary>
  public partial class SmartWatchPopUp
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SmartWatchPopUp));

    /// <summary>
    /// Fires, when user accept the dialog
    /// </summary>
    public event SmartWatchOpenFileEventHandler SmartWatchOpenFile;

    /// <summary>
    /// New file
    /// </summary>
    public string NewFileOpen
    {
      get;
      set;
    }

    /// <summary>
    /// Full path of file
    /// </summary>
    public string FullPath
    {
      get;
      set;
    }


    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchPopUp()
    {
      InitializeComponent();
    }

    private void SmartWatchWnd_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        string message = string.Format(Application.Current.FindResource("SmartWatchHint").ToString(), NewFileOpen, LogFile.APPLICATION_CAPTION);
        Regex regex = new Regex($"({NewFileOpen})", RegexOptions.IgnoreCase);
        string[] substrings = regex.Split(message);

        LblNewFile.Inlines.Clear();

        Array.ForEach(substrings, item =>
        {
          if ( regex.Match(item).Success )
          {
            Brush highlightText = new SolidColorBrush(Color.FromArgb(255, 52, 180, 227));
            Run run = new Run(item)
            {
              Background = Brushes.White,
              Foreground = highlightText
            };
            LblNewFile.Inlines.Add(run);
          }
          else
          {
            LblNewFile.Inlines.Add(item);
          }
        });

        Activate();
        Focus();
        BtnOpenSameTab.Focus();
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void SmartWatchWnd_Deactivated(object sender, EventArgs e)
    {
      LOG.Trace("{0}", System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    private void BtnIgnore_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void BtnOpenSameTab_Click(object sender, RoutedEventArgs e)
    {
      SmartWatchOpenFileEventArgs args = new SmartWatchOpenFileEventArgs
      {
        FileFullPath = FullPath,
        OpenInTab = false
      };

      SmartWatchOpenFile?.Invoke(this, args);
      Close();
    }

    private void BtnOpenNewTab_Click(object sender, RoutedEventArgs e)
    {
      SmartWatchOpenFileEventArgs args = new SmartWatchOpenFileEventArgs
      {
        FileFullPath = FullPath,
        OpenInTab = true
      };

      SmartWatchOpenFile?.Invoke(this, args);
      Close();
    }
  }
}
