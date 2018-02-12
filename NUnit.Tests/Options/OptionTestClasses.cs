using Org.Vs.TailForWin.Business.Interfaces;

namespace Org.Vs.NUnit.Tests.Options
{
  public class GeneralOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Generial options";
  }

  public class GeneralProxyOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Proxy settings";
  }

  public class GeneralResetOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Reset current settings";
  }

  public class ExtraOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Extra settings";
  }

  public class ExtraSmartWatch : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "SmartWatch";
  }

  public class AboutOptions : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "About";
  }

  public class AboutSystemInformations : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "System information";
  }

  public class AboutUpdate : IOptionPage
  {
    public string PageTitle
    {
      get;
    } = "Update configuration";
  }
}
