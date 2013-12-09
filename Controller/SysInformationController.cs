using TailForWin.Data;
using System.Reflection;
using TailForWin.Utils;
using System.Windows.Forms;
using System;
using System.Management;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using TailForWin.Native;


namespace TailForWin.Controller
{
  public class SysInformationController
  {
    /// <summary>
    /// Get systeminformations from computer
    /// </summary>
    /// <returns>Object with systeminformations</returns>
    public static SysInformationData GetAllSystemInformation ()
    {
      Assembly assembly = Assembly.GetExecutingAssembly ( );
      string format = string.Format ("{0} {1}", SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultDateFormat), SettingsData.GetEnumDescription (SettingsHelper.TailSettings.DefaultTimeFormat));
      string buildDateTime = (BuildDate.GetBuildDateTime (assembly)).ToString (format);

      SysInformationData sysInfo = new SysInformationData ( )
      {
        ApplicationName = LogFile.APPLICATION_CAPTION,
        BuildDateTime = buildDateTime,
        ApplicationVersion = Application.ProductVersion,
        MachineName = Environment.MachineName,
        OsName = GetOSFriendlyName ( ).Trim ( ),
        OsVersion = string.Format ("{0} {1} Build {2}", Environment.OSVersion.Version, Environment.OSVersion.ServicePack, Environment.OSVersion.Version.Build),
        OsType = string.Format ("{0} Bit-{1}", GetOSArchitecture ( ), System.Windows.Application.Current.FindResource ("OS")),
        HostIPAddress = GetIPAddress ( ),
        MachineMemory = GetMachineMemoryInfo ( ),
        Language = GetSystemLanguage ( ),
        CpuInfo = GetCPUInfo ( ),
      };
      return (sysInfo);
    }

    private static string GetOSFriendlyName ()
    {
      string result = string.Empty;

      using (ManagementObjectSearcher searcher = new ManagementObjectSearcher ("root\\CIMV2", "SELECT Caption FROM Win32_OperatingSystem"))
      {
        foreach (ManagementObject os in searcher.Get ( ))
        {
          result = os["Caption"].ToString ( );
          break;
        }
      }
      return (result);
    }

    private static int GetOSArchitecture ()
    {
      string pa = Environment.GetEnvironmentVariable ("PROCESSOR_ARCHITECTURE");

      return ((string.IsNullOrEmpty (pa) || string.Compare (pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
    }

    private static TailForWin.Data.IPAddress GetIPAddress ()
    {
      TailForWin.Data.IPAddress ipAddress = new TailForWin.Data.IPAddress ( );

      try
      {
        IPHostEntry lvsHost = Dns.GetHostEntry (Dns.GetHostName ( ));

        Array.ForEach (lvsHost.AddressList, currentAddress =>
        {
          if (String.Compare (currentAddress.AddressFamily.ToString ( ), ProtocolFamily.InterNetworkV6.ToString ( ), false) == 0)
            ipAddress.ipv6 = currentAddress.ToString ( );
          else
            ipAddress.ipv4 = currentAddress.ToString ( );
        });
      }
      catch (Exception ex)
      {
        Debug.WriteLine (ex);
      }
      return (ipAddress);
    }

    private static MemoryObject GetMachineMemoryInfo ()
    {
      MemoryObject memoryInfo = new MemoryObject ( );

      if (NativeMethods.GlobalMemoryStatusEx (memoryInfo))
        return (memoryInfo);
      else
        return (null);
    }

    private static CpuInfo GetCPUInfo ()
    {
      CpuInfo myCpu = new CpuInfo ( );

      using (ManagementObjectSearcher cpuInfo = new ManagementObjectSearcher ("SELECT * FROM Win32_Processor"))
      {
        foreach (ManagementObject cpu in cpuInfo.Get ( ))
        {
          myCpu.Manufacturer = cpu["Manufacturer"].ToString ( );
          myCpu.ClockSpeed = cpu["CurrentClockSpeed"].ToString ( );
          myCpu.Name = cpu["Name"].ToString ( );
        }
      }

      using (ManagementObjectSearcher cpuInfo = new ManagementObjectSearcher ("SELECT * FROM Win32_ComputerSystem "))
      {
        foreach (ManagementObject cpu in cpuInfo.Get ( ))
        {
          myCpu.NumberOfProcessors = cpu["NumberOfProcessors"].ToString ( );
          myCpu.LogicalNumberOfProcessors = cpu["NumberOfLogicalProcessors"].ToString ( );
        }
      }
      return (myCpu);
    }

    private static string GetSystemLanguage ()
    {
      return (Thread.CurrentThread.CurrentCulture.DisplayName);
    }
  }
}
