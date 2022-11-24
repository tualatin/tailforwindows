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
using Org.Vs.TailForWin.Business.Utils.Interfaces;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule;
using Org.Vs.TailForWin.Controllers.PlugIns.FileManagerModule.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.Data.Messages;
using Org.Vs.TailForWin.PlugIns.LogWindowModule.Interfaces;
using Org.Vs.TailForWin.UI.Utils;


namespace Org.Vs.TailForWin
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(App));

    private readonly Guid _tail4WindowsGuid = new Guid("a741233e-f81e-4924-a373-cb4f280bc10b");

    private string[] _args;
    private IFileManagerController _fileManagerController;
    private Guid _itemId;

    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
      // If it is not the minimum .NET version installed
      if ( EnvironmentContainer.NetFrameworkKey < 528040 )
      {
        InteractionService.ShowInformationMessageBox(string.Format(Current.TryFindResource("ApplicationWrongNetFx").ToString(), Current.TryFindResource("ApplicationTitle")));
        LOG.Error("Wrong .NET version! Please install .NET 4.8 or newer.");
        Shutdown(-1);
        return;
      }

      if ( e.Args.Length > 0 )
      {
        _args = e.Args;

        if ( _args.Contains("/convert") )
        {
          ConvertProfile();
          return;
        }
      }

      AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
      NotifyTaskCompletion.Create(EnvironmentContainer.Instance.ReadSettingsAsync()).PropertyChanged += OnReadSettingsPropertyChanged;
    }

    private void ConvertProfile()
    {
      IProfileConverter converter = new ProfileConverter();

      if ( _args.Contains("local") )
      {
        ConvertToLocalProfile(converter);
      }
      else if ( _args.Contains("roaming") )
      {
        ConvertToRoamingProfile(converter);
      }
      else
      {
        InteractionService.ShowErrorMessageBox(Current.TryFindResource("ConvertProfileWrongParameter").ToString());
        Shutdown(-1);
      }
    }

    private void ConvertToLocalProfile(IProfileConverter converter) =>
      NotifyTaskCompletion.Create(converter.ConvertIntoLocalProfileAsync()).PropertyChanged += OnConvertProfilePropertyChanged;

    private void ConvertToRoamingProfile(IProfileConverter converter) =>
      NotifyTaskCompletion.Create(converter.ConvertIntoRoamingProfileAsync).PropertyChanged += OnConvertProfilePropertyChanged;

    private void OnConvertProfilePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      Shutdown(0);
    }

    private void OnReadSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      if ( SettingsHelperController.CurrentSettings.SingleInstance )
      {
        using ( var instance = new SingleInstance(_tail4WindowsGuid) )
        {
          instance.ArgsReceived += OnArgsReceived;
          instance.Run(() =>
          {
            new T4Window().Show();
            return MainWindow;
          }, _args);
        }

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

      var match = Regex.Match(arg, @"/id=");

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
      var id = Regex.Match(guid, @"^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$");

      if ( !id.Success )
        return;

      _itemId = Guid.Parse(id.Value);
      _fileManagerController = new FileManagerController();

      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
      {
        NotifyTaskCompletion.Create(
         _fileManagerController.ReadJsonFileAsync(cts.Token)).PropertyChanged += OnReadXmlFilePropertyChanged;
      }
    }

    private void OnReadXmlFilePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      if ( !(sender is NotifyTaskCompletion<ObservableCollection<TailData>> task) )
        return;

      if ( task.Result == null || task.Result.Count <= 0 )
        return;

      using ( var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)) )
      {
        NotifyTaskCompletion.Create(
          _fileManagerController.GetTailDataByIdAsync(task.Result, _itemId, cts.Token)).PropertyChanged += OnGetTailDataByIdPropertyChanged;
      }
    }

    private void OnGetTailDataByIdPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ( e.PropertyName != nameof(NotifyTaskCompletion.IsSuccessfullyCompleted) )
        return;

      if ( !(sender is NotifyTaskCompletion<TailData> task) )
        return;

      _fileManagerController = null;
      _itemId = Guid.Empty;

      if ( task.Result == null )
        return;

      var firstTab = UiHelper.TabItemList.FirstOrDefault();

      if ( !(firstTab?.Content is ILogWindowControl myControl) )
        return;

      EnvironmentContainer.Instance.CurrentEventManager.SendMessage(new OpenTailDataMessage(this, task.Result, myControl.ParentWindowId, myControl.WindowId, false));
    }

    private void OpenFileByName(string fileName)
    {
      if ( !File.Exists(fileName) )
        return;

      using ( var execute = new ThrottledExecution() )
      {
        execute.InMs(500).Do(() =>
        {
          Current.Dispatcher?.Invoke(() =>
          {
            var firstTab = UiHelper.TabItemList.FirstOrDefault();

            if ( !(firstTab?.Content is ILogWindowControl myControl) )
              return;

            myControl.SelectedItem = fileName;
          });

        });
      }
    }

    private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) =>
      LOG.Error("{0} caused a(n) {1} {2}", System.Reflection.MethodBase.GetCurrentMethod()?.Name, e.ExceptionObject.GetType().Name, e.ExceptionObject);
  }
}
