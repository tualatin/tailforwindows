using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Control container for T4W
  /// </summary>
  public class ControlContainer
  {
    private ControlContainer _instance;

    /// <summary>
    /// Current instance
    /// </summary>
    public ControlContainer Instance => _instance ?? (_instance = new ControlContainer());

    private ISettingsHelper _settings;

    private ControlContainer()
    {
      _settings = new SettingsHelper();
    }
  }
}
