using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Create Mutex class
  /// </summary>
  public static class GlobalMutex
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(GlobalMutex));

    private readonly static List<KeyValuePair<string, Mutex>> mutexes = new List<KeyValuePair<string, Mutex>>();

    /// <summary>
    /// Create new Mutex
    /// </summary>
    /// <param name="strName">Name of Mutex</param>
    /// <param name="bInitialOwned">Owned Init</param>
    /// <returns>If success than true otherwise false</returns>
    public static bool CreateMutext (string strName, bool bInitialOwned)
    {
      return (CreateMutexWin32(strName, bInitialOwned));
    }

    private static bool CreateMutexWin32 (string strName, bool bInitialOwned)
    {
      try
      {
        bool bMyMutex;
        Mutex m = new Mutex(bInitialOwned, strName, out bMyMutex);

        if (bMyMutex)
        {
          mutexes.Add(new KeyValuePair<string, Mutex>(strName, m));
          return (true);
        }
      }
      catch (Exception ex)
      {
        LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return (false);
    }

    /// <summary>
    /// Delete Mutex by name
    /// </summary>
    /// <param name="strName">Name of Mutex</param>
    /// <returns>true if success otherwise false</returns>
    public static bool ReleaseMutex (string strName)
    {
      return (ReleaseMutexWin32(strName));
    }

    private static bool ReleaseMutexWin32 (string strName)
    {
      foreach (var item in mutexes.Select((x, i) => new
      {
        Value = x,
        Index = i
      }))
      {
        if (item.Value.Key.Equals(strName, StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            item.Value.Value.ReleaseMutex();
            item.Value.Value.Close();
          }
          catch (Exception ex)
          {
            LOG.Error(ex, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
          }
        }
        mutexes.RemoveAt(item.Index);

        return (true);
      }
      return (false);
    }

    /// <summary>
    /// Delete all known mutexes
    /// </summary>
    public static void ReleaseAll ()
    {
      for (int i = mutexes.Count - 1; i >= 0; --i)
        ReleaseMutex(mutexes[i].Key);
    }
  }
}
