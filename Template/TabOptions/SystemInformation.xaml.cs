using Org.Vs.TailForWin.Controller;
using Org.Vs.TailForWin.Data;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Org.Vs.TailForWin.Template.TabOptions
{
  /// <summary>
  /// Interaction logic for SystemInformation.xaml
  /// </summary>
  public partial class SystemInformation
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SystemInformation ()
    {
      InitializeComponent();

      PreviewKeyDown += HandleEsc;

      InitListView();
    }

    private void InitListView ()
    {
      SysInformationData sysInfo = SysInformationController.GetAllSystemInformation();

      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Application name", sysInfo.ApplicationName));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Application version", sysInfo.ApplicationVersion));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Application build date", sysInfo.BuildDateTime));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Machine Name", sysInfo.MachineName));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Operating System", string.Format("{0} / {1}", sysInfo.OsName, sysInfo.OsType)));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Version", sysInfo.OsVersion));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("CPU name", sysInfo.CpuInfo.Name));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("CPU manufacturer", sysInfo.CpuInfo.Manufacturer));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("CPU speed", string.Format("{0} MHz", sysInfo.CpuInfo.ClockSpeed)));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("CPU physical number of processors", sysInfo.CpuInfo.NumberOfProcessors));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("CPU logical number of processors", sysInfo.CpuInfo.LogicalNumberOfProcessors));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Host IP address Ipv4/Ipv6", string.Format("{0} / {1}", sysInfo.HostIpAddress.Ipv4, sysInfo.HostIpAddress.Ipv6)));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Physical memory total", sysInfo.GuiTotalPhys));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Physical memory available", sysInfo.GuiAvailPhys));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Virtual memory total", sysInfo.GuiTotalVirtual));
      listViewSysInfo.Items.Add(new KeyValuePair<string, string>("Virtual memory available", sysInfo.GuiAvailVirtual));
    }

    private void btnClose_Click (object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void listViewSysInfo_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        btnClose_Click(sender, e);
    }
  }
}
