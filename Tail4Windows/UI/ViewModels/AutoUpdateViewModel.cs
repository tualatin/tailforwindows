using System;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.UI.UserControls.Interfaces;


namespace Org.Vs.TailForWin.UI.ViewModels
{
  /// <summary>
  /// Auto update view model
  /// </summary>
  public class AutoUpdateViewModel : NotifyMaster, IUpdateControlViewModel
  {
    public string ApplicationVersion
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
    public string WebVersion
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
    public string UpdateHint
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }
  }
}
