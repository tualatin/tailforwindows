using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Org.Vs.TailForWin.Business.Data.SystemInformation;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Extensions;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.Core.Native.Data;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Business.Controllers
{
  /// <summary>
  /// System information controller
  /// </summary>
  public class SystemInformationController
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(SystemInformationController));

    /// <summary>
    /// Get systeminformations from computer
    /// </summary>
    /// <returns>Object with systeminformations</returns>
    public async Task<SystemInformationData> GetAllSystemInformationsAsync(CancellationToken token)
    {
      LOG.Trace("Get System informations");
      SystemInformationData sysInfo = new SystemInformationData();

      await Task.Run(() =>
      {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string format = $"{SettingsHelperController.CurrentSettings.DefaultDateFormat.GetEnumDescription()}" +
                        $"{SettingsHelperController.CurrentSettings.DefaultTimeFormat.GetEnumDescription()}";
        string buildDateTime = BuildDate.GetBuildDateTime(assembly).ToString(format);

        sysInfo.ApplicationName = EnvironmentContainer.ApplicationTitle;
        sysInfo.BuildDateTime = buildDateTime;
        sysInfo.ApplicationVersion = Application.ProductVersion;
        sysInfo.MachineName = Environment.MachineName;
        sysInfo.OsName = GetOsFriendlyName().Trim();
        sysInfo.OsVersion = $"{Environment.OSVersion.Version} {Environment.OSVersion.ServicePack} Build {Environment.OSVersion.Version.Build}";
        sysInfo.OsType = $"{GetOsArchitecture()} Bit-{System.Windows.Application.Current.TryFindResource("Os")}";
        sysInfo.HostIpAddress = GetIpAddress();
        sysInfo.MachineMemory = GetMachineMemoryInfo();
        sysInfo.Language = GetSystemLanguage();
        sysInfo.CpuInfo = GetCpuInfo();
      }, token).ConfigureAwait(false);

      return sysInfo;
    }

    private static string GetOsFriendlyName()
    {
      string result = string.Empty;

      using ( ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT Caption FROM Win32_OperatingSystem") )
      {
        foreach ( var os in searcher.Get().Cast<ManagementObject>() )
        {
          result = os["Caption"].ToString();
          break;
        }
      }
      return result;
    }

    private static int GetOsArchitecture()
    {
      string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

      return string.IsNullOrEmpty(pa) || string.Compare(pa, 0, "x86", 0, 3, true) == 0 ? 32 : 64;
    }

    private static IpAddress GetIpAddress()
    {
      IpAddress ipAddress = new IpAddress();

      try
      {
        IPHostEntry lvsHost = Dns.GetHostEntry(Dns.GetHostName());

        Array.ForEach(lvsHost.AddressList, currentAddress =>
        {
          if ( string.CompareOrdinal(currentAddress.AddressFamily.ToString(), ProtocolFamily.InterNetworkV6.ToString()) == 0 )
            ipAddress.Ipv6 = currentAddress.ToString();
          else
            ipAddress.Ipv4 = currentAddress.ToString();
        });
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", MethodBase.GetCurrentMethod().Name, ex.GetType().Name);
      }
      return ipAddress;
    }

    private static MemoryObject GetMachineMemoryInfo()
    {
      MemoryObject memoryInfo = new MemoryObject();

      return NativeMethods.GlobalMemoryStatusEx(memoryInfo) ? memoryInfo : null;
    }

    private static CpuInfo GetCpuInfo()
    {
      CpuInfo myCpu = new CpuInfo();

      using ( ManagementObjectSearcher cpuInfo = new ManagementObjectSearcher("SELECT * FROM Win32_Processor") )
      {
        foreach ( var cpu in cpuInfo.Get().Cast<ManagementObject>() )
        {
          myCpu.Manufacturer = cpu["Manufacturer"].ToString();
          myCpu.ClockSpeed = cpu["CurrentClockSpeed"].ToString();
          myCpu.Name = cpu["Name"].ToString();
        }
      }

      using ( ManagementObjectSearcher cpuInfo = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem ") )
      {
        foreach ( var cpu in cpuInfo.Get().Cast<ManagementObject>() )
        {
          myCpu.NumberOfProcessors = cpu["NumberOfProcessors"].ToString();
          myCpu.LogicalNumberOfProcessors = cpu["NumberOfLogicalProcessors"].ToString();
        }
      }
      return myCpu;
    }

    private static string GetSystemLanguage() => Thread.CurrentThread.CurrentCulture.DisplayName;
  }
}
