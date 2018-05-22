using System;
using System.IO;
using LiteDB;
using log4net;
using Org.Vs.TailForWin.Business.DbEngine.Data;
using Org.Vs.TailForWin.Business.DbEngine.Interfaces;


namespace Org.Vs.TailForWin.Business.DbEngine.Controllers
{
  /// <summary>
  /// Settings DataBase controller
  /// </summary>
  public class SettingsDbController : ISettingsDbController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SettingsDbController));

    private readonly string _dbFile;


    /// <summary>
    /// Standard constructor
    /// </summary>
    public SettingsDbController() => _dbFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\T4W.db";

    /// <summary>
    /// Read current DataBase settings
    /// </summary>
    public void ReadDbSettings()
    {
      try
      {
        using ( var db = new LiteDatabase(_dbFile) )
        {
          db.Shrink();

          var settings = db.GetCollection<DatabaseSetting>("settings");

          if ( settings.Count() == 0 )
            AddSettingsToDataBase(settings);

          settings = db.GetCollection<DatabaseSetting>("settings");
          settings.EnsureIndex(p => p.Id);

          var count = settings.Count();
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
    }

    private void AddSettingsToDataBase(LiteCollection<DatabaseSetting> settings)
    {
      var findResultWindowPosition = new DatabaseSetting
      {
        Key = "FindResultWndTop",
        Value = "100"
      };
      settings.Insert(findResultWindowPosition);

      findResultWindowPosition = new DatabaseSetting
      {
        Key = "FindResultWndLeft",
        Value = "100"
      };
      settings.Insert(findResultWindowPosition);
    }
  }
}
