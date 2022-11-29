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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
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

    public bool PageSettingsChanged => throw new NotImplementedException();
  }
}
