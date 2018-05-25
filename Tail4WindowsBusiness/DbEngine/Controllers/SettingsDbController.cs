using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using log4net;
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

    private readonly string _dbFile;

    #region Setting keys

    private const string Settings = "Settings";

    private const string FindResultWindowTop = "FindResultWndTop";
    private const string FindResultWindowLeft = "FindResultWndLeft";
    private const string FindResultWindowHeight = "FindResultWndHeight";
    private const string FindResultWindowWidth = "FindResultWndWidth";

    private const string FindDialogWindowTop = "FindDialogWndTop";
    private const string FindDialogWindowLeft = "FindDialogWndLeft";

    private const string ProxyPassword = "Proxy.Password";
    private const string SmtpPasword = "Smtp.Password";

    #endregion

    private static SettingsDbController instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static SettingsDbController Instance => instance ?? (instance = new SettingsDbController());

    private SettingsDbController() => _dbFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\T4W.db";

    /// <summary>
    /// Read current DataBase settings
    /// </summary>
    /// <returns>Task</returns>
    public async Task ReadDbSettingsAsync()
    {
      await Task.Run(
        () =>
        {
          lock ( DbLock )
          {
            try
            {
              using ( var db = new LiteDatabase(_dbFile) )
              {
                var settings = db.GetCollection<DatabaseSetting>(Settings);

                if ( settings.Count() == 0 )
                  AddSettingsToDataBase(settings);

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

                // FindDialog position
                setting = settings.Find(p => p.Key == FindDialogWindowTop).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindDialogPositionX = Convert.ToDouble(setting?.Value);
                setting = settings.Find(p => p.Key == FindDialogWindowLeft).FirstOrDefault();
                SettingsHelperController.CurrentSettings.FindDialogPositionY = Convert.ToDouble(setting?.Value);

                // Proxy password
                setting = settings.Find(p => p.Key == ProxyPassword).FirstOrDefault();
                SettingsHelperController.CurrentSettings.ProxySettings.Password = setting?.Value;

                // SMTP password
                setting = settings.Find(p => p.Key == SmtpPasword).FirstOrDefault();
                SettingsHelperController.CurrentSettings.SmtpSettings.Password = setting?.Value;
              }
            }
            catch ( Exception ex )
            {
              LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
            }
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
           lock ( DbLock )
           {
             LOG.Info("Reset all database settings");

             SettingsHelperController.CurrentSettings.ProxySettings.Password = DefaultEnvironmentSettings.ProxyPassword;
             SettingsHelperController.CurrentSettings.SmtpSettings.Password = DefaultEnvironmentSettings.SmtpPassword;

             SetDefaultWindowSettings();

             UpdatePasswordSettings();
             UpdateFindDialogDbSettings();
             UpdateFindResultDbSettings();
           }
         }, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates password settings
    /// </summary>
    public void UpdatePasswordSettings()
    {
      lock ( DbLock )
      {
        LOG.Trace("Save passwords");

        try
        {
          using ( var db = new LiteDatabase(_dbFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            var setting = settings.Find(p => p.Key == ProxyPassword).FirstOrDefault();

            if ( setting != null )
            {
              setting.Value = SettingsHelperController.CurrentSettings.ProxySettings.Password;
              settings.Update(setting);
            }

            setting = settings.Find(p => p.Key == SmtpPasword).FirstOrDefault();

            if ( setting == null )
              return;

            setting.Value = SettingsHelperController.CurrentSettings.SmtpSettings.Password;
            settings.Update(setting);
          }
        }
        catch ( Exception ex )
        {
          LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
      }
    }

    /// <summary>
    /// Updates current DataBase settings
    /// </summary>
    public void UpdateFindResultDbSettings()
    {
      lock ( DbLock )
      {
        try
        {
          using ( var db = new LiteDatabase(_dbFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            var setting = settings.Find(p => p.Key == FindResultWindowTop).FirstOrDefault();

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
          LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
      }
    }

    /// <summary>
    /// Updates FindDialog DataBase settings
    /// </summary>
    public void UpdateFindDialogDbSettings()
    {
      lock ( DbLock )
      {
        try
        {
          using ( var db = new LiteDatabase(_dbFile) )
          {
            db.Shrink();

            var settings = db.GetCollection<DatabaseSetting>(Settings);
            var setting = settings.Find(p => p.Key == FindDialogWindowTop).FirstOrDefault();

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
          LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
        }
      }
    }

    private void AddSettingsToDataBase(LiteCollection<DatabaseSetting> settings)
    {
      LOG.Info("Create new database file");

      SetDefaultWindowSettings();

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

      var passwordSetting = new DatabaseSetting
      {
        Key = ProxyPassword,
        Value = DefaultEnvironmentSettings.ProxyPassword
      };
      settings.Insert(passwordSetting);

      passwordSetting = new DatabaseSetting
      {
        Key = SmtpPasword,
        Value = DefaultEnvironmentSettings.SmtpPassword
      };
      settings.Insert(passwordSetting);
    }

    private void SetDefaultWindowSettings()
    {
      double workWidth = System.Windows.SystemParameters.WorkArea.Width;
      double workHeight = System.Windows.SystemParameters.WorkArea.Height;

      double findResultWidth = workWidth - 800;
      double findResultHeight = workHeight - 300;
      double findDialogWidth = workWidth - 400;

      SettingsHelperController.CurrentSettings.FindResultPositionX = findResultWidth;
      SettingsHelperController.CurrentSettings.FindResultPositionY = findResultHeight;
      SettingsHelperController.CurrentSettings.FindResultHeight = 300;
      SettingsHelperController.CurrentSettings.FindResultWidth = 800;

      SettingsHelperController.CurrentSettings.FindDialogPositionX = findDialogWidth;
      SettingsHelperController.CurrentSettings.FindDialogPositionY = 0;
    }
  }
}
