using System;
using System.Globalization;
using System.IO;
using System.Linq;
using LiteDB;
using log4net;
using Org.Vs.TailForWin.Business.DbEngine.Data;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;


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

    private const string Settings = "Settings";

    private const string FindResultWindowTop = "FindResultWndTop";
    private const string FindResultWindowLeft = "FindResultWndLeft";
    private const string FindResultWindowHeight = "FindResultWndHeight";
    private const string FindResultWindowWidth = "FindResultWndWidth";

    private const string FindDialogWindowTop = "FindDialogWndTop";
    private const string FindDialogWindowLeft = "FindDialogWndLeft";

    private static SettingsDbController instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public static SettingsDbController Instance => instance ?? (instance = new SettingsDbController());

    private SettingsDbController() => _dbFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\T4W.db";

    /// <summary>
    /// Read current DataBase settings
    /// </summary>
    public void ReadDbSettings()
    {
      try
      {
        lock ( DbLock )
        {
          using ( var db = new LiteDatabase(_dbFile) )
          {
            db.Shrink();

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
          }
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    /// <summary>
    /// Updates current DataBase settings
    /// </summary>
    public void UpdateFindResultDbSettings()
    {
      lock ( DbLock )
      {
        using ( var db = new LiteDatabase(_dbFile) )
        {
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
    }

    /// <summary>
    /// Updates FindDialog DataBase settings
    /// </summary>
    public void UpdateFindDialogDbSettings()
    {
      lock ( DbLock )
      {
        using ( var db = new LiteDatabase(_dbFile) )
        {
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
    }

    private void AddSettingsToDataBase(LiteCollection<DatabaseSetting> settings)
    {
      double workWidth = System.Windows.SystemParameters.WorkArea.Width;
      double workHeight = System.Windows.SystemParameters.WorkArea.Height;

      double findResultWidth = workWidth - 800;
      double findResultHeight = workHeight - 300;
      double findDialogWidth = workWidth - 400;

      var findResultWindowPosition = new DatabaseSetting
      {
        Key = FindResultWindowTop,
        Value = findResultWidth.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findResultWindowPosition);

      findResultWindowPosition = new DatabaseSetting
      {
        Key = FindResultWindowLeft,
        Value = findResultHeight.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findResultWindowPosition);

      var findResultSize = new DatabaseSetting
      {
        Key = FindResultWindowHeight,
        Value = "300"
      };
      settings.Insert(findResultSize);

      findResultSize = new DatabaseSetting
      {
        Key = FindResultWindowWidth,
        Value = "800"
      };
      settings.Insert(findResultSize);

      var findDialogWindowPosition = new DatabaseSetting
      {
        Key = FindDialogWindowTop,
        Value = findDialogWidth.ToString(CultureInfo.InvariantCulture)
      };
      settings.Insert(findDialogWindowPosition);

      findDialogWindowPosition = new DatabaseSetting
      {
        Key = FindDialogWindowLeft,
        Value = "0"
      };
      settings.Insert(findDialogWindowPosition);
    }
  }
}
