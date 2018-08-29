using System;
using System.Globalization;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Native.Data;


namespace Org.Vs.TailForWin.Business.Data.SystemInformation
{
  /// <summary>
  /// SystemInformationData object
  /// </summary>
  public class SystemInformationData
  {
    /// <summary>
    /// Appname
    /// </summary>
    public string ApplicationName
    {
      get;
      set;
    }

    /// <summary>
    /// Appversion
    /// </summary>
    public string ApplicationVersion
    {
      get;
      set;
    }

    /// <summary>
    /// Holds IP-addresses in IPv4 and IPv6
    /// </summary>
    public IpAddress HostIpAddress
    {
      get;
      set;
    }

    /// <summary>
    /// Machine name from user
    /// </summary>
    public string MachineName
    {
      get;
      set;
    }

    /// <summary>
    /// Which name has the OS (Windows XP, Windows Vista, Windows 7 ...)
    /// </summary>
    public string OsName
    {
      get;
      set;
    }

    /// <summary>
    /// Which version of OS, Service Pack included?
    /// </summary>
    public string OsVersion
    {
      get;
      set;
    }

    /// <summary>
    /// OS release Id
    /// </summary>
    public string OsReleaseId
    {
      get;
      set;
    }

    /// <summary>
    /// Is it a 32 or 64-Bit OS
    /// </summary>
    public string OsType
    {
      get;
      set;
    }

    /// <summary>
    /// Machine memory (e.g. physical memory, available memory ...)
    /// </summary>
    public MemoryObject MachineMemory
    {
      get;
      set;
    }

    /// <summary>
    /// System language
    /// </summary>
    public string Language
    {
      get;
      set;
    }

    /// <summary>
    /// LVS build date
    /// </summary>
    public string BuildDateTime
    {
      get;
      set;
    }

    /// <summary>
    /// CPU Information
    /// </summary>
    public CpuInfo CpuInfo
    {
      get;
      set;
    }

    /// <summary>
    /// Gui total memory
    /// </summary>
    public string GuiTotalPhys => (MachineMemory.ullTotalPhys / Math.Pow(1024, 3)).ToString("N2", SetNumberFormat()) + " GB";

    /// <summary>
    /// Gui available memory
    /// </summary>
    public string GuiAvailPhys => (MachineMemory.ullAvailPhys / Math.Pow(1024, 2)).ToString("N2", SetNumberFormat()) + " MB";

    /// <summary>
    /// Gui total virtual memory
    /// </summary>
    public string GuiTotalVirtual => (MachineMemory.ullTotalVirtual / Math.Pow(1024, 3)).ToString("N2", SetNumberFormat()) + " GB";

    /// <summary>
    /// Gui available virtual memory
    /// </summary>
    public string GuiAvailVirtual => (MachineMemory.ullAvailVirtual / Math.Pow(1024, 2)).ToString("N2", SetNumberFormat()) + " MB";

    /// <summary>
    /// Gui page file size
    /// </summary>
    public string GuiTotalPageFileSize => (MachineMemory.ullTotalPageFile / Math.Pow(1024, 3)).ToString("N2", SetNumberFormat()) + " GB";

    /// <summary>
    /// GuiAvailavlePageFileSize
    /// </summary>
    public string GuiAvailPageFileSize => (MachineMemory.ullAvailPageFile / Math.Pow(1024, 2)).ToString("N2", SetNumberFormat()) + " MB";

    /// <summary>
    /// SetNumberFormat
    /// </summary>
    /// <returns></returns>
    private NumberFormatInfo SetNumberFormat() => SettingsHelperController.CurrentSettings.CurrentCultureInfo.NumberFormat;
  }
}
