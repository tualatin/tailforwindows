using System;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.Interfaces;

namespace Org.Vs.NUnit.Tests.Options
{
  public class GeneralOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Generial options";

    public Guid PageId
    {
      get;
    }
  }

  public class GeneralProxyOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Proxy settings";

    public Guid PageId
    {
      get;
    }
  }

  public class GeneralResetOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Reset current settings";

    public Guid PageId
    {
      get;
    }
  }

  public class ExtraOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Extra settings";

    public Guid PageId
    {
      get;
    }
  }

  public class ExtraSmartWatch : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "SmartWatch";

    public Guid PageId
    {
      get;
    }
  }

  public class AboutOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "About";

    public Guid PageId
    {
      get;
    }
  }

  public class AboutSystemInformations : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "System information";

    public Guid PageId
    {
      get;
    }
  }

  public class AboutUpdate : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Update configuration";

    public Guid PageId
    {
      get;
    }
  }
}
