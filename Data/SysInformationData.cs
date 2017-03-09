using System;
using System.Globalization;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// SysInformationData object
  /// </summary>
  public class SysInformationData
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
    public string GuiTotalPhys
    {
      get
      {
        return ((MachineMemory.ullTotalPhys / Math.Pow(1024, 3)).ToString("N2", SetNumberFormat()) + " GB");
      }
    }

    /// <summary>
    /// Gui available memory
    /// </summary>
    public string GuiAvailPhys
    {
      get
      {
        return ((MachineMemory.ullAvailPhys / Math.Pow(1024, 2)).ToString("N2", SetNumberFormat()) + " MB");
      }
    }

    /// <summary>
    /// Gui total virtual memory
    /// </summary>
    public string GuiTotalVirtual
    {
      get
      {
        return ((MachineMemory.ullTotalVirtual / Math.Pow(1024, 3)).ToString("N2", SetNumberFormat()) + " GB");
      }
    }

    /// <summary>
    /// Gui available virtual memory
    /// </summary>
    public string GuiAvailVirtual
    {
      get
      {
        return ((MachineMemory.ullAvailVirtual / Math.Pow(1024, 2)).ToString("N2", SetNumberFormat()) + " MB");
      }
    }

    /// <summary>
    /// Gui page file size
    /// </summary>
    public string GuiTotalPageFileSize
    {
      get
      {
        return ((MachineMemory.ullTotalPageFile / Math.Pow(1024, 3)).ToString("N2", SetNumberFormat()) + " GB");
      }
    }

    /// <summary>
    /// GuiAvailavlePageFileSize
    /// </summary>
    public string GuiAvailPageFileSize
    {
      get
      {
        return ((MachineMemory.ullAvailPageFile / Math.Pow(1024, 2)).ToString("N2", SetNumberFormat()) + " MB");
      }
    }

    /// <summary>
    /// SetNumberFormat
    /// </summary>
    /// <returns></returns>
    private NumberFormatInfo SetNumberFormat()
    {
      if(string.IsNullOrEmpty(CultureNumberFormat))
        return (new CultureInfo("de-DE", false).NumberFormat);

      int pos = CultureNumberFormat.IndexOf('_');
      char[] snipped = CultureNumberFormat.ToCharArray();
      snipped[pos] = '-';

      return (new CultureInfo(new string(snipped), false).NumberFormat);
    }

    /// <summary>
    /// Culture number format
    /// </summary>
    public string CultureNumberFormat
    {
      get;
      set;
    }
  }

  /// <summary>
  /// IPAddress Class
  /// </summary>
  public class IpAddress
  {
    /// <summary>
    /// IPv4 address
    /// </summary>
    public string Ipv4
    {
      get;
      set;
    }

    /// <summary>
    /// IPv6 address
    /// </summary>
    public string Ipv6
    {
      get;
      set;
    }
  }

  /// <summary>
  /// CPU Info
  /// </summary>
  public class CpuInfo
  {
    /// <summary>
    /// Full name of CPU
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Clockspeed of CPU
    /// </summary>
    public string ClockSpeed
    {
      get;
      set;
    }

    /// <summary>
    /// CPU manufacturer
    /// </summary>
    public string Manufacturer
    {
      get;
      set;
    }

    /// <summary>
    /// Native number of processors
    /// </summary>
    public string NumberOfProcessors
    {
      get;
      set;
    }

    /// <summary>
    /// Logical number of processors
    /// </summary>
    public string LogicalNumberOfProcessors
    {
      get;
      set;
    }
  }
}
