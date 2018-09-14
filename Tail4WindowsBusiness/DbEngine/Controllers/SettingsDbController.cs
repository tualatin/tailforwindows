using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using log4net;
using Org.Vs.TailForWin.Business.Data;
using Org.Vs.TailForWin.Business.DbEngine.Data;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Settings;


namespace Org.Vs.TailForWin.Business.DbEngine.Controllers
{
  /// <summary>
  /// Settings DataBase controller
  /// </summary>
  public class SettingsDbController : ISettingsDbController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SettingsDbController));
    private static readonly object DbLock = new object();

    /// <summary>
    /// Current lock time span in milliseconds
    /// </summary>
    private const int LockTimeSpanIsMs = 200;

    /// <summary>
    /// Work width
    /// </summary>
    private readonly double _workWidth = System.Windows.SystemParameters.WorkArea.Width;

    /// <summary>
    /// Work height
    /// </summary>
    private readonly double _workHeight = System.Windows.SystemParameters.WorkArea.Height;

    #region Setting keys

    private const string Settings = "Settings";

    private const string FindResultWindowTop = "FindResultWndTop";
    private const string FindResultWindowLeft = "FindResultWndLeft";
    private const string FindResultWindowHeight = "FindResultWndHeight";
    private const string FindResultWindowWidth = "FindResultWndWidth";

    private const string FindDialogWindowTop = "FindDialogWndTop";
    private const string FindDialogWindowLeft = "FindDialogWndLeft";

    private const string BookmarkOverviewWindowTop = "BookmarkOverviewWndTop";
    private const string BookmarkOverviewWindowLeft = "BookmarkOverviewWndLeft";
    private const string BookmarkOverviewWindowHeight = "BookmarkOverviewWndHeight";
    private const string BookmarkOverviewWindowWidth = "BookmarkOverivewWndWidth";

    private const string ProxyPassword = "Proxy.Password";
    private const string SmtpPassword = "Smtp.Password";

    #endregion

    #region Singleton pattern

    private static SettingsDbController instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static SettingsDbController Instance => instance ?? (instance = new SettingsDbController());

    private SettingsDbController()
    {
    }

    #endregion

    /// <summary>
    /// Read current DataBase settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task ReadDbSettingsAsync()
    {
      await Task.Run(
        () =>
        {
          if ( Monitor.TryEnter(DbLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
          {
            try
            {
              using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
              {
                var settings = db.GetCollection<DatabaseSetting>(Settings);

                if ( settings.Count() == 0 )
                  AddSettingsToDataBase(settings);

                AddMissingDbSettings(settings);

                settings = db.GetCollection<DatabaseSetting>(Settings);
                settings.EnsureIndex(p => p.Key);

                // FindResult position
                var setting = settings.Find(p => p.Key == FindResultWindowTop).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindResultPositionX = Convert.ToDouble(setting?.Value);
                setting = settings.Find(p => p.Key == FindResultWindowLeft).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindResultPositionY = Convert.ToDouble(setting?.Value);

                // FindResult size
                setting = settings.Find(p => p.Key == FindResultWindowHeight).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindResultHeight = Convert.ToDouble(setting?.Value);
                setting = settings.Find(p => p.Key == FindResultWindowWidth).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindResultWidth = Convert.ToDouble(setting?.Value);

                // Bookmark overview position
                setting = settings.Find(p => p.Key == BookmarkOverviewWindowTop).FirstOrDefault();
                SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX = Convert.ToDouble(setting?.Value);
                setting = settings.Find(p => p.Key == BookmarkOverviewWindowLeft).FirstOrDefault();
                SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY = Convert.ToDouble(setting?.Value);

                // Bookmark overview size
                setting = settings.Find(p => p.Key == BookmarkOverviewWindowHeight).FirstOrDefault();
                SettingsHelperController.CurrentSettings.BookmarkOverviewHeight = Convert.ToDouble(setting?.Value);
                setting = settings.Find(p => p.Key == BookmarkOverviewWindowWidth).FirstOrDefault();
                SettingsHelperController.CurrentSettings.BookmarkOverviewWidth = Convert.ToDouble(setting?.Value);

                // FindDialog position
                setting = settings.Find(p => p.Key == FindDialogWindowTop).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindDialogPositionX = Convert.ToDouble(setting?.Value);
                setting = settings.Find(p => p.Key == FindDialogWindowLeft).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindDialogPositionY = Convert.ToDouble(setting?.Value);

                // Proxy password
                setting = settings.Find(p => p.Key == ProxyPassword).FirstOrDefault();
                SettingsHelperController.CurrentSettings.ProxySettings.Password = setting?.Value;

                // SMTP password
                setting = settings.Find(p => p.Key == SmtpPassword).FirstOrDefault();
                SettingsHelperController.CurrentSettings.SmtpSettings.Password = setting?.Value;
              }
            }
            catch ( Exception ex )
            {
              LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
            }
            finally
            {
              Monitor.Exit(DbLock);
            }
          }
          else
          {
            LOG.Error("Can not lock!");
          }
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Resets DataBase settings
    /// </summary>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>Task</returns>
    public async Task ResetDbSettingsAsync(CancellationToken token)
    {
      await Task.Run(
         () =>
         {
           if ( Monitor.TryEnter(DbLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
           {
             try
             {
               LOG.Info("Reset all database settings");

               SettingsHelperController.CurrentSettings.ProxySettings.Password = DefaultEnvironmentSettings.ProxyPassword;
               SettingsHelperController.CurrentSettings.SmtpSettings.Password = DefaultEnvironmentSettings.SmtpPassword;

               SetDefaultWindowSettings();

               UpdatePasswordSettings();
               UpdateFindDialogDbSettings();
               UpdateFindResultDbSettings();
               UpdateBookmarkOverviewDbSettings();
             }
             finally
             {
               Monitor.Exit(DbLock);
             }
           }
           else
           {
             LOG.Error("Can not lock!");
           }
         }, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates password settings
    /// </summary>
    public void UpdatePasswordSettings()
    {
      if ( Monitor.TryEnter(DbLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            DatabaseSetting setting = settings.Find(p => p.Key == ProxyPassword).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.ProxySettings.Password;
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == SmtpPassword).FirstOrDefault();

            if ( setting == null )
              return;

            setting.Value = SettingsHelperController.CurrentSettings.SmtpSettings.Password;
            settings.Update(setting);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(DbLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    /// <summary>
    /// Updates current DataBase settings
    /// </summary>
    public void UpdateFindResultDbSettings()
    {
      if ( Monitor.TryEnter(DbLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            DatabaseSetting setting = settings.Find(p => p.Key == FindResultWindowTop).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.FindResultPositionX.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == FindResultWindowLeft).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.FindResultPositionY.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == FindResultWindowHeight).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.FindResultHeight.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == FindResultWindowWidth).FirstOrDefault();

            if ( setting == null )
              return;

            setting.Value = SettingsHelperController.CurrentSettings.FindResultWidth.ToString(CultureInfo.InvariantCulture);
            settings.Update(setting);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(DbLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    /// <summary>
    /// Updates Bookmark overview DataBase settings
    /// </summary>
    public void UpdateBookmarkOverviewDbSettings()
    {
      if ( Monitor.TryEnter(DbLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            DatabaseSetting setting = settings.Find(p => p.Key == BookmarkOverviewWindowTop).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == BookmarkOverviewWindowLeft).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == BookmarkOverviewWindowHeight).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.BookmarkOverviewHeight.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == BookmarkOverviewWindowWidth).FirstOrDefault();

            if ( setting == null )
              return;

            setting.Value = SettingsHelperController.CurrentSettings.BookmarkOverviewWidth.ToString(CultureInfo.InvariantCulture);
            settings.Update(setting);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(DbLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    /// <summary>
    /// Updates FindDialog DataBase settings
    /// </summary>
    public void UpdateFindDialogDbSettings()
    {
      if ( Monitor.TryEnter(DbLock, TimeSpan.FromMilliseconds(LockTimeSpanIsMs)) )
      {
        try
        {
          using ( var db = new LiteDatabase(BusinessEnvironment.TailForWindowsDatabaseFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            DatabaseSetting setting = settings.Find(p => p.Key == FindDialogWindowTop).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.FindDialogPositionX.ToString(CultureInfo.InvariantCulture);
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == FindDialogWindowLeft).FirstOrDefault();

            if ( setting == null )
              return;

            setting.Value = SettingsHelperController.CurrentSettings.FindDialogPositionY.ToString(CultureInfo.InvariantCulture);
            settings.Update(setting);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
        }
        finally
        {
          Monitor.Exit(DbLock);
        }
      }
      else
      {
        LOG.Error("Can not lock!");
      }
    }

    private void AddMissingDbSettings(LiteCollection<DatabaseSetting> settings)
    {
      LOG.Info("Add missing database settings");

      DatabaseSetting setting = settings.Find(p => p.Key == BookmarkOverviewWindowTop).FirstOrDefault();

      if ( setting == null )
      {
        SetDefaultBookmarkOverviewSettings();
        AddBookmarkOverviewDbSettings(settings);
      }
    }

    private void AddSettingsToDataBase(LiteCollection<DatabaseSetting> settings)
    {
      LOG.Info("Create new database file");

      SetDefaultWindowSettings();

      AddFindResultDbSettings(settings);
      AddFindDialogDbSettings(settings);
      AddBookmarkOverviewDbSettings(settings);
      AddProxyDbSettings(settings);
      AddSmtpDbSettings(settings);
    }

    private static void AddSmtpDbSettings(LiteCollection<DatabaseSetting> settings)
    {
      var passwordSetting = new DatabaseSetting
      {
        Key = SmtpPassword,
        Value = DefaultEnvironmentSettings.SmtpPassword
      };
      settings.Insert(passwordSetting);
    }

    private static void AddProxyDbSettings(LiteCollection<DatabaseSetting> settings)
    {
      var passwordSetting = new DatabaseSetting
      {
        Key = ProxyPassword,
        Value = DefaultEnvironmentSettings.ProxyPassword
      };
      settings.Insert(passwordSetting);
    }

    private static void AddFindDialogDbSettings(LiteCollection<DatabaseSetting> settings)
    {
      var findDialogWindowPosition = new DatabaseSetting
      {
        Key = FindDialogWindowTop,
        Value = SettingsHelperController.CurrentSettings.FindDialogPositionX.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findDialogWindowPosition);

      findDialogWindowPosition = new DatabaseSetting
      {
        Key = FindDialogWindowLeft,
        Value = SettingsHelperController.CurrentSettings.FindDialogPositionY.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findDialogWindowPosition);
    }

    private static void AddFindResultDbSettings(LiteCollection<DatabaseSetting> settings)
    {
      var findResultWindowPosition = new DatabaseSetting
      {
        Key = FindResultWindowTop,
        Value = SettingsHelperController.CurrentSettings.FindResultPositionX.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findResultWindowPosition);

      findResultWindowPosition = new DatabaseSetting
      {
        Key = FindResultWindowLeft,
        Value = SettingsHelperController.CurrentSettings.FindResultPositionY.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findResultWindowPosition);

      var findResultSize = new DatabaseSetting
      {
        Key = FindResultWindowHeight,
        Value = SettingsHelperController.CurrentSettings.FindResultHeight.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findResultSize);

      findResultSize = new DatabaseSetting
      {
        Key = FindResultWindowWidth,
        Value = SettingsHelperController.CurrentSettings.FindResultWidth.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findResultSize);
    }

    private static void AddBookmarkOverviewDbSettings(LiteCollection<DatabaseSetting> settings)
    {
      var bookmarkOverviewWindowPosition = new DatabaseSetting
      {
        Key = BookmarkOverviewWindowTop,
        Value = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(bookmarkOverviewWindowPosition);

      bookmarkOverviewWindowPosition = new DatabaseSetting
      {
        Key = BookmarkOverviewWindowLeft,
        Value = SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(bookmarkOverviewWindowPosition);

      var bookmarkOverviewSize = new DatabaseSetting
      {
        Key = BookmarkOverviewWindowHeight,
        Value = SettingsHelperController.CurrentSettings.BookmarkOverviewHeight.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(bookmarkOverviewSize);

      bookmarkOverviewSize = new DatabaseSetting
      {
        Key = BookmarkOverviewWindowWidth,
        Value = SettingsHelperController.CurrentSettings.BookmarkOverviewWidth.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(bookmarkOverviewSize);
    }

    private void SetDefaultWindowSettings()
    {
      SetDefaultFindDialogSettings();
      SetDefaultFindResultSettings();
      SetDefaultBookmarkOverviewSettings();
    }

    private void SetDefaultFindDialogSettings()
    {
      SettingsHelperController.CurrentSettings.FindDialogPositionX = _workWidth - 400;
      SettingsHelperController.CurrentSettings.FindDialogPositionY = 0;
    }

    private void SetDefaultFindResultSettings()
    {
      SettingsHelperController.CurrentSettings.FindResultPositionX = _workWidth - 800;
      SettingsHelperController.CurrentSettings.FindResultPositionY = _workHeight - 300;
      SettingsHelperController.CurrentSettings.FindResultHeight = 300;
      SettingsHelperController.CurrentSettings.FindResultWidth = 800;
    }

    private void SetDefaultBookmarkOverviewSettings()
    {
      SettingsHelperController.CurrentSettings.BookmarkOverviewPositionX = _workWidth - 800;
      SettingsHelperController.CurrentSettings.BookmarkOverviewPositionY = _workHeight - 300;
      SettingsHelperController.CurrentSettings.BookmarkOverviewHeight = 300;
      SettingsHelperController.CurrentSettings.BookmarkOverviewWidth = 800;
    }
  }
}
