using System;
using System.Diagnostics;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Commands;
using Org.Vs.TailForWin.UI.ExtendedControls;


namespace Org.Vs.TailForWin.UI.ViewModels
{
  /// <summary>
  /// Auto update view model
  /// </summary>
  public class AutoUpdateViewModel : NotifyMaster
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(AutoUpdateViewModel));

    #region Commands

    private ICommand _closeCommand;

    /// <summary>
    /// Close command
    /// </summary>
    public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand(p => ExecuteCloseCommand((WindowEx) p)));

    private ICommand _visitWebsiteCommand;

    /// <summary>
    /// Visivit website command
    /// </summary>
    public ICommand VisitWebsiteCommand => _visitWebsiteCommand ?? (_visitWebsiteCommand = new RelayCommand(p => ExecuteVisitWebsiteCommand()));

    #endregion

    #region Command functions

    private void ExecuteCloseCommand(WindowEx window) => window?.Close();

    private void ExecuteVisitWebsiteCommand()
    {
      try
      {
        var url = new Uri(EnvironmentContainer.ApplicationReleaseWebUrl);
        Process.Start(new ProcessStartInfo(url.AbsoluteUri));
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
    }

    #endregion
  }
}
