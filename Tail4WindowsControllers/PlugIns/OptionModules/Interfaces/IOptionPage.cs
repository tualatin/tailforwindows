﻿using System;


namespace Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces
{
  /// <summary>
  /// Interface for option pages
  /// </summary>
  public interface IOptionPage
  {
    /// <summary>
    /// Current page title
    /// </summary>
    string PageTitle
    {
      get;
    }

    /// <summary>
    /// Page GuId
    /// </summary>
    Guid PageId
    {
      get;
    }
  }
}
