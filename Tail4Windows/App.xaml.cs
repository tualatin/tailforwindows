using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using log4net;
using Org.Vs.TailForWin.BaseView;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.UserControls.DragSupportUtils;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(App));

    private readonly Guid _tail4WindowsGuid = new Guid("1c0c2cfa-add6-4b66-8c1d-6416f73f2046");

    private string[] _args;
    private IXmlFileManager _xmlFileManagerController;
    private Guid _itemId;

    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
      // If it is not the minimum .NET version installed
      if ( EnvironmentContainer.NetFrameworkKey <= 393295 )
      {
        LOG.Error("Wrong .NET version! Please install .NET 4.6 or newer.");
        Shutdown();
        return;
      }

      if ( e.Args.Length > 0 )
        _args = e.Args;

      AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
      NotifyTaskCompletion.Create(EnvironmentContainer.Instance.ReadSettingsAsync()).PropertyChanged += OnReadSettingsPropertyChanged;
    }

    private void OnReadSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( SettingsHelperController.CurrentSettings.SingleInstance )
      {
        var instance = new SingleInstance(_tail4WindowsGuid);
        instance.ArgsReceived += OnArgsReceived;

        instance.Run(() =>
        {
          new T4Window().Show();
          return MainWindow;
        }, _args);

        return;
      }

      new T4Window().Show();
      OnArgsReceived(_args);
    }

    private void OnArgsReceived(string[] args)
    {
      if ( args == null || args.Length <= 0 )
        return;

      string arg = args.FirstOrDefault();

      if ( string.IsNullOrWhiteSpace(arg) )
        return;

      Match match = Regex.Match(arg, @"/id=");

      if ( match.Success )
      {
        try
        {
          // Argument is an ID from TailManager
          OpenTailManagerEntryById(arg.Substring("/id=".Length));
        }
        catch
        {
          // Nothing
        }
      }
      else
      {
        try
        {
          // Argument is a FileName
          var regex = new Regex(@"(?:(?:(?:\b[a-z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)\\|\\?[^\\/:*?""<>|\r\n]+\\?)(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
          match = regex.Match(arg);

          if ( match.Success )
            OpenFileByName(arg);
        }
        catch
        {
          // Nothing
        }
      }
    }

    private void OpenTailManagerEntryById(string guid)
    {
      Match id = Regex.Match(guid, @"^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$");

      if ( !id.Success )
        return;

      _itemId = Guid.Parse(id.Value);
      _xmlFileManagerController = new XmlFileManagerController();
      NotifyTaskCompletion.Create(_xmlFileManagerController.ReadXmlFileAsync(new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token)).PropertyChanged += OnReadXmlFilePropertyChanged;
    }

    private void OnReadXmlFilePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( !(sender is NotifyTaskCompletion<ObservableCollection<TailData>> task) )
        return;

      if ( task.Result == null || task.Result.Count <= 0 )
        return;

      NotifyTaskCompletion.Create(_xmlFileManagerController.GetTailDataByIdAsync(task.Result, _itemId)).PropertyChanged += OnGetTailDataByIdPropertyChanged;
    }

    private void OnGetTailDataByIdPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( !e.PropertyName.Equals("IsSuccessfullyCompleted") )
        return;

      if ( !(sender is NotifyTaskCompletion<TailData> task) )
        return;

      _xmlFileManagerController = null;
      _itemId = Guid.Empty;

      if ( task.Result == null )
        return;

      DragSupportTabItem firstTab = BusinessHelper.TabItemList.FirstOrDefault();

      if ( !(firstTab?.Content is ILogWindowControl myControl) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataMessage(this, task.Result, myControl.ParentWindowId, false));
    }

    private void OpenFileByName(string fileName)
    {
      if ( !File.Exists(fileName) )
        return;

      new ThrottledExecution().InMs(500).Do(() =>
      {
        Current.Dispatcher.InvokeAsync(() =>
        {
          DragSupportTabItem firstTab = BusinessHelper.TabItemList.FirstOrDefault();

          if ( !(firstTab?.Content is ILogWindowControl myControl) )
            return;

          myControl.SelectedItem = fileName;
        });
      });
    }

    private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) =>
      LOG.Error("{0} caused a(n) {1} {2}", System.Reflection.MethodBase.GetCurrentMethod().Name, e.ExceptionObject.GetType().Name, e.ExceptionObject);
  }
}
